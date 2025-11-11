using Cysharp.Threading.Tasks;
using EventChannel;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
namespace Managers
{

    public class DayManager : MonoBehaviour
    {
        [Title("Invoked Events")]
        [SerializeField] private SaveTypeVoidUniTaskEventSO _autoSaveGameEvent;

        [Title("Listening to Events")]
        [SerializeField] private GameStateDataEventSO _loadGameDataEvent;
        [SerializeField] private VoidUniTaskEventSO _respawnNextDayEvent;

        [Title("Async Events")]
        [SerializeField] private VoidAsyncEventSO _dayChangeAnimationEvent;

        [Title("Responding to Events")]
        [SerializeField] private VoidIntRequestEventSO _changeDaySO;

        [ShowInInspector] public int CurrentDay { get; private set; } = 0;

        #region Singleton
        // singleton reference
        private static DayManager _instance;
        public static DayManager Instance { get { return _instance; } }

        private void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        private void OnEnable()
        {
            _loadGameDataEvent.OnInvokeEvent += OnLoadGameData;
            _changeDaySO.OnRequestEvent += OnDayChangeRequest;
            _dayChangeAnimationEvent.OnEndEvent += ChangeDay;
            _respawnNextDayEvent.OnStartEvent += ChangeDay;
        }
        private void OnDisable()
        {
            _loadGameDataEvent.OnInvokeEvent -= OnLoadGameData;
            _changeDaySO.OnRequestEvent -= OnDayChangeRequest;
            _dayChangeAnimationEvent.OnEndEvent -= ChangeDay;
            _respawnNextDayEvent.OnStartEvent -= ChangeDay;
        }

        #region Load Data
        private void OnLoadGameData(SaveData.GameStateData data)
        {
            CurrentDay = data.CurrentDay;
        }
        #endregion

        private void OnDayChangeRequest()
        {
            _dayChangeAnimationEvent.StartEvent();
        }

        private void ChangeDay()
        {
            if (ShouldDayBeChanged())
            {
                CurrentDay++;
            }
            _changeDaySO.OnResultEvent.Invoke(CurrentDay);
            _autoSaveGameEvent.StartEvent(SaveData.SaveType.DayStart);
        }

        private bool ShouldDayBeChanged()
        {
            return true;
        }

        [Button]
        public void InvokeDayChange()
        {
            _changeDaySO.RequestEvent();
        }
        [Button]
        public void ChangeDay(int day)
        {
            CurrentDay = day;
            _changeDaySO.OnResultEvent.Invoke(CurrentDay);
        }
    }
}