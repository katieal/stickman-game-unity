using UnityEngine;
using UnityEngine.UI;

using EventChannel;

namespace Shop
{
    public class ShopMenuController : MonoBehaviour
    {
        [Header("General Components")]
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _shopPanel;
        [SerializeField] private GameObject _sellPanel;
        [SerializeField] private GameObject _optionsMenu;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button _exitButton;

        [Header("Invoked Events")]
        [SerializeField] private BoolEventSO _greyOutPauseEvent;
        [Header("Listening to Events")]
        [SerializeField] private VoidEventSO _cancelInputSO;

        private bool _isActive = false;

        private void Awake()
        {
            _mainMenu.SetActive(false);
            _optionsMenu.SetActive(false);

            _buyButton.onClick.AddListener(() => ShowShop());
            _sellButton.onClick.AddListener(() => ShowSell());
            _exitButton.onClick.AddListener(() => CloseStoreMenu());
        }

        private void OnEnable()
        {
            _cancelInputSO.OnInvokeEvent += CloseStoreMenu;
        }
        private void OnDisable()
        {
            _cancelInputSO.OnInvokeEvent -= CloseStoreMenu;
        }

        public void OpenStoreMenu()
        {
            _mainMenu.SetActive(true);
            _optionsMenu.SetActive(true);
            // shop menu displayed by default
            ShowShop();
            _isActive = true;

            // pause game while shop is open
            _greyOutPauseEvent.InvokeEvent(true);
        }

        // needs to be public for button callback
        public void CloseStoreMenu()
        {
            if (_isActive)
            {
                _mainMenu.SetActive(false);
                _optionsMenu.SetActive(false);
                _isActive = false;

                // unpause game
                _greyOutPauseEvent.InvokeEvent(false);
            }
        }

        private void ShowShop()
        {
            _shopPanel.SetActive(true);
            _sellPanel.SetActive(false);
            _buyButton.interactable = false;
            _buyButton.interactable = true;
        }

        private void ShowSell()
        {
            _shopPanel.SetActive(false);
            _sellPanel.SetActive(true);
            _buyButton.interactable = true;
            _buyButton.interactable = false;
        }
    }
}
