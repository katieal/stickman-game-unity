using EventChannel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.StateMachine
{
    /// <summary>
    /// Base Combat State Functions:
    ///     - taking damage
    ///     - stun/stagger
    ///     - knockback
    /// </summary>
    public abstract class CombatStateBase : MonoBehaviour, IState
    {
        public ICharacter Character { get; protected set; }
        public List<Transition> Transitions { get; protected set; } = new();

        protected bool _isKnockedBack = false;

        protected Coroutine _kbCoroutine;

        protected Timer.FlexibleTimer _stunTimer;

        #region Interface Methods
        public virtual void Init(ICharacter character)
        {
            Character = character;
        }
        public abstract void SetEnabled(bool enabled);
        #endregion

        public virtual void Awake()
        {
            _stunTimer = new Timer.FlexibleTimer();
        }

        public virtual void Update()
        {
            _stunTimer?.Tick();
        }

        /// <summary>
        /// GetHit() handles the stagger/stun mechanics of combat.
        /// Upon getting hit, the entity will take knockback equal to its base knockback multiplied
        /// by the ability's KB multiplier, stopping after (0.1) seconds + (0.1) second stun.
        /// The entity will be stunned for the ability's stun seconds, or staggered for the 
        /// entity's stagger seconds.
        /// If the entity can't be staggered by damage, it will retain its ability to attack.
        /// The knockback and stun timers run concurrently.
        /// </summary>
        /// <param name="damageInstance"></param>
        public abstract void GetHit(Combat.DamageInstance damageInstance);


        protected void TakeKnockback(Vector2 baseKB, float multiplier)
        {
            // freeze movement
            Character.MoveLogic.Freeze(true);

            // check if character is already taking knockback
            if (_isKnockedBack)
            {
                // if so, stop previous coroutine
                StopCoroutine(_kbCoroutine);
                // reset character velocity
                Character.RigidBody.linearVelocity = Vector2.zero;
            }

            _isKnockedBack = true;
            // take knockback
            Character.RigidBody.linearVelocity = baseKB * multiplier;
            // character travels at the KB velocity for 0.1 seconds
            _kbCoroutine = StartCoroutine(KnockbackTimer());
        }

        protected virtual void GetStunned(float seconds)
        {
            // freeze movement
            Character.MoveLogic.Freeze(true);

            // check if character is already stunned
            if (_stunTimer.IsRunning)
            {
                // if character is already stunned, stunTimer duration will either be set to the new
                // duration (seconds) or keep its original value, whichever is higher
                if (_stunTimer.Duration < seconds) { _stunTimer.SetDuration(seconds); }
            }
            else
            {
                Character.Animator.SetBool("IsStunned", true);
                _stunTimer.StartTimer(seconds);
                _stunTimer.OnTimerFinished += OnStunFinished;
            }
        }

        protected virtual void Unfreeze()
        {
            Character.Animator.SetBool("IsStunned", false);
            Character.MoveLogic.Freeze(false);
        }

        protected IEnumerator KnockbackTimer()
        {
            // ALL knockback expires after 0.1 seconds!!
            yield return new WaitForSeconds(0.1f);
            Character.RigidBody.linearVelocity = Vector2.zero;
            // brief stun after movement stops
            yield return new WaitForSeconds(0.1f);
            _isKnockedBack = false;

            // unfreeze if mob isn't also stunned
            if (!_stunTimer.IsRunning) { Unfreeze(); }
        }

        protected void OnStunFinished()
        {
            _stunTimer.OnTimerFinished -= OnStunFinished;

            // unstun if mob isn't also being knocked back
            if (!_isKnockedBack) { Unfreeze(); }
        }

        public virtual void ResetState()
        {
            _isKnockedBack = false;
            _kbCoroutine = null;
            _stunTimer.Stop();
        }
    }
}