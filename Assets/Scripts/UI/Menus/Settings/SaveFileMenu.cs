using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SaveData;
using EventChannel;
using Cysharp.Threading.Tasks;
using TMPro;

namespace UserInterface
{
    public class SaveFileMenu : MonoBehaviour
    {
        [Title("Refs")]
        public Managers.SaveManager SaveManager;

        [Title("General Components")]
        [SerializeField] private Sprite _activeSlotSprite;
        [SerializeField] private Sprite _emptySlotSprite;

        [Title("Autosave Components")]
        [SerializeField] private GameObject _autoSavesPanel;
        [SerializeField] private Image _autoSaveDropdownImage;
        [SerializeField] private Button _autoSaveButton;

        [Title("Slots")]
        [SerializeField] private SaveSlotUI[] _autoSaveSlots = new SaveSlotUI[4];
        [SerializeField] private SaveSlotUI[] _manualSaveSlots = new SaveSlotUI[4];

        [Title("Confirmation Popup")]
        [SerializeField] private GameObject _popupPanel;
        [SerializeField] private TMP_Text _popupText;
        [SerializeField] private Button _popupConfirmButton;
        [SerializeField] private Button _popupCancelButton;

        [Title("Invoked Events")]
        [SerializeField] private IntSaveDisplayDataUniTaskEventSO _saveGameManualEvent;
        [SerializeField] private IntEventSO _loadAutoSaveEvent;
        [SerializeField] private IntEventSO _loadManualSaveEvent;
        [SerializeField] private IntEventSO _deleteSaveManualEvent;

        [ShowInInspector] private bool _showingAutoSaves = false;

        private readonly string _manualOverwriteMsg = "Are you sure you want to replace this save file?";
        private readonly string _loadFileMsg = "Are you sure? Any current unsaved progress will be lost.";
        private readonly string _deleteFileMsg = "Are you sure you want to delete this file? This action cannot be undone.";

        // debug only
        public SaveSlotDisplayData[] AutoSaveData;
        public SaveSlotDisplayData[] ManualSaveData;

        private void OnEnable()
        {
            _autoSaveButton.onClick.AddListener(() => OnAutoSaveMenu());
            _popupPanel.SetActive(false);
            SetSlots();
        }
        private void OnDisable()
        {
            _autoSaveButton.onClick.RemoveAllListeners();

            ResetSlots();
        }

        private void SetSlots()
        {
            SaveManager.GetSaveDisplayData(out SaveSlotDisplayData[] autoSaveData, out SaveSlotDisplayData[] manualSaveData);

            AutoSaveData = autoSaveData;
            ManualSaveData = manualSaveData;

            // disable the panel if there is no autosave data
            if (IsDataEmpty(autoSaveData))
            {
                _autoSavesPanel.SetActive(false);
            }
            else
            {
                _autoSavesPanel.SetActive(true);
                // display info for autosaves
                for (int index = 0; index < autoSaveData.Length; index++)
                {
                    // closure variable
                    int i = index;
                    // disable slot if it is empty and enable it otherwise
                    _autoSaveSlots[i].SetEnabled(!autoSaveData[i].IsEmpty);
                    // show data if it is not empty
                    if (!autoSaveData[i].IsEmpty) { _autoSaveSlots[i].SetDetails(_activeSlotSprite, autoSaveData[i]); }
                    // set button callback
                    _autoSaveSlots[i].LoadButton.onClick.AddListener(() => OnLoadAutoSaveAsync(i).Forget());
                }
            }

            // display info for manual saves
            for (int index = 0; index < manualSaveData.Length; index++)
            {
                // closure variable
                int i = index;
                // show info or empty slot
                if (manualSaveData[i].IsEmpty) { _manualSaveSlots[i].SetEmpty(_emptySlotSprite); }
                else { _manualSaveSlots[i].SetDetails(_activeSlotSprite, manualSaveData[i]); }
                // set button callbacks
                _manualSaveSlots[i].SaveButton.onClick.AddListener(() => OnManualSaveGameAsync(i).Forget());
                _manualSaveSlots[i].LoadButton.onClick.AddListener(() => OnLoadManualSaveAsync(i).Forget());
                _manualSaveSlots[i].DeleteButton.onClick.AddListener(() => OnDeleteManualSaveAsync(i).Forget());
            }
        }

        private bool IsDataEmpty(SaveSlotDisplayData[] displayData)
        {
            foreach (SaveSlotDisplayData data in displayData)
            {
                if (!data.IsEmpty) { return false; }
            }
            return true;
        }

        private void ResetSlots()
        {
            foreach (var slot in _autoSaveSlots) { slot.RemoveCallbacks(); }
            foreach (var slot in _manualSaveSlots) { slot.RemoveCallbacks(); }
        }

        private void OnAutoSaveMenu()
        {

            if (_showingAutoSaves)
            {
                Debug.Log("hiding autosave menu");
                _showingAutoSaves = false;
                _autoSaveDropdownImage.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
            else
            {
                Debug.Log("showing autosave menu");
                _showingAutoSaves = true;
                _autoSaveDropdownImage.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        // save game in one of the manual save slots
        private async UniTaskVoid OnManualSaveGameAsync(int index)
        {
            // if slot isn't empty, send popup to confirm overwrite
            if (!_manualSaveSlots[index].IsEmpty)
            {
                bool response = await SendPopup(_manualOverwriteMsg);

                // return if user cancels operation
                if (!response) { return; }
            }

            // invoke manual save event and get display data
            SaveSlotDisplayData displayData = await _saveGameManualEvent.InvokeEventAsync(index);
            // show new file details
            _manualSaveSlots[index].SetDetails(_activeSlotSprite, displayData);
        }


        private async UniTaskVoid OnLoadAutoSaveAsync(int index)
        {
            Debug.Log($"loading autosave in slot {index}");

            // show confirmation window
            bool response = await SendPopup(_loadFileMsg);
            // return if user cancels operation
            if (!response) { return; }

            // load file in save manager
            _loadAutoSaveEvent.InvokeEvent(index);
        }


        private async UniTaskVoid OnLoadManualSaveAsync(int index)
        {
            Debug.Log($"loading manual save in slot {index}");

            // show confirmation window
            bool response = await SendPopup(_loadFileMsg);
            // return if user cancels operation
            if (!response) { return; }

            // load file in save manager
            _loadManualSaveEvent.InvokeEvent(index);
        }

        private async UniTaskVoid OnDeleteManualSaveAsync(int index)
        {
            Debug.Log($"Deleting manual save in slot {index}");

            // show confirmation window
            bool response = await SendPopup(_deleteFileMsg);
            // return if user cancels operation
            if (!response) { return; }

            _deleteSaveManualEvent.InvokeEvent(index);
        }

        // show a popup with a message and options to confirm or cancel
        private UniTask<bool> SendPopup(string message)
        {
            UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();

            void OnResponseButton(bool response)
            {
                _popupConfirmButton.onClick.RemoveAllListeners();
                _popupCancelButton.onClick.RemoveAllListeners();
                _popupPanel.SetActive(false);
                tcs.TrySetResult(response);
            }

            _popupConfirmButton.onClick.AddListener(() => OnResponseButton(true));
            _popupCancelButton.onClick.AddListener(() => OnResponseButton(false));
            _popupText.text = message;
            _popupPanel.SetActive(true);

            return tcs.Task;
        }

    }
}