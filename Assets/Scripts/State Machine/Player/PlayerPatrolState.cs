using EventChannel;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.StateMachine
{
    public class PlayerPatrolState : PatrolStateBase
    {
        [Header("Player Patrol State Fields")]
        [Tooltip("Seconds to wait after player last moved before transitioning back to idle state.")]
        [SerializeField] private float _idleSeconds;

        [Header("Listening to Events")]
        [SerializeField] private BoolEventSO _onMoveSO;
        [SerializeField] private BoolEventSO _onCombatSO;

        private Timer.FixedTimer _idleTimer;
        private PlayerData _player;

        // flags
        private bool _combatFlag = false;
        private bool _deathFlag = false;

        #region Interface Methods
        public override void Init(ICharacter character)
        {
            _idleTimer = new Timer.FixedTimer(_idleSeconds);
            BaseInit(character);
        }

        private void BaseInit(ICharacter character)
        {
            Character = character;
            _player = character as PlayerData;

            Transitions.Add(new Transition(StateType.Idle, () => _idleTimer.Status == Timer.Status.Complete));
            Transitions.Add(new Transition(StateType.Combat, () => _combatFlag == true));
            Transitions.Add(new Transition(StateType.Death, () => _deathFlag == true));
        }

        public override void SetEnabled(bool enabled) { this.enabled = enabled; }

        public override void ResetState()
        {
            _idleTimer.Stop();
            _combatFlag = false;
            _deathFlag = false;
        }
        #endregion

        private void OnEnable()
        {
            _onMoveSO.OnInvokeEvent += OnMove;
            _onCombatSO.OnInvokeEvent += OnCombat;
            _player.Health.DeathSO.OnInvokeEvent += OnDeath;
        }

        private void OnDisable()
        {
            _onMoveSO.OnInvokeEvent -= OnMove;
            _onCombatSO.OnInvokeEvent -= OnCombat;
            _player.Health.DeathSO.OnInvokeEvent -= OnDeath;
            ResetState();
        }

        private void Update()
        {
            // start timer only if player isn't moving
            _idleTimer.Tick();
        }

        private void OnMove(bool isMoving)
        {
            // TODO: change to use player.velocity == 0 instead???
            if (isMoving == true)
            {
                // stop and reset timer if player starts moving
                _idleTimer.Stop();
            }
            else
            {
                if (!_idleTimer.IsRunning) { _idleTimer.Start(); }
            }
        }

        private void OnCombat(bool isInCombat) {  _combatFlag = isInCombat; }

        private void OnDeath() { _deathFlag = true; }
    }
}
