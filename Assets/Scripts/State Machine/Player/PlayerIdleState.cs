using Sirenix.OdinInspector;
using UnityEngine;

using EventChannel;
namespace Characters.StateMachine
{
    public class PlayerIdleState : IdleStateBase
    {
        [Title("Listening to Events")]
        [SerializeField] private BoolEventSO _onMoveSO;
        [SerializeField] private BoolEventSO _onCombatSO;

        private PlayerData _player;

        // flags
        private bool _moveFlag = false;
        private bool _combatFlag = false;
        private bool _deathFlag = false;

        #region Interface Methods
        public override void Init(ICharacter character)
        {
            Character = character;
            _player = character as PlayerData;

            Transitions.Add(new Transition(StateType.Patrol, () => _moveFlag == true));
            Transitions.Add(new Transition(StateType.Combat, () => _combatFlag == true));
            Transitions.Add(new Transition(StateType.Death, () => _deathFlag == true));
        }

        public override void SetEnabled(bool enabled) { this.enabled = enabled; }

        public override void ResetState()
        {
            base.ResetState();
            _moveFlag = false;
            _combatFlag = false;
            _deathFlag = false;
        }
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            _onMoveSO.OnInvokeEvent += OnMove;
            _onCombatSO.OnInvokeEvent += OnCombat;
            _player.Health.DeathSO.OnInvokeEvent += OnDeath;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            _onMoveSO.OnInvokeEvent -= OnMove;
            _onCombatSO.OnInvokeEvent -= OnCombat;
            _player.Health.DeathSO.OnInvokeEvent -= OnDeath;
        }

        private void OnMove(bool isMoving)
        {
            if (isMoving) { _moveFlag = true; }
        }

        private void OnCombat(bool isInCombat)
        {
            if (isInCombat) { _combatFlag = true; }
        }

        private void OnDeath() { _deathFlag = true; }
    }
}