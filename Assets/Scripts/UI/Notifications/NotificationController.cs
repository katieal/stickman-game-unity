using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace UserInterface.Notification
{
    public enum Type { Normal, Confirm, Alert, Error }

    public struct NotificationInstance
    {
        public string Key;
        public Notification Notification;
        public Coroutine Coroutine;
    }
    /*
     * Note to self: not sure if instantiating notification prefabs is the best/most efficient
     *              way to do this, but thinking about that comes later!
     * 
     */

    public class NotificationController : SerializedMonoBehaviour
    {
        [Title("Components")]
        [Tooltip("Panel which notifications will be childed to.")]
        [SerializeField] private GameObject _notificationPanel;
        [Tooltip("Prefab for individual notification")]
        [SerializeField] private GameObject _notificationPrefab;

        [Tooltip("Notification Dict SO")]
        [SerializeField] private NotificationDataSO _notificationDict;

        [Tooltip("Icon sprite library")]
        [SerializeField] private SpriteLibrary _sprites;

        [BoxGroup("Events", CenterLabel = true)]
        [TitleGroup("Events/Listening to Events")]
        [SerializeField] private EventChannel.StringBoolEventSO _notificationSO;

        // seconds until notification fade animation begins
        private readonly float _secondsUntilFade = 3f;
        // duration of notification fade animation 
        private readonly float _fadeDuration = 2f;
        // max number of notifications that can be displayed at once
        private readonly int _maxCapacity = 3;

        // holds currently displayed notifications
        private List<NotificationInstance> _currentNotifs = new(3);


        private void OnEnable()
        {
            _notificationSO.OnInvokeEvent += SendNotification;
        }
        private void OnDisable()
        {
            _notificationSO.OnInvokeEvent -= SendNotification;
        }

        private void SendNotification(string key, bool shouldUseVariant)
        {
            #region Debug
            Debug.Assert(key != null && _notificationDict.Messages.ContainsKey(key), "Invalid notification key.");
            if (shouldUseVariant) { Debug.Assert(_notificationDict.Messages[key].HasVariant, "Variant requested on a message with no variant!"); }
            #endregion

            // check for duplicates to prevent spam
            if (_currentNotifs.Count > 0 && IsDuplicate(key)) { return; }

            // if max num of notifications is already displayed, destroy oldest one
            if (_currentNotifs.Count >= _maxCapacity) { DestroyFirst(); }

            // instantiate new notification
            NotificationInstance newNotif = new()
            {
                Key = key,
                Notification = Instantiate(_notificationPrefab, _notificationPanel.transform).GetComponent<Notification>()
            };

            // determine notification fields
            string message = (shouldUseVariant) ? _notificationDict.Messages[key].Variant : _notificationDict.Messages[key].Message;
            Sprite sprite = _sprites.GetSprite("Main", _notificationDict.Messages[key].Type.ToString());

            newNotif.Notification.Init(sprite, message, _secondsUntilFade, _fadeDuration);

            // start coroutine to destroy after notif has completely faded
            newNotif.Coroutine = StartCoroutine(DestroyNotification(newNotif, _secondsUntilFade + _fadeDuration));
            _currentNotifs.Add(newNotif);
        }

        // determines if notification is the same as the most recent notification and isn't fading
        private bool IsDuplicate(string key)
        {
            return _currentNotifs[^1].Key == key && !_currentNotifs[^1].Notification.IsFading;
        }

        // destroy oldest notification early
        private void DestroyFirst()
        {
            StopCoroutine(_currentNotifs[0].Coroutine);
            _currentNotifs[0].Notification.DestroySelf();
            _currentNotifs.RemoveAt(0);
        }

        IEnumerator DestroyNotification(NotificationInstance notif, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);

            // destroy gameobject
            notif.Notification.DestroySelf();

            // when notification expires, it will always be oldest aka last in list
            // remove last notification in list
            _currentNotifs.RemoveAt(_currentNotifs.Count - 1);
        }
    }
}
