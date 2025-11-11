using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

using Combat;
using EventChannel;

namespace Characters.StateMachine
{
    public class PlayerCombatState : CombatStateBase
    {
        [Title("Player Combat State Fields")]
        [SerializeField] private AttackLogic _attackLogic;
        [Tooltip("If no weapon is equipped, switch back to idle state after not taking damage for this many seconds.")]
        [SerializeField] protected float _combatCountdown;

        [Title("Listening to Events")]
        [SerializeField] private BoolEventSO _onCombatSO;

        private bool _isArmed;
        private PlayerData _player;

        /// <summary>
        /// Timer to track player's automatic transition out of combat state
        ///     - Enabled: !_isArmed
        ///     - Running: default
        ///     - Reset: onTakeDamage
        /// </summary>
        private Timer.FixedTimer _combatTimer;

        // flag
        private bool _deathFlag = false;

        #region Interface Methods
        public override void Init(ICharacter character)
        {
            _combatTimer ??= new Timer.FixedTimer(_combatCountdown);

            Character = character;
            _player = character as PlayerData;

            Transitions.Add(new Transition(StateType.Patrol, () => _combatTimer.Status == Timer.Status.Complete));
            Transitions.Add(new Transition(StateType.Death, () => _deathFlag == true));
        }

        public override void SetEnabled(bool enabled) { this.enabled = enabled; }

        public override void ResetState()
        {
            base.ResetState();
            _combatTimer.Stop();
            _deathFlag = false;
        }
        #endregion

        private void OnEnable()
        {
            ToggleAttack(true);
            _onCombatSO.OnInvokeEvent += OnCombat;
            _player.Health.DeathSO.OnInvokeEvent += OnDeath;

            // get initial armed status - will be corrected by event if incorrect
            _isArmed = _player.CombatManager.IsHoldingWeapon;
            _combatTimer ??= new Timer.FixedTimer(_combatCountdown);
            UpdateTimerStatus();
        }

        private void OnDisable()
        {
            ToggleAttack(false);
            _onCombatSO.OnInvokeEvent -= OnCombat;
            _player.Health.DeathSO.OnInvokeEvent -= OnDeath;

            StopAllCoroutines();
            ResetState();
        }

        public override void Update()
        {
            base.Update();

            // tick timer
            _combatTimer.Tick();
        }

        // called when player is armed/unarmed
        private void OnCombat(bool isHoldingWeapon) 
        { 
            // update if status changes
            if (_isArmed != isHoldingWeapon)
            {
                _isArmed = isHoldingWeapon;
                UpdateTimerStatus();
            }
        }

        private void OnDeath() { _deathFlag = true; }

        public override void GetHit(DamageInstance damageInstance)
        {
            // if ability stuns
            if (damageInstance.StunDuration > 0)
            {
                // ignore stagger mechanics and just get stunned
                GetStunned(damageInstance.StunDuration);
            }
            else
            {
                // if ability doesn't stun, player will get briefly staggered
                GetStunned(_player.Settings.StaggerSeconds);
            }

            // take knockback
            TakeKnockback(_player.Settings.Knockback, damageInstance.KnockbackMultiplier);

            UpdateTimerStatus();
        }

        protected void ToggleAttack(bool enabled)
        {
            _attackLogic.enabled = enabled;
        }

        protected override void GetStunned(float seconds)
        {
            ToggleAttack(false);
            base.GetStunned(seconds);
        }

        protected override void Unfreeze()
        {
            base.Unfreeze();
            ToggleAttack(true);

            UpdateTimerStatus();
        }

        private void UpdateTimerStatus()
        {
            // player will never leave combat state if they have a weapon equipped
            if (_isArmed)
            {
                _combatTimer.Stop();
                _combatTimer.Disable();
            }
            else
            {
                // enable combat coundown if player is unarmed
                _combatTimer.Enable();

                // stop timer if player is knocked back or stunned
                if (_isKnockedBack || _stunTimer.IsRunning) { _combatTimer.Stop(); }
                else
                {
                    // otherwise, start timer if it isn't already running
                    if (!_combatTimer.IsRunning) { _combatTimer.Start(); }
                }
            }
        }
    }
}
