using EventChannel;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserInterface
{
    public class InGameMenuController : SerializedMonoBehaviour
    {

        public enum WindowType { None = -1, Inventory = 0, Quests = 1, Abilities, Options, Exit };

        [Title("Components")]
        [SerializeField][ChildGameObjectsOnly] private GameObject _tabPanel;
        [SerializeField][ChildGameObjectsOnly] private Dictionary<WindowType, Button> _tabButtons = new();
        [SerializeField][ChildGameObjectsOnly] private Dictionary<WindowType, GameObject> _windows = new();

        [Header("Invoked Events")]
        [SerializeField] private BoolEventSO _greyOutPauseEvent;

        [Header("Listening to Events")]
        [SerializeField] private VoidEventSO _inventoryInputSO;
        [SerializeField] private VoidEventSO _cancelInputSO;
        //[SerializeField] private VoidQuestDataListEventSO _getActiveQuestsEvent;


        private WindowType _activeWindow = WindowType.None;

        private void Awake()
        {
            // ensure all windows start closed
            _tabPanel.SetActive(false);
            foreach (WindowType type in _windows.Keys)
            {
                _windows[type].SetActive(false);
            }
        }

        private void OnEnable()
        {
            // bind tab button callbacks
            foreach (WindowType type in _tabButtons.Keys)
            {
                _tabButtons[type].onClick.AddListener(() => OpenWindow(type));
            }

            // bind input callbacks
            _inventoryInputSO.OnInvokeEvent += ToggleInventory;
        }

        private void OnDisable()
        {
            // unbind tab button callbacks
            foreach (WindowType type in _tabButtons.Keys)
            {
                _tabButtons[type].onClick.RemoveAllListeners();
            }

            // unbind input callbacks
            _inventoryInputSO.OnInvokeEvent -= ToggleInventory;
        }

        private void ToggleInventory()
        {
            // when inventory button is pressed, open window if it isn't already open, otherwise close it
            if (_activeWindow != WindowType.Inventory) { OpenWindow(WindowType.Inventory); }
            else { Close(); }
        }

        private void OpenWindow(WindowType type)
        {
            // if close button is pressed
            if (type == WindowType.Exit) { Close(); return; }

            // if no window is currently active
            if (_activeWindow == WindowType.None)
            {
                // pause game and grey bg while inventory is open
                _greyOutPauseEvent.InvokeEvent(true);
                // show menu tabs
                _tabPanel.SetActive(true);

                // bind cancel button callback
                _cancelInputSO.OnInvokeEvent += Close;
            }
            else // if another window is already active
            {
                // reactivate current button
                _tabButtons[_activeWindow].interactable = true;
                // deactivate current window
                _windows[_activeWindow].SetActive(false);
            }

            // activate new window
            _windows[type].SetActive(true);
            // deactivate selected button
            _tabButtons[type].interactable = false;
            _activeWindow = type;
        }

        private void Close()
        {
            ResetTabs();

            // unbind callback
            _cancelInputSO.OnInvokeEvent -= Close;

            // deactivate window
            _windows[_activeWindow].SetActive(false);
            _activeWindow = WindowType.None;
            _tabPanel.SetActive(false);

            // unpause game
            _greyOutPauseEvent.InvokeEvent(false);
        }

        private void ResetTabs()
        {
            foreach (var key in _tabButtons.Keys)
            {
                _tabButtons[key].interactable = true;
            }
        }
    }
}