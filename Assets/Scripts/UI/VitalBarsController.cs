using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

using EventChannel;

namespace UserInterface
{
    public class VitalBarsController : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private Image _healthBar, _manaBar;
        [Title("Listening to Events")]
        [SerializeField] private FloatVector2RequestEventSO _changeHealthSO;
        [SerializeField] private Vector2EventSO _manaChangedSO;

        private void OnEnable()
        {
            _changeHealthSO.OnResultEvent += UpdateHealthUI;
            _manaChangedSO.OnInvokeEvent += UpdateManaUI;
        }

        private void OnDisable()
        {
            _changeHealthSO.OnResultEvent -= UpdateHealthUI;
            _manaChangedSO.OnInvokeEvent -= UpdateManaUI;
        }

        private void UpdateHealthUI(Vector2 health)
        {
            // calculate new fill percent
            float newFill = health.x / health.y;

            // send to coroutine
            StartCoroutine(ChangeFill(_healthBar, _healthBar.fillAmount, newFill, 0.5f));
        }

        private void UpdateManaUI(Vector2 mana)
        {
            //Debug.Log("updating mana");
            // calculate new fill percent
            float newFill = mana.x / mana.y;

            // send to coroutine
            StartCoroutine(ChangeFill(_manaBar, _manaBar.fillAmount, newFill, 0.1f));
        }

        private IEnumerator ChangeFill(Image bar, float oldFill, float newFill, float seconds)
        {
            // gradually change image bar fill over seconds
            for (float time = 0; time <= seconds; time += Time.unscaledDeltaTime)
            {
                bar.fillAmount = Mathf.Lerp(oldFill, newFill, time / seconds);
                yield return null;
            }
        }
    }
}
