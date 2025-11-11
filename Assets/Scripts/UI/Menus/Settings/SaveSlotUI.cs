using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SaveData;

namespace UserInterface
{
    public class SaveSlotUI : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private bool _isManualSaveSlot;
        public Button LoadButton;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _locationText;

 
        [Title("Manual Save Slot Only Components")]
        [ShowIf("_isManualSaveSlot")][SerializeField] private TMP_Text _completionText;
        [ShowIf("_isManualSaveSlot")][SerializeField] private TMP_Text _emptyText;
        [ShowIf("_isManualSaveSlot")] public Button SaveButton;
        [ShowIf("_isManualSaveSlot")] public Button DeleteButton;

        public bool IsEmpty { get; private set; }

        public void SetEnabled(bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
        }

        public void SetDetails(Sprite image, SaveSlotDisplayData data)
        {
            IsEmpty = false;

            _image.sprite = image;
            _dayText.gameObject.SetActive(true);
            _dayText.text = $"Day {data.Day}";
            _timeText.gameObject.SetActive(true);
            _timeText.text = data.Time;
            _locationText.gameObject.SetActive(true);
            _locationText.text = data.Location;

            // if this is a manual save slot
            if (_isManualSaveSlot)
            {
                _completionText.gameObject.SetActive(true);
                _completionText.text = $"{data.Completion:F2}%";
                _emptyText.gameObject.SetActive(false);
                // ensure buttons are enabled
                LoadButton.interactable = true;
                DeleteButton.interactable = true;
            }
        }

        public void SetEmpty(Sprite image)
        {
            // this method is only used on manual save slots - unused autosave slots are simply disabled
            if (!_isManualSaveSlot) { return; }

            // set slot to empty
            IsEmpty = true;

            // show empty save slot
            _image.sprite = image;
            _dayText.gameObject.SetActive(false);
            _timeText.gameObject.SetActive(false);
            _completionText.gameObject.SetActive(false);
            _locationText.gameObject.SetActive(false);
            _emptyText.gameObject.SetActive(true);

            // disable buttons
            LoadButton.interactable = false;
            DeleteButton.interactable = false;
        }

        public void RemoveCallbacks()
        {
            LoadButton.onClick.RemoveAllListeners();
            if (_isManualSaveSlot)
            {
                SaveButton.onClick.RemoveAllListeners();
                DeleteButton.onClick.RemoveAllListeners();
            }
        }
    }
}