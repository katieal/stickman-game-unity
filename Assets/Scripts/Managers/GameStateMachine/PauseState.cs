using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using EventChannel;

namespace Managers.GameStateMachine
{
    /*
     * Pause state freezes character movement and game physics but still allows
     * player to interact with UI elements (i.e. inventory and menus)
     * 
     */
    public class PauseState : MonoBehaviour, IGameState
    {

        //[Title("Invoked Events")]
        //[SerializeField] private BoolBoolEventSO _enableInputSO;

        #region Interface Fields
        public List<Transition> Transitions { get; set; } = new();
        public StateType Type { get; } = StateType.Pause;
        public void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion

        private void OnEnable()
        {
            PauseGame();
        }

        private void OnDisable()
        {
            UnPauseGame();
        }

        #region Pause Game
        private void PauseGame()
        {
            // freeze time
            Time.timeScale = 0f;

            // enable menu and ui actions and disable player actions 
            InputSystem.actions.FindActionMap("Player").Disable();
            InputSystem.actions.FindActionMap("Menu").Enable();
            InputSystem.actions.FindActionMap("UI").Enable();
        }
        private void UnPauseGame()
        {
            // unfreeze time
            Time.timeScale = 1f;

            // re enable player actions
            InputSystem.actions.FindActionMap("Player").Enable();
        }
        #endregion
    }
}