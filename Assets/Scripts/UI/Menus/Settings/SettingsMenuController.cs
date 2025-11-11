using EventChannel;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UserInterface
{
    public class SettingsMenuController : SerializedMonoBehaviour
    {
        public enum MenuType { None, Save, Settings, Controls, Exit };

        [Title("Components")]
        [SerializeField][ChildGameObjectsOnly] private GameObject _tabPanel;
        [SerializeField][ChildGameObjectsOnly] private Dictionary<MenuType, Button> _tabButtons = new();
        [SerializeField][ChildGameObjectsOnly] private Dictionary<MenuType, GameObject> _menuPanels = new();

        [Title("Invoked Events")]
        [SerializeField] private BoolEventSO _greyOutPauseEvent;

        [Title("Listening to Events")]
        [SerializeField] private VoidEventSO _settingsInputSO;
        [SerializeField] private VoidEventSO _cancelInputSO;

        [Title("Listening to Events")]
        [SerializeField] private GameStateRequestEventSO _changeStateSO;

        /// <summary>
        /// By default, key to open settings menu is the same as key to close other menus
        /// This variable tracks game state to ensure settings menu callback has lower priority than others
        /// </summary>
        private bool _isPaused = false;
        private bool _isBuffered = false;

        private MenuType _activeMenu = MenuType.None;

        private void Awake()
        {
            // ensure all menus start closed
            _tabPanel.SetActive(false);
            foreach (MenuType type in _menuPanels.Keys)
            {
                _menuPanels[type].SetActive(false);
            }
        }

        private void OnEnable()
        {
            _changeStateSO.OnResultEvent += OnGameStateChanged;
            _settingsInputSO.OnInvokeEvent += OnSettingsMenu;

            // bind tab button callbacks
            foreach (MenuType type in _tabButtons.Keys)
            {
                _tabButtons[type].onClick.AddListener(() => OpenMenu(type));
            }
        }
        private void OnDisable()
        {
            _changeStateSO.OnResultEvent -= OnGameStateChanged;
            _settingsInputSO.OnInvokeEvent -= OnSettingsMenu;

            // unbind tab button callbacks
            foreach (MenuType type in _tabButtons.Keys)
            {
                _tabButtons[type].onClick.RemoveAllListeners();
            }
        }

        #region Event Callbacks
        private void OnGameStateChanged(Managers.GameStateMachine.StateType state)
        {
            _isPaused = (state == Managers.GameStateMachine.StateType.Pause);
        }

        /// <summary>
        /// Settings Menu and Cancel key are the same by default, but this should
        /// work if the user binds them separately as well
        /// </summary>
        private void OnSettingsMenu()
        {
            // only open settings menu if another menu isn't already open
            if (_activeMenu == MenuType.None && !_isPaused) 
            {
                //Debug.Log("opening");
                StartCoroutine(BufferCoroutine());
                OpenMenu(MenuType.Save);

                // bind cancel input to close
                _cancelInputSO.OnInvokeEvent += OnCancelInput;
            }
            // close settings menu if one of them is open
            else if (_activeMenu != MenuType.None) 
            {
                StartCoroutine(BufferCoroutine());
                Close(); 
            }
        }

        private void OnCancelInput()
        {
            if (_activeMenu != MenuType.None && !_isBuffered) { Close(); }
        }
        #endregion

        private void OpenMenu(MenuType type)
        {
            // close settings menu
            if (type == MenuType.Exit) { Close(); return; }

            // when settings menu is initially opened
            if (_activeMenu == MenuType.None)
            {
                //Debug.Log("starting");
                // pause game and grey bg while inventory is open
                _greyOutPauseEvent.InvokeEvent(true);
                // show menu tabs
                
                _tabPanel.SetActive(true);
            }
            else // if another menu is currently active
            {
                // reactivate current button
                _tabButtons[_activeMenu].interactable = true;
                // deactivate current menu
                _menuPanels[_activeMenu].SetActive(false);
            }

            // activate new menu
            _menuPanels[type].SetActive(true);
            // deactivate selected button
            _tabButtons[type].interactable = false;
            _activeMenu = type;
        }

        private void Close()
        {
            // reset interactable status of tab buttons
            ResetTabs();

            // unbind callback
            _cancelInputSO.OnInvokeEvent -= OnCancelInput;

            // deactivate shown menu
            _menuPanels[_activeMenu].SetActive(false);
            _activeMenu = MenuType.None;
            _tabPanel.SetActive(false);

            // unpause game
            _greyOutPauseEvent.InvokeEvent(false);
        }

        private void ResetTabs()
        {
            foreach (MenuType key in _tabButtons.Keys)
            {
                _tabButtons[key].interactable = true;
            }
        }

        private IEnumerator BufferCoroutine()
        {
            _isBuffered = true;
            yield return new WaitForEndOfFrame();
            _isBuffered = false;
        }

        [Button]
        public void Open()
        {
            OpenMenu(MenuType.Settings);
        }
    }
}