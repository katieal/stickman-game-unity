using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

using EventChannel;

namespace Managers
{
    public class InputManager : MonoBehaviour
    {
        [Title("Invoked Events")]
        [Header("General Controls")]
        [SerializeField] private Vector2EventSO _lookInputSO;
        [SerializeField] private Vector2EventSO _moveInputSO;
        [SerializeField] private VoidEventSO _interactSO;
        [SerializeField] private VoidEventSO _inventorySO;
        [SerializeField] private VoidEventSO _settingsSO;
        [SerializeField] private VoidEventSO _cancelSO;

        [Header("Combat")]
        [SerializeField] private VoidEventSO _attackSO;
        [SerializeField] private IntEventSO _selectWeaponSO;
        [SerializeField] private IntEventSO _selectAbilitySO;

        //[Title("Listening to Events")]


        // action maps
        private InputActionMap _uiActions, _menuActions, _playerActions;

        // UI Actions
        private InputAction _cancelAction;

        // Menu Actions
        private InputAction _settingsAction, _inventoryAction;

        // Player Actions
        private InputAction _moveAction, _lookAction, _attackAction, _interactAction;
        // need to keep them in an array to send index in the event
        private InputAction[] _weaponActions;
        private InputAction[] _abilityActions;

        #region Debug
        [Button]
        public void EnablePlayerInput(bool shouldEnable)
        {
            if (shouldEnable) { _playerActions.Enable(); }
            else { _playerActions.Disable(); }
        }
        #endregion

        private void Awake()
        {
            // Initialize UI Actions
            _uiActions = InputSystem.actions.FindActionMap("UI");
            _cancelAction = _uiActions.FindAction("Cancel");

            // Initialize Menu Actions
            _menuActions = InputSystem.actions.FindActionMap("Menu");
            _settingsAction = _menuActions.FindAction("Settings");
            _inventoryAction = _menuActions.FindAction("Inventory");

            // Initialize player actions
            _playerActions = InputSystem.actions.FindActionMap("Player");
            // general
            _moveAction = _playerActions.FindAction("Move");
            _lookAction = _playerActions.FindAction("Point");
            _interactAction = _playerActions.FindAction("Interact");
            // combat
            _attackAction = _playerActions.FindAction("Attack");
            _weaponActions = new InputAction[3] { _playerActions.FindAction("Weapon1"), _playerActions.FindAction("Weapon2"),
            _playerActions.FindAction("Weapon3") };
            _abilityActions = new InputAction[3] { _playerActions.FindAction("Ability1"), _playerActions.FindAction("Ability2"),
            _playerActions.FindAction("Ability3") };
        }

        private void OnEnable()
        {
            // load saved player controls from playerprefs
            LoadControls();

            // bind callbacks for less frequently called input actions
            _interactAction.performed += OnInteract;
            _inventoryAction.performed += OnInventory;
            _settingsAction.performed += OnSettings;

            // UI
            _cancelAction.performed += OnCancel;

            // combat callbacks
            for (int index = 0; index < _weaponActions.Length; index++)
            {
                _weaponActions[index].performed += OnWeaponSwitch;
            }
            for (int index = 0; index < _abilityActions.Length; index++)
            {
                _abilityActions[index].performed += OnSelectAbility;
            }
        }

        private void OnDisable()
        {
            // unbind callbacks for less frequently called input actions
            _interactAction.performed -= OnInteract;
            _inventoryAction.performed -= OnInventory;
            _settingsAction.performed -= OnSettings;

            // UI
            _cancelAction.performed -= OnCancel;

            // combat callbacks
            for (int index = 0; index < _weaponActions.Length; index++)
            {
                _weaponActions[index].performed -= OnWeaponSwitch;
            }
            for (int index = 0; index < _abilityActions.Length; index++)
            {
                _abilityActions[index].performed -= OnSelectAbility;
            }
        }

        // check for frequent input calls
        private void Update()
        {
            // pressed and released only trigger once when button is initially pressed
            if (_moveAction.WasReleasedThisFrame())
            {
                _moveInputSO.InvokeEvent(Vector2.zero);
            }
            if (_moveAction.IsPressed())
            {
                _moveInputSO.InvokeEvent(_moveAction.ReadValue<Vector2>());
            }

            if (_attackAction.WasPressedThisFrame())
            {
                _attackSO.InvokeEvent();
            }

            // send mouse pos coordinates when a ranged weapon is equipped
            if (_playerActions.enabled) { _lookInputSO.InvokeEvent(_lookAction.ReadValue<Vector2>()); }
        }

        private void LoadControls()
        {
            // load binding overrides if applicable
            if (PlayerPrefs.HasKey("HasBindingOverrides") && 
                PlayerPrefs.GetString("HasBindingOverrides") == "true") 
            { 
                if (PlayerPrefs.HasKey("PlayerRebinds"))
                {
                    _playerActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("PlayerRebinds"));
                }
                if (PlayerPrefs.HasKey("MenuRebinds"))
                {
                    _menuActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("MenuRebinds"));
                }
            }
        }

        // interact with environment
        private void OnInteract(InputAction.CallbackContext context)
        {
            _interactSO.InvokeEvent();
        }

        // open equipment inventory menu
        private void OnInventory(InputAction.CallbackContext context)
        {
            _inventorySO.InvokeEvent();
        }

        // open settings menu
        private void OnSettings(InputAction.CallbackContext context)
        {
            _settingsSO.InvokeEvent();
        }

        // UI action - close current screen
        private void OnCancel(InputAction.CallbackContext context)
        {
            _cancelSO.InvokeEvent();
        }

        // switch the weapon the player is holding
        private void OnWeaponSwitch(InputAction.CallbackContext context)
        {
            //Debug.Log("weapon input: " + Array.IndexOf(_weaponActions, context.action).ToString());

            _selectWeaponSO.InvokeEvent(Array.IndexOf(_weaponActions, context.action));
        }

        // switch the player's active skill
        private void OnSelectAbility(InputAction.CallbackContext context)
        {
            _selectAbilitySO.InvokeEvent(Array.IndexOf(_abilityActions, context.action) + 1);
        }
    }
}