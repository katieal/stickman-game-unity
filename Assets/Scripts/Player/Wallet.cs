using Sirenix.OdinInspector;
using UnityEngine;

using EventChannel;
namespace Player
{
    public class Wallet : MonoBehaviour
    {
        [Title("Listening to Events")]
        [SerializeField] private PlayerStaticDataEventSO _loadStaticDataEvent;

        [Title("Responding to Events")]
        [SerializeField] private IntIntRequestEventSO _changeMoneySO;
        [SerializeField] private IntBoolRequestEventSO _checkBalanceEvent;

        [ShowInInspector] public int Balance { get; private set; } = 0;
        [ShowInInspector] public int Debt { get; private set; } = 0;

        #region Singleton
        private static Wallet _instance;
        public static Wallet Instance { get { return _instance; } }

        private void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        private void OnEnable()
        {
            _loadStaticDataEvent.OnInvokeEvent += OnLoadStaticData;
            _changeMoneySO.OnRequestEvent += ChangeBalance;
            _checkBalanceEvent.OnRequestEvent += OnCheckBalanceEvent;
        }
        private void OnDisable()
        {
            _loadStaticDataEvent.OnInvokeEvent -= OnLoadStaticData;
            _changeMoneySO.OnRequestEvent -= ChangeBalance;
            _checkBalanceEvent.OnRequestEvent -= OnCheckBalanceEvent;
        }

        #region Load Data
        private void OnLoadStaticData(SaveData.PlayerStaticData data)
        {
            Balance = data.PlayerMoney;
            Debt = data.PlayerDebt;
        }
        #endregion

        #region Accessors
        public bool CheckBalance(int amount) { return Balance >= amount; }
        public int GetDebt() { return Debt; }
        #endregion

        private void ChangeBalance(int amount)
        {
            Balance += amount;
            _changeMoneySO.OnResultEvent(Balance);
        }

        private void OnCheckBalanceEvent(int amount)
        {
            _checkBalanceEvent.SendResult(Balance >= amount);
        }


        [Button]
        public void AddMoney()
        {
            ChangeBalance(50);
        }
    }
}
