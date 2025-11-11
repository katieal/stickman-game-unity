using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

using EventChannel;

namespace UserInterface
{
    public class InfoPanelController : MonoBehaviour
    {

        [Header("Components")]
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _timeText;

        [Header("Listening to Events")]
        [SerializeField] private VoidIntRequestEventSO _changeDaySO;
        [SerializeField] private IntIntRequestEventSO _changeMoneySO;
        [SerializeField] private IntEventSO _timeChangedEvent;

        private void OnEnable()
        {
            _changeDaySO.OnResultEvent += OnDayChanged;
            _changeMoneySO.OnResultEvent += OnMoneyChanged;
            _timeChangedEvent.OnInvokeEvent += OnTimeChanged;
        }
        private void OnDisable()
        {
            _changeDaySO.OnResultEvent -= OnDayChanged;
            _changeMoneySO.OnResultEvent -= OnMoneyChanged;
            _timeChangedEvent.OnInvokeEvent -= OnTimeChanged;
        }

        private void OnDayChanged(int number)
        {
            _dayText.text = number.ToString();
        }
        private void OnMoneyChanged(int newBalance)
        {
            StopAllCoroutines();
            // get starting value
            int oldBalance = int.Parse(_moneyText.text);

            // show animation 
            StartCoroutine(ShowBalanceChange(oldBalance, newBalance, 1f));
        }
        private void OnTimeChanged(int newTime)
        {
            int hours = newTime / 60;
            int minutes = newTime % 60;
            _timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }

        IEnumerator ShowBalanceChange(int oldBalance, int newBalance, float seconds)
        {
            // gradually change number displayed over seconds
            for (float time = 0; time <= seconds; time += Time.unscaledDeltaTime)
            {
                _moneyText.text = ((int)Mathf.Lerp(oldBalance, newBalance, time / seconds)).ToString();
                yield return null;
            }
            _moneyText.text = newBalance.ToString();
        }
    }
}
