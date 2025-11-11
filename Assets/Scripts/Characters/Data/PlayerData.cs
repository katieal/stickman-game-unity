using EventChannel;
using Items;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class PlayerData : MonoBehaviour, ICharacter, IDamagable, IManaUser
    {
        #region ICharacter
        [field: Title("Character Fields")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public Transform Position { get; private set; }
        [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }

        [field: Header("Scripts")]
        [field: SerializeField] public MoveLogic MoveLogic { get; private set; }
        [field: SerializeField] public AudioController Audio { get; private set; }
        [field: SerializeField] public AnimationController Animation { get; private set; }
        #endregion

        #region IDamagable
        public float MaxHealth { get; private set; }
        public float AttackSpeed { get; private set; }
        public Vector2 Knockback { get; private set; }
        public float StaggerSeconds { get; private set; }

        [field: Title("Damagable Script")]
        [field: SerializeField] public HealthPoints Health { get; private set; }
        #endregion

        #region IManaUser
        public float MaxMana { get; private set; }

        [field: Title("Mana User Fields")]
        [field: SerializeField] public ManaPoints Mana { get; private set; }
        #endregion

        #region Player
        [field: Title("Player Fields")]
        [field: SerializeField] public PlayerSettingsSO Settings { get; private set; }
        [field: SerializeField] public Transform LaunchOrigin { get; private set; }

        [field: Header("Scripts")]
        [field: SerializeField] public StateMachine.PlayerStateMachine StateMachine { get; private set; }
        [field: SerializeField] public Combat.PlayerCombatManager CombatManager { get; private set; }
        [field: SerializeField] public Combat.PlayerAttackLogic AttackLogic { get; private set; }
        #endregion

        public PlayerAnimationController PlayerAnimation { get { return Animation as PlayerAnimationController; } }


        [Title("Invoked Events")]
        [SerializeField] private VoidPlayerDataRequestEventSO _loadPlayerDataSO;

        [Title("Listening to Events")]
        [SerializeField] private VoidPlayerSaveDataUniTaskEventSO _savePlayerDataEvent;


        private void Init()
        {
            // ICharacter
            this.MoveSpeed = Settings.MoveSpeed;

            // IDamagable
            this.MaxHealth = Settings.MaxHealth;
            this.AttackSpeed = Settings.AttackSpeed;
            this.Knockback = Settings.Knockback;
            this.StaggerSeconds = Settings.StaggerSeconds;

            // IManaUser
            this.MaxMana = Settings.MaxMana;
        }

        private void Awake()
        {
            Init();
        }
        private void OnEnable()
        {
            _savePlayerDataEvent.OnStartEvent += OnSavePlayerData;
        }
        private void OnDisable()
        {
            _savePlayerDataEvent.OnStartEvent -= OnSavePlayerData;
        }

        private void Start()
        {
            // load player save data
            _loadPlayerDataSO.OnRequestEvent();
        }

        public void GetHit(Combat.DamageInstance damageInstance)
        {
            Health.ChangeHealthSO.OnRequestEvent(-damageInstance.Damage);
            StateMachine.GetHit(damageInstance);
        }

        private void OnSavePlayerData()
        {
            SaveData.PlayerSaveData saveData = new SaveData.PlayerSaveData()
            {
                CurrentHealth = Health.CurrentHealth,
                MaxHealth = Health.MaxHealth,
                CurrentMana = Mana.CurrentMana,
                MaxMana = Mana.MaxMana,
                MoveSpeed = MoveLogic.MoveSpeed,
                HeldWeapon = CombatManager.HeldWeapon,
                SelectedAbility = CombatManager.AbilityIndex,
                Abilities = CombatManager.GetData(),
                CurrentState = StateMachine.CurrentState,
                PreviousState = StateMachine.PreviousState
            };

            _savePlayerDataEvent.EndEvent(saveData);
        }
    }
}