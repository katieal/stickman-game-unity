using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserInterface
{
    public class OptionsController : MonoBehaviour
    {
        // buttons in options menu
        [field: Title("Components")]
        [field: SerializeField] public Button EquipButton { get; private set; }
        [field: SerializeField] public Button UnequipButton { get; private set; }
        [field: SerializeField] public Button UseButton { get; private set; }
        [field: SerializeField] public Button DropButton { get; private set; }

        [Title("Drop One Components")]
        [SerializeField] private GameObject _dropOnePanel;
        [SerializeField] private Button _dropOneButton;

        [Title("Drop Many Components")]
        [SerializeField] private GameObject _dropManyPanel;
        [SerializeField] private TMP_Text _quantityText;
        [SerializeField] private Slider _quantitySlider;
        [SerializeField] private Button _dropManyButton;

        [HideInInspector] public UnityEvent<int> OnDropItem;

        private void OnDisable()
        {
            UnsubscribeAll();

            _dropOnePanel.SetActive(false);
            _dropManyPanel.SetActive(false);
        }

        public void ShowOptions(Vector2 position, bool equip, bool unequip, bool use, bool drop)
        {
            transform.position = position;
            gameObject.SetActive(true);
            EquipButton.gameObject.SetActive(equip);
            UnequipButton.gameObject.SetActive(unequip);
            UseButton.gameObject.SetActive(use);
            DropButton.gameObject.SetActive(drop);

            _dropOnePanel.SetActive(false);
            _dropManyPanel.SetActive(false);
        }

        /// <summary>
        /// Hide options panel and unsubscribe all its listeners
        /// </summary>
        public void HideOptions()
        {
            UnsubscribeAll();

            _dropOnePanel.SetActive(false);
            _dropManyPanel.SetActive(false);
            gameObject.SetActive(false);
        }

        public void ShowDropItem(int quantity)
        {
            // show separate confirm menu if item quantity is only 1
            if (quantity <= 1) { ShowConfirmOne(); }
            else { ShowConfirmMany(quantity); }
        }

        private void ShowConfirmOne()
        {
            _dropOneButton.onClick.AddListener(() => OnDropItem.Invoke(1));
            _dropOnePanel.SetActive(true);
        }

        private void ShowConfirmMany(int quantity)
        {
            _quantitySlider.maxValue = quantity;
            _quantitySlider.onValueChanged.AddListener((value) => OnSliderChange(value));
            _dropManyButton.onClick.AddListener(() => DropMany());

            _dropManyPanel.SetActive(true);
        }

        private void OnSliderChange(float value)
        {
            _quantityText.text = ((int)value).ToString();
        }

        private void DropMany()
        {
            OnDropItem.Invoke((int)_quantitySlider.value);

            _quantitySlider.onValueChanged.RemoveAllListeners();
            _dropManyButton.onClick.RemoveAllListeners();

        }

        private void UnsubscribeAll()
        {
            EquipButton.onClick.RemoveAllListeners();
            UnequipButton.onClick.RemoveAllListeners();
            UseButton.onClick.RemoveAllListeners();
            DropButton.onClick.RemoveAllListeners();

            _dropOneButton.onClick.RemoveAllListeners();
            _quantitySlider.onValueChanged.RemoveAllListeners();
            _dropManyButton.onClick.RemoveAllListeners();
            OnDropItem.RemoveAllListeners();
        }
    }
}
