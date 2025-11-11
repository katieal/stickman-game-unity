using UnityEngine;

namespace Characters.StateMachine
{
    public class PlayerDeathState : DeathStateBase
    {

        #region Interface Methods

        public override void SetEnabled(bool enabled) { this.enabled = enabled; }

        #endregion


        private void OnEnable()
        {
            Debug.Log("Player died!");
        }
    }
}