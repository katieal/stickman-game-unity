using UnityEngine;


using Sirenix.OdinInspector;
using System.Collections;
using EventChannel;
using Cysharp.Threading.Tasks;

namespace Managers
{
    /// <summary>
    /// want this class to handle game state pausing from now on?
    /// </summary>
    public class ScreenEffectsManager : MonoBehaviour
    {
        [Title("Background Effect Components")]
        [SerializeField] private GameObject _greyOutPanel;
        [Title("Main Effect Components")]
        [SerializeField] private GameObject _dayChangePanel;
        [SerializeField] private CanvasGroup _dayChangeGroup;
        [SerializeField] private GameObject _dayTimeoutPanel;
        [SerializeField] private CanvasGroup _dayTimeoutGroup;

        [Title("Invoked Events")]
        [SerializeField] private GameStateRequestEventSO _changeStateSO;

        [Title("Listening to Events")]
        [SerializeField] private BoolEventSO _greyOutPauseEvent;

        [Title("UniTask Events")]
        [SerializeField] private VoidAsyncEventSO _dayChangeAnimationEvent;
        [SerializeField] private VoidUniTaskEventSO _dayTimeoutEvent;
        [SerializeField] private VoidUniTaskEventSO _respawnNextDayEvent;

        // greyoutpauseevent?

        private float _dayChangeFadeIn = 1f;
        private float _dayChangeFadeOut = 1f;
        private int _dayChangeAnimation = 2;
        private float _timeoutFadeIn = 1f;
        private float _timeoutFadeOut = 1f;
        private int _timeoutAnimation = 2;

        private void OnEnable()
        {
            _greyOutPauseEvent.OnInvokeEvent += OnGreyOutPause;
            _dayChangeAnimationEvent.OnStartEvent += OnDayChange;
            _dayTimeoutEvent.OnStartEvent += OnDayTimeout;
        }
        private void OnDisable()
        {
            _greyOutPauseEvent.OnInvokeEvent -= OnGreyOutPause;
            _dayChangeAnimationEvent.OnStartEvent -= OnDayChange;
            _dayTimeoutEvent.OnStartEvent -= OnDayTimeout;
        }

        private void OnDayChange() { DayChangeAsync().Forget(); }
        private void OnDayTimeout() { DayTimeoutAsync().Forget();  }

        private void OnGreyOutPause(bool isEnabled)
        {
            // pause or unpause game
            if (isEnabled) { _changeStateSO.RequestEvent(GameStateMachine.StateType.Pause); }
            else { _changeStateSO.RequestEvent(GameStateMachine.StateType.Run); }

            // toggle greyed background panel
            _greyOutPanel.SetActive(isEnabled);
        }

        private async UniTaskVoid DayChangeAsync()
        {
            // pause game during animation
            _changeStateSO.RequestEvent(GameStateMachine.StateType.Pause);

            // activate panel
            _dayChangePanel.SetActive(true);

            // play fade in animation
            await PanelFadeInCoroutine(_dayChangeFadeIn, _dayChangeGroup);

            // end event
            _dayChangeAnimationEvent.EndEvent();

            // brief delay
            await UniTask.Delay(_dayChangeAnimation * 1000, ignoreTimeScale: true);
            // play fade out animation
            await PanelFadeOutCoroutine(_dayChangeFadeOut, _dayChangeGroup);

            // deactivate panel
            _dayChangePanel.SetActive(false);

            // unpause game
            _changeStateSO.RequestEvent(GameStateMachine.StateType.Run);


        }

        /// <summary>
        /// DayTimeout Event
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid DayTimeoutAsync()
        {
            // --- On Start ---
            // pause game during animation
            _changeStateSO.RequestEvent(GameStateMachine.StateType.Pause);
            // activate panel
            _dayTimeoutPanel.SetActive(true);

            // --- Animation ---
            // play fade in animation
            await PanelFadeInCoroutine(_timeoutFadeIn, _dayTimeoutGroup);

            // respawn player while screen is black
            _respawnNextDayEvent.StartEvent();

            await UniTask.Yield();
            // brief delay
            await UniTask.Delay(_timeoutAnimation * 1000, ignoreTimeScale: true);
            // TODO: DOES THIS MESS STUFF UP??
            //await _respawnNextDayEvent.Task;

            // play fade out animation
            await PanelFadeOutCoroutine(_timeoutFadeOut, _dayTimeoutGroup);

            // --- On End ---
            // deactivate panel
            _dayTimeoutPanel.SetActive(false);
            // unpause game
            _changeStateSO.RequestEvent(GameStateMachine.StateType.Run);
            // end event
            _dayTimeoutEvent.EndEvent();
        }

        // Coroutine to fade panel into view
        private IEnumerator PanelFadeInCoroutine(float seconds, CanvasGroup group)
        {
            // fade in
            for (float time = 0; time <= seconds; time += Time.unscaledDeltaTime)
            {
                group.alpha = Mathf.Lerp(0, 1, time / seconds);
                yield return null;
            }
        }

        // Coroutine to gradually fade panel out
        private IEnumerator PanelFadeOutCoroutine(float seconds, CanvasGroup group)
        {
            for (float time = 0; time <= seconds; time += Time.unscaledDeltaTime)
            {
                group.alpha = Mathf.Lerp(1, 0, time / seconds);
                yield return null;
            }
        }
    }
}