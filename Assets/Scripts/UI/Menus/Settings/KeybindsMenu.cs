using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace SettingsUI
{
    public class KeybindsMenu : MonoBehaviour
    {
        // keyboard only for now
        [Title("General Controls")]
        [SerializeField] private GameObject _generalControlsPanel;
        [SerializeField] private GameObject _combatControlsPanel, _rebindPanel;
        [SerializeField] private TMP_Text _rebindActionText;
        [SerializeField] private Button _resetToDefaultButton;

        private InputBindingUI[] _generalControls, _combatControls;
        private InputActionMap _playerMap, _menuMap, _uiMap;

        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;
        private string _currentLayout;
        private bool _hasBindingOverrides = false;

        private void Awake()
        {
            _playerMap = InputSystem.actions.FindActionMap("Player");
            _menuMap = InputSystem.actions.FindActionMap("Menu");
            _uiMap = InputSystem.actions.FindActionMap("UI");
            _generalControls = _generalControlsPanel.GetComponentsInChildren<InputBindingUI>();
            _combatControls = _combatControlsPanel.GetComponentsInChildren<InputBindingUI>();
            _resetToDefaultButton.onClick.AddListener(() => ResetBindingsToDefault());

            _currentLayout = "Keyboard&Mouse";
        }

        private void OnEnable()
        {
            _hasBindingOverrides = false;
            ShowBindings();
        }
        private void OnDisable()
        {
            // save any changes made this session
            if (_hasBindingOverrides) { SaveBindings(); }
        }

        #region Utility Methods
        private string GetKeyName(InputAction action, int bindingIndex)
        {
            return action.GetBindingDisplayString(bindingIndex);
        }

        private List<int> FindBindingsWithGroup(InputAction action, string group)
        {
            List<int> result = new List<int>();

            for (int index = 0; index < action.bindings.Count; index++)
            {
                if (action.bindings[index].groups.Contains(_currentLayout))
                {
                    result.Add(index);
                }
            }

            return result;
        }

        private List<int> FindCompositePartIndex(InputAction action, string part)
        {
            List<int> result = new List<int>();

            for (int index = 0; index < action.bindings.Count; index++)
            {
                if (action.bindings[index].groups.Contains(_currentLayout) && action.bindings[index].name == part)
                {
                    result.Add(index);
                }
            }

            return result;
        }
        #endregion

        private void ShowBindings()
        {
            AssignPanel(_generalControls);
            AssignPanel(_combatControls);
        }

        private void AssignPanel(InputBindingUI[] controls)
        {
            // assign key panels for control panel
            foreach (InputBindingUI actionPanel in controls)
            {
                InputAction action = actionPanel.ActionRef.action;

                // assign key to composite part if the action is the move action
                if (actionPanel.IsMoveAction)
                {
                    List<int> indexes = FindCompositePartIndex(action, actionPanel.Part.ToString());
                    #region Debug
#if UNITY_EDITOR
                    Debug.Assert(indexes.Count == 2, $"Incorrect number of bindings found for {action.name} {actionPanel.Part}");
#endif
                    #endregion
                    InitPanelKey(actionPanel.KeyPanel1, action, indexes[0]);
                    InitPanelKey(actionPanel.KeyPanel2, action, indexes[1]);
                }
                else
                {
                    // display primary and secondary keybindings
                    List<int> indexes = FindBindingsWithGroup(action, _currentLayout);

                    #region Debug
#if UNITY_EDITOR
                    if (actionPanel.HasSecondaryBinding)
                    {
                        Debug.Assert(indexes.Count == 2, $"Incorrect number of bindings found for {action.name}");
                    }
                    else
                    {
                        Debug.Assert(indexes.Count == 1, $"Incorrect number of bindings found for {action.name}");
                    }
#endif
                    #endregion

                    InitPanelKey(actionPanel.KeyPanel1, action, indexes[0]);
                    // show secondary binding if applicable
                    if (actionPanel.HasSecondaryBinding) { InitPanelKey(actionPanel.KeyPanel2, action, indexes[1]); }
                    else { actionPanel.KeyPanel2?.SetPanelNull(); }
                }
            }
        }

        private void InitPanelKey(KeyPanel panel, InputAction action, int bindingIndex)
        {
            string name = GetKeyName(action, bindingIndex);

            if (name.IsNullOrWhitespace())
            {
                // if no key is assigned
                panel.ResetKey();
            }
            else
            {
                // otherwise, show key name
                panel.SetKeyName(name);
            }
            panel.Button.onClick.RemoveAllListeners();
            panel.Button.onClick.AddListener(() => OnRebind(panel, action, bindingIndex));
        }

        private void OnRebind(KeyPanel panel, InputAction action, int bindingIndex)
        {
            // null out operation
            _rebindOperation?.Cancel();

            void CleanUp()
            {
                _rebindOperation?.Dispose();
                _rebindOperation = null;
                _menuMap.Enable();
                _uiMap.Enable();
            }

            // other action maps should already have been disabled when settings menu was opened
            _menuMap.Disable();
            _uiMap.Disable();

            // configure rebind operation
            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(
                    operation =>
                    {
                        _rebindPanel.SetActive(false);
                        CleanUp();
                    })
                .OnComplete(
                    operation =>
                    {
                        // update display
                        panel.SetKeyName(GetKeyName(action, bindingIndex));

                        _rebindPanel.SetActive(false);
                        _hasBindingOverrides = true;
                        CleanUp();
                    });

            // update action label
            _rebindActionText.text = $"Binding: {action.name}";
            _rebindPanel.SetActive(true);

            _rebindOperation.Start();
        }

        private void SaveBindings()
        {
            PlayerPrefs.SetString("HasBindingOverrides", "true");
            string playerRebinds = _playerMap.SaveBindingOverridesAsJson();
            string menuRebinds = _menuMap.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("PlayerRebinds", playerRebinds);
            PlayerPrefs.SetString("MenuRebinds", menuRebinds);
        }

        private void ResetBindingsToDefault()
        {
            _playerMap.RemoveAllBindingOverrides();
            _menuMap.RemoveAllBindingOverrides();

            PlayerPrefs.SetString("HasBindingOverrides", "false");
            PlayerPrefs.DeleteKey("PlayerRebinds");
            PlayerPrefs.DeleteKey("MenuRebinds");
            // refresh binding display
            ShowBindings();
        }
    }
}