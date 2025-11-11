using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.StateMachine
{
    /// <summary>
    /// Basic implementation of Idle State
    /// </summary>
    public class IdleState : IdleStateBase
    {
        #region Interface Methods
        public override void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion
    }
}