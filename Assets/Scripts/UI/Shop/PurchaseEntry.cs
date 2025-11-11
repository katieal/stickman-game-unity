using UnityEngine;
using TMPro;
using Items;
using UnityEngine.UI;

namespace Shop
{
    public class PurchaseEntry : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text _nameText;
        [field: SerializeField] public TMP_InputField QuantityInput { get; private set; }
        [SerializeField] private TMP_Text _priceText;
        [field: SerializeField] public Button DeleteButton { get; private set; }

        public ItemSO Item { get; private set; }
        public int Quantity { get; private set; }

        /// <summary>
        /// Maximum quantity that can be purchased in a single purchase entry
        /// </summary>
        public int MaxQuantity { get; private set; }
        public int Price { get; private set; }

        public void Init(ItemSO item, int num, int maxNum, int price)
        {
            Item = item;
            Quantity = num;
            MaxQuantity = maxNum;
            Price = price;

            _nameText.text = item.name;
            QuantityInput.text = Quantity.ToString(); ;
            _priceText.text = Price.ToString();
        }

        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
            QuantityInput.text = Quantity.ToString();
        }

        public void ChangeQuantity(int amount)
        {
            Quantity += amount;
            QuantityInput.text = Quantity.ToString();
        }

        public int CalculateTotalCost() { return Price * Quantity; }

        public void DestroySelf() 
        {
            QuantityInput.onEndEdit.RemoveAllListeners();
            DeleteButton.onClick.RemoveAllListeners();
            Destroy(gameObject); 
        }
    }
}