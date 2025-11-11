using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

using EventChannel;

namespace Characters
{
    public class HealthPoints : SerializedMonoBehaviour
    {
        /* 
         * this class will hold all armor values 
         * for damage calculations
         * 
         * physical armor
         * magic armor
         * pierce armor? (for arrows?)
         * 
         * 
         * 
        */


        [field: Title("Pre-assigned for player only: Invoked Events")]
        [field: SerializeField] public FloatVector2RequestEventSO ChangeHealthSO { get; private set; }
        [field: SerializeField] public VoidEventSO DeathSO { get; private set; }

        [Title("Player Only: Listening to Events")]
        [SerializeField] private VoidPlayerDataRequestEventSO _sendDataSO;

        // health values
        // serialized for DEBUG ONLY
        [Title("show in inspector for Debug")]
        [ShowInInspector] public float MaxHealth { get; private set; }
        [ShowInInspector] public float CurrentHealth { get; private set; }

        private AnimationController _animation;

        private void Awake()
        {
            _animation = this.GetComponentInParent<ICharacter>().Animation;

            // mob only, won't have these assigned in inspector
            if (DeathSO == null)
            {
                DeathSO = ScriptableObject.CreateInstance<VoidEventSO>();
                ChangeHealthSO = ScriptableObject.CreateInstance<FloatVector2RequestEventSO>();
            }
        }

        private void OnEnable()
        {
            ChangeHealthSO.OnRequestEvent += ChangeHealth;

            // player only
            if (_sendDataSO != null) { _sendDataSO.OnResultEvent += Init; }
        }
        private void OnDisable()
        {
            ChangeHealthSO.OnRequestEvent -= ChangeHealth;

            // player only
            if (_sendDataSO != null) { _sendDataSO.OnResultEvent -= Init; }
        }

        private void Start()
        {
            // if stats have not been initialized
            if (MaxHealth == 0)
            {
                IDamagable damagable = this.GetComponentInParent<IDamagable>();
                MaxHealth = damagable.MaxHealth;
                CurrentHealth = damagable.MaxHealth;
            }
        }

        #region Save Data
        // player only method for initialization after loading
        private void Init(SaveData.PlayerSaveData data)
        {
            if (data != null)
            {
                CurrentHealth = data.CurrentHealth;
                MaxHealth = data.MaxHealth;
            }
        }
        #endregion

        private void ChangeHealth(float amount)
        {
            //Debug.Log("taking " + amount.ToString() + " damage!");
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

            // notify listeners of new health
            ChangeHealthSO.OnResultEvent(new Vector2(CurrentHealth, MaxHealth));


            if (CurrentHealth <= 0) { DeathSO.InvokeEvent(); }
            else if (amount < 0)
            {
                // if health is being subtracted and entity is not dead, play take damage animation
                _animation.TakeDamage();
            }
        }
    }
}