using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


namespace Characters
{
    /// <summary>
    /// This class will be for trainers only - basically Npcs that can also do combat
    /// Combat related stuff is commented out for now
    ///     ********** TODO: ADD IN COMBAT CAPABILITIES!!!!!!!!!!!! **********
    /// </summary>
    public class TrainerData : MonoBehaviour, ICharacter, INpc, IDamagable
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

        #region INPC
        [field: Title("NPC Fields")]
        [field: SerializeField] public NpcName Name { get; private set; }

        public string Schedule;
        #endregion

        #region Trainer
        [field: Title("Trainer Fields")]
        [field: SerializeField] public TrainerSettingsSO Settings { get; private set; }

        //[ShowInInspector] public List<ItemData.ItemInstance> LootDrops { get; private set; }
        #endregion



        private void Init()
        {
            // ICharacter
            this.MoveSpeed = Settings.MoveSpeed;

            // IDamagable
            this.MaxHealth = Settings.MaxHealth;
            this.AttackSpeed = Settings.AttackSpeed;
            this.Knockback = Settings.Knockback;
            this.StaggerSeconds = Settings.StaggerSeconds;

            // INPC
            //this.Name = Settings.Name; // TODO: IMPLEMENT

            // calculate loot drops
            //LootDrops = Settings.LootTable.CalculateLoot();
        }

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {

        }

        public void GetHit(Combat.DamageInstance damageInstance)
        {
            //Health.ChangeHealthSO.OnRequestEvent(-damageInstance.Damage);
            //StateMachine.GetHit(damageInstance);
        }
    }
}