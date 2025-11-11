using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Characters
{

    public class MobData : MonoBehaviour, ICharacter, IDamagable
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

        #region Mob
        [field: Title("Mob Fields")]
        [field: SerializeField] public MobSettingsSO Settings { get; private set; }
        [field: SerializeField] private TMP_Text _healthText;

        [field: Header("Scripts")]
        [field: SerializeField] public StateMachine.MobStateMachine StateMachine { get; private set; }
        [field: SerializeField] public Combat.DetectionLogic DetectionLogic { get; private set; }
        [field: SerializeField] public Combat.EnemyAttackLogic AttackLogic { get; private set; }

        [ShowInInspector] public List<Items.ItemInstance> LootDrops { get; private set; }
        #endregion

        public UnityAction OnDeath; // TODO -- delete this???


        private void Init()
        {
            // ICharacter
            this.MoveSpeed = Settings.MoveSpeed;

            // IDamagable
            this.MaxHealth = Settings.MaxHealth;
            this.AttackSpeed = Settings.AttackSpeed;
            this.Knockback = Settings.Knockback;
            this.StaggerSeconds = Settings.StaggerSeconds;

            // calculate mob's loot drops
            LootDrops = Settings.LootTable.CalculateLoot();
        }

        private void Awake()
        {
            Init();
        }
        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            Health.ChangeHealthSO.OnResultEvent -= UpdateHealthUI;
        }

        private void Start()
        {
            // doesn't work in onenable
            Health.ChangeHealthSO.OnResultEvent += UpdateHealthUI;
            if (_healthText != null) _healthText.text = Settings.MaxHealth.ToString() + " / " + Settings.MaxHealth.ToString();
        }

        public void UpdateHealthUI(Vector2 health)
        {
            if (_healthText != null) { _healthText.text = health.x.ToString() + " / " + health.y.ToString(); }
        }

        public void GetHit(Combat.DamageInstance damageInstance)
        {
            Health.ChangeHealthSO.OnRequestEvent(-damageInstance.Damage);
            StateMachine.GetHit(damageInstance);
        }


        [Button]
        public void PrintLoot()
        {
            Debug.Log("size: " + LootDrops.Count);
            string lootString = "Loot: ";

            for (int i = 0; i < LootDrops.Count; i++)
            {
                lootString += new string("\n\t" + LootDrops[i].DisplayName + " X" + LootDrops[i].Quantity.ToString());
            }

            Debug.Log(lootString);
        }
    }
}
