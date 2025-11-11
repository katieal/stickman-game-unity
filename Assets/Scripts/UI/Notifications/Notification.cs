using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

namespace UserInterface.Notification
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _icon;
        [SerializeField] private CanvasGroup _canvasGroup;

        public bool IsFading = false;

        public void Init(Sprite sprite, string message, float secondsUntilFade, float fadeDuration)
        {
            // set icon from sprite library
            _icon.sprite = sprite;
            _text.text = message;

            StartCoroutine(StartFade(secondsUntilFade, fadeDuration));
        }

        IEnumerator StartFade(float waitTime, float duration)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            StartCoroutine(Fade(duration));
        }

        IEnumerator Fade(float duration)
        {
            IsFading = true;
            for (float time = 0; time <= duration; time += Time.unscaledDeltaTime)
            {
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / duration);
                yield return null;
            }
        }

        public void DestroySelf() { Destroy(gameObject); }
    }
}
