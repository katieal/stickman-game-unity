using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

using EventChannel;
namespace Managers.GameStateMachine
{
    public class RunState : MonoBehaviour, IGameState
    {

        //[Title("Invoked Events")]
        //[SerializeField] private BoolBoolEventSO _enableInputSO;

        #region Interface Fields
        public List<Transition> Transitions { get; set; } = new();
        public StateType Type { get; } = StateType.Run;
        public void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion


        private void OnEnable()
        {
            //_enableInputSO.InvokeEvent(true, true);
        }

        private void OnDisable()
        {
            
        }

        private void Update()
        {
            
        }
    }
}