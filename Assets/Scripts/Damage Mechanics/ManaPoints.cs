using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;

using EventChannel;

namespace Characters
{
    public class ManaPoints : SerializedMonoBehaviour
    {
        // this class will just be accessed via characterdata classes, not going to use
        // events for those refs for now
        [Title("Reference")]
        [SerializeField][Required] private IManaSettings _manaSettings;

        [Title("Invoked Events")]
        [SerializeField] private Vector2EventSO _manaChangedSO;

        [Title("Player Only: Listening to Events")]
        [SerializeField] private VoidPlayerDataRequestEventSO _sendDataSO;

        // serialized for DEBUG ONLY
        [Title("Showing for Debug")]
        [ShowInInspector] public float MaxMana { get; private set; }
        [ShowInInspector] public float CurrentMana { get; private set; }

        /// <summary>
        /// The amount of mana the character recovers per second
        /// </summary>
        private float _recoveryAmount = 1f;

        private bool _shouldRecoverMana = true;

        private void Awake()
        {
            MaxMana = _manaSettings.MaxMana;
            CurrentMana = MaxMana;
        }

        private void OnEnable()
        {
            // player only
            if (_sendDataSO != null) { _sendDataSO.OnResultEvent += Init; }
        }
        private void OnDisable()
        {
            // player only
            if (_sendDataSO != null) { _sendDataSO.OnResultEvent -= Init; }
        }

        private void Start()
        {
            StartCoroutine(RecoveryCoroutine());
        }

        #region Save Data
        // player only method for initialization after loading
        private void Init(SaveData.PlayerSaveData data)
        {
            if (data != null)
            {
                CurrentMana = data.CurrentMana;
                MaxMana = data.MaxMana;
            }
        }
        #endregion

        public bool CheckCost(float mana)
        {
            return mana <= CurrentMana;
        }

        /// <summary>
        /// Add specified amount to character's current mana.
        /// </summary>
        /// <param name="amount">Amount of mana to add.</param>
        public void ChangeMana(float amount)
        {
            CurrentMana += amount;
            CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana);

            _manaChangedSO.InvokeEvent(new Vector2(CurrentMana, MaxMana));
        }


        private IEnumerator RecoveryCoroutine()
        {
            //Debug.Log("starting recovery");
            while (_shouldRecoverMana)
            {
                if (CurrentMana < MaxMana)
                {
                    ChangeMana(_recoveryAmount);
                }

                yield return new WaitForSeconds(2f);
            }
        }

        [Button]
        public void UseMana(float amount)
        {
            ChangeMana(amount);
        }
    }
}