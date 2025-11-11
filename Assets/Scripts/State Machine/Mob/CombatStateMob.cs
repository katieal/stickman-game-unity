using UnityEngine;
using Sirenix.OdinInspector;

using Combat;


namespace Characters.StateMachine
{
    /// <summary>
    /// Aggressive mobs need attacklogic and detectionlogic and actively pursuit enemies.
    /// Normal mobs need attacklogic and detectionlogic and attack when an enemy is in range.
    /// Passive mobs don't attack.
    /// </summary>
    public enum MobType { Aggressive, Normal, Passive }

    public class CombatStateMob : CombatStateBase
    {

        [Title("Mob Combat State Fields")]
        [SerializeField] [EnumToggleButtons] private MobType _mobType;

        // Aggresive
        //[EnableIf("_mobType", Value = MobType.Aggressive)]
        //[SerializeField] private DetectionLogic _detectionLogic;

        [EnableIf("_mobType", Value = MobType.Aggressive)]
        [Tooltip("Mob will continue pursiung an enemy after losing LOS for this many seconds. " +
            "Resets upon seeing enemy or taking damage.")]
        [SerializeField] private float _searchSeconds;

        // Normal
        //[EnableIf("@this._mobType == MobType.Aggressive || this._mobType == MobType.Normal")]
        //[SerializeField] private EnemyAttackLogic _attackLogic;

        // Passive
        [EnableIf("@this._mobType == MobType.Normal || this._mobType == MobType.Passive")]
        [Tooltip("If mob doesn't pursue enemies, wait this many seconds after last taken damage before switching to return state.")]
        [SerializeField] private float _combatCountdown;


        private MobData _mob;
        private DetectionLogic _detectLogic;
        private EnemyAttackLogic _attackLogic;

        /// <summary>
        /// For mobs that track enemies, timer tracks _searchSeconds.
        /// If mob doesn't track enemies, timer tracks _combatCountdown.
        /// </summary>
        private Timer.FixedTimer _combatTimer;

        /// <summary>
        /// Flag indicating if detection logic detects an enemy.
        /// If _shouldTrackEnemy == false, this bool is always false.
        /// </summary>
        [Title("Debug")] [ShowInInspector] private bool _isEnemyDetected = false;

        private Vector2 _enemyPos;
        private bool _isEnemyFound = false;

        #region Interface Methods
        public override void Init(ICharacter character)
        {
            Character = character;
            _mob = character as MobData;
            if (_mobType == MobType.Aggressive) 
            { 
                _combatTimer = new(_searchSeconds);
                _detectLogic = _mob.DetectionLogic;
            }
            else
            {
                _combatTimer = new(_combatCountdown);
            }

            if (_mobType == MobType.Aggressive || _mobType == MobType.Normal) { _attackLogic = _mob.AttackLogic; }

            Transitions.Add(new Transition(StateType.Return, () => _combatTimer.Status == Timer.Status.Complete));
        }

        public override void SetEnabled(bool enabled) { this.enabled = enabled; }

        public override void ResetState()
        {
            base.ResetState();
            _combatTimer.Stop();
            _isEnemyDetected = false;
        }
        #endregion

        private void OnEnable()
        {
            ToggleAttack(true);

            // DO NORMAL MOBS FIRST
            if (_mobType == MobType.Normal)
            {
                // enable detection if necessary
                _detectLogic.enabled = true;

                _detectLogic.EnemyFoundEvent += OnEnemyFound;
                _detectLogic.EnemyLostEvent += OnEnemyLost;
                _detectLogic.EnemyPositionEvent += OnEnemyPosition;


                // if no current target (when this state is activated upon taking damage), search for target
                if (_detectLogic.Target == null)
                {
                    OnEnemyLost();
                }
                else
                {
                    OnEnemyFound();
                }
            }


            // enable detection and tracking for aggressive mobs
            //if (_mobType == MobType.Aggressive)
            //{
            //    // enable detection if necessary
            //    _detectLogic.enabled = true;
            //    // if no current target (when this state is activated upon taking damage), search for target
            //    if (_detectLogic.Target == null)
            //    {
            //        _detectLogic.EnemyFoundEvent += OnEnemyFound;
            //        _detectLogic.StartSearching();
            //    }
            //    else
            //    {
            //        _detectLogic.EnemyLostEvent += OnEnemyLost;
            //        // start tracking enemy position
            //        _detectLogic.EnemyPositionEvent += OnEnemyPosition;
            //        _detectLogic.StartTracking();
            //    }
            //}
            //else
            //{
            //    // for other mob types, start regular combat timer
            //    _combatTimer.Start();
            //}

            // determine which ability to use
            // use mob pos to determine if ability can execute
            // if yes, execute ability
            // if no, (aggro only) call movelogic to move mob to correct position
        }

        private void OnDisable()
        {
            ToggleAttack(false);
            if (_mobType == MobType.Aggressive) 
            {
                _detectLogic.EnemyFoundEvent -= OnEnemyFound;
                _detectLogic.EnemyLostEvent -= OnEnemyLost;
                _detectLogic.EnemyPositionEvent -= OnEnemyPosition; 
            }
            ResetState();
        }

        public override void Update()
        {
            base.Update();
            // if timer finishes without mob regaining LOS or taking damage, switch to return state
            _combatTimer.Tick();


            if (_isEnemyFound)
            {
                // if attacklogic is ready to attack
                // select an ability if needed
                if (_attackLogic.CurrentAbility.Trigger == TriggerType.Default)
                {
                    // check if enemy is in default attack range
                }
                else if (_attackLogic.CurrentAbility.Trigger == TriggerType.AbilityRange)
                {
                    // check if enemy is in ability range
                }
                else
                {
                    // execute ability if it is ready
                }
            }
        }

        private void OnEnemyFound() 
        {
            _isEnemyFound = true;
            _detectLogic.StartTracking();
        }
        private void OnEnemyLost() 
        {
            _isEnemyFound = false;
            _detectLogic.StartSearching();
        }
        private void OnEnemyPosition(Vector2 pos) { _enemyPos = pos; }

        private void TryAttack()
        {

        }



        protected void ToggleAttack(bool enabled)
        {
            // only function if mob can attack
            if (_mobType == MobType.Aggressive || _mobType == MobType.Normal)
            {
                _attackLogic.enabled = enabled;
            }
        }

        #region Inherited Methods
        public override void GetHit(DamageInstance damageInstance)
        {
            // stop and reset timer upon taking damage
            _combatTimer.Stop();

            if (damageInstance.StunDuration > 0)
            {
                // ignore stagger mechanics and just get stunned
                GetStunned(damageInstance.StunDuration);
            }
            else
            {
                // if ability doesn't stun, take knockback with a stagger
                if (_mob.Settings.IsStaggeredWhenHit)
                {
                    GetStunned(_mob.Settings.StaggerSeconds); 
                }
            }

            // take knockback
            TakeKnockback(_mob.Settings.Knockback, damageInstance.KnockbackMultiplier);
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

            // if not currently pursuing an enemy, start search timer
            if (!_isEnemyDetected) { _combatTimer?.Restart(); }
        }

        #endregion

        private void OnEnemyDetected(Vector2? pos)
        {
            // if enemy not found
            if (pos == null)
            {
                if (_isEnemyDetected == true)
                {
                    // start enemy not found countdown
                    _isEnemyDetected = false;
                    _combatTimer?.Start();
                }
            }
            else
            {
                // if target is new
                if (_isEnemyDetected == false)
                {
                    _isEnemyDetected = true;
                    // stop and reset enemy not found countdown
                    _combatTimer?.Stop();
                }

                // if both is able to and needs to move closer to target
                if (!_isKnockedBack && !_stunTimer.IsRunning && !_attackLogic.HasEnemyInRange)
                {
                    // TODO: set variance to be related to radius of attack range!
                    _mob.MoveLogic.MoveToPoint((Vector2)pos, 0.2f);
                }
            }
        }
    }
}