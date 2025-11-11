using EventChannel;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Characters.StateMachine
{
    public abstract class IdleStateBase : MonoBehaviour, IState
    {
        [Title("Idle State Fields")]
        [Tooltip("Does this entity have an idle animation?")]
        [SerializeField] protected bool _hasIdleAnimation;
        [Tooltip("The name of the trigger for the idle animation.")]
        [EnableIf("_hasIdleAnimation")][SerializeField] protected string _animationTrigger;
        [Tooltip("After entering state, seconds to wait before playing the first idle animation.")]
        [EnableIf("_hasIdleAnimation")][SerializeField] protected float _initialAnimationTimer;
        [Tooltip("The max number of seconds to wait in between every idle animation.")]
        [EnableIf("_hasIdleAnimation")][SerializeField] protected float _maxAnimationTimer;
        [Tooltip("Will this entity automatically enter the patrol state?")]
        [SerializeField] protected bool _canAutoPatrol = true;
        [Tooltip("Seconds to wait before changing to patrol state.")]
        [EnableIf("_canAutoPatrol")][SerializeField] protected float _patrolTimer;

        public ICharacter Character { get; protected set; }
        public List<Transition> Transitions { get; protected set; } = new();

        // state flags
        protected bool _animationFlag = false;
        protected bool _patrolFlag = false;

        #region Interface Methods
        public virtual void Init(ICharacter character)
        {
            Character = character;
            Transitions.Add(new Transition(StateType.Patrol, () => _patrolFlag == true));
        }
        public abstract void SetEnabled(bool enabled);

        public virtual void ResetState()
        {
            _animationFlag = false;
            _patrolFlag = false;
        }
        #endregion


        protected virtual void OnEnable()
        {
            Character.RigidBody.linearVelocity = Vector2.zero;
            if (_hasIdleAnimation) { StartCoroutine(AnimationCooldown(_initialAnimationTimer)); }
            if (_canAutoPatrol) { StartCoroutine(PatrolCooldown()); }
        }

        private void Update()
        {
            // play idle animation
            if (_animationFlag)
            {
                _animationFlag = false;
                Character.Animator.SetTrigger(_animationTrigger);
                StartCoroutine(AnimationCooldown(UnityEngine.Random.Range(1, _maxAnimationTimer)));
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            ResetState();
        }

        protected IEnumerator AnimationCooldown(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _animationFlag = true;
        }

        protected IEnumerator PatrolCooldown()
        {
            yield return new WaitForSeconds(_patrolTimer);
            _patrolFlag = true;
        }


    }
}