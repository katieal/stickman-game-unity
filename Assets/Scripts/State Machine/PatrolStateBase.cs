using System.Collections.Generic;
using UnityEngine;

namespace Characters.StateMachine
{
    public abstract class PatrolStateBase : MonoBehaviour, IState
    {
        public ICharacter Character { get; protected set; }
        public List<Transition> Transitions { get; protected set; } = new();

        #region Interface Methods
        public virtual void Init(ICharacter character)
        {
            Character = character;
        }

        public abstract void SetEnabled(bool enabled);
        public abstract void ResetState();
        #endregion
    }
}