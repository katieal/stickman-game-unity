using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.StateMachine
{
    // this state makes the mob walk back to their spawn point, then go into idle state
    public abstract class ReturnStateBase : MonoBehaviour, IState
    {
        public ICharacter Character { get; protected set; }
        public List<Transition> Transitions { get; protected set; } = new();

        // State variables
        [ShowInInspector] protected Vector3 _spawn;
        // flag
        protected bool _hasReachedSpawn = false;


        #region Interface Methods
        public virtual void Init(ICharacter character)
        {
            Character = character;
            Transitions.Add(new Transition(StateType.Idle, () => _hasReachedSpawn));
        }

        public abstract void SetEnabled(bool enabled);

        public void ResetState()
        {
            _hasReachedSpawn = false;
        }
        #endregion

        public void SetSpawn(Transform point) { _spawn = point.position; }

        protected void OnEnable()
        {
            // play walking animation
            Character.Animator.SetBool("IsWalking", true);
        }
        protected void OnDisable()
        {
            // stop walking animation
            Character.Animator.SetBool("IsWalking", false);
            ResetState();
        }
    }
}