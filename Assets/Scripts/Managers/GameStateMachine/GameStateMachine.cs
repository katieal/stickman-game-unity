using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

using EventChannel;

namespace Managers.GameStateMachine
{
    // state machine for the game as a whole
    // also manages timescale?
    public class GameStateMachine : SerializedMonoBehaviour
    {
        // not sure if I want to implement this yet.....
        //[Header("Global time variables")]
        //public static float LocalTimeScale { get; private set; } = 1f;
        //public static float DeltaTime { get { return Time.deltaTime * LocalTimeScale; } }
        //public static float TimeScale { get { return Time.timeScale * LocalTimeScale; } }
        //public static bool IsPaused { get { return LocalTimeScale == 0f; } }

#if UNITY_EDITOR
        [Title("Debug Var Only")]
        [SerializeField] private StateType StartingGameState;
#endif

        [Title("Listening to Events")]
        [SerializeField] private SceneNameLocationEventSO _changeSceneEvent;


        [Title("Responding to Events")]
        [SerializeField] private GameStateRequestEventSO _changeStateSO;

        // state machine fields
        [SerializeField]
        private Dictionary<StateType, IGameState> _gameStates = new Dictionary<StateType, IGameState>()
        {
            { StateType.Menu, null },
            { StateType.Run, null },
            { StateType.Pause, null },
            { StateType.Load, null },
            { StateType.Start, null },
            { StateType.Exit, null },
        };
        private StateType _cState;

        private void Init()
        {
            foreach(StateType type in _gameStates.Keys)
            {
                _gameStates[type]?.SetEnabled(false);
            }
        }

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            _changeStateSO.OnRequestEvent += ChangeState;
            _changeSceneEvent.OnInvokeEvent += OnChangeScene;
        }
        private void OnDisable()
        {
            _changeStateSO.OnRequestEvent -= ChangeState;
            _changeSceneEvent.OnInvokeEvent -= OnChangeScene;
        }

        private void Start()
        {
            // enable start state
            _cState = StateType.Start;

#if UNITY_EDITOR
            _cState = StartingGameState;
#endif

            _gameStates[_cState].SetEnabled(true);

            // broadcast changed state
            _changeStateSO.OnResultEvent(_cState);
        }

        private void Update()
        {
            foreach (Transition transition in _gameStates[_cState].Transitions)
            {
                if (transition.Condition()) { ChangeState(transition.NewState); }
            }
        }

        private void ChangeState(StateType newState)
        {
            if (_cState == newState) { return; } 

            // exit current state
            _gameStates[_cState].SetEnabled(false);

            // enable new state
            _cState = newState;
            _gameStates[_cState].SetEnabled(true);

            // broadcast changed state
            _changeStateSO.OnResultEvent(_cState);
        }

        #region Callbacks
        // called to change scenes
        private void OnChangeScene(Scenes.SceneName sceneName, Scenes.Location spanwLocation)
        {
            ChangeState(StateType.Load);
            (_gameStates[StateType.Load] as LoadState).ChangeSceneAsync(sceneName, spanwLocation).Forget();
        }


        #endregion
    }
}