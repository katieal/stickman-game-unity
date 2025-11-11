using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using EventChannel;
using Scenes;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Managers.GameStateMachine
{
    /*
     * Load State freezes physics and ALL input from player while stuff is loading
     *      maybe also display a loading screen?
     */
    public class LoadState : MonoBehaviour, IGameState
    {
        [Title("Invoked Events")]
        [SerializeField] private SaveTypeVoidUniTaskEventSO _autoSaveGameEvent;
        [SerializeField] private SceneNameVoidUniTaskEventSO _loadNewSceneEvent;
        [SerializeField] private LocationVoidUniTaskEventSO _spawnSceneObjectsEvent;

        #region Interface Fields
        public List<Transition> Transitions { get; set; } = new();
        public StateType Type { get; } = StateType.Load;
        public void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion

        // loading flag
        private bool _isLoadingFinished = false;

        //private bool _isPlayerEnabled;

        private void Awake()
        {
            Transitions.Add(new Transition(StateType.Run, () => _isLoadingFinished == true));
        }

        private void OnEnable()
        {
            //Debug.Log("loading start");
            //_isPlayerEnabled = InputSystem.actions.FindActionMap("Player").enabled;
            FreezeGame();
        }
        private void OnDisable()
        {
            UnfreezeGame();
            _isLoadingFinished = false;
            //Debug.Log("loading end");
        }

        // called when in game when switching to a new scene
        public async UniTaskVoid ChangeSceneAsync(SceneName name, Location location)
        {
            /*  Order of Events
             *  - freeze game
             *  - call save manager (save game)
             *  - call scene loader (unload old scene, load new one)
             *  - call spawn manager (spawn mobs, it stores its own session data so dont need to pass save data)
             *  - unfreeze game
             */
           // _isPlayerEnabled = InputSystem.actions.FindActionMap("Player").enabled;
            _autoSaveGameEvent.StartEvent(SaveData.SaveType.SceneChange);
            await _autoSaveGameEvent.Task;

            _loadNewSceneEvent.StartEvent(name);
            await _loadNewSceneEvent.Task;

            _spawnSceneObjectsEvent.StartEvent(location);
            await _spawnSceneObjectsEvent.Task;

            _isLoadingFinished = true;
        }

        private IEnumerator BufferCoroutine(int seconds)
        {
            for (int i = 0; i <= seconds; i++)
            {
                Debug.Log($"loading timer: {i}");
                yield return new WaitForSecondsRealtime(1);
            }
        }

        #region Freeze Game
        private void FreezeGame()
        {
            // freeze time
            Time.timeScale = 0f;

            // freeze all input
            InputSystem.actions.FindActionMap("Player").Disable();
            InputSystem.actions.FindActionMap("Menu").Disable();
            InputSystem.actions.FindActionMap("UI").Disable();

            // TODO: add in a loading screen?
        }

        private void UnfreezeGame()
        {
            // unfreeze time
            Time.timeScale = 1f;

            // unfreeze all input
            InputSystem.actions.FindActionMap("Player").Enable();
            InputSystem.actions.FindActionMap("Menu").Enable();
            InputSystem.actions.FindActionMap("UI").Enable();
        }
        #endregion
    }
}