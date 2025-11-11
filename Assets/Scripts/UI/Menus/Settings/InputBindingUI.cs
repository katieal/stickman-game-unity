using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.GettingStarted;
using Sirenix.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SettingsUI
{

    public class InputBindingUI : MonoBehaviour
    {
        [Title("Info")]
        public InputActionReference ActionRef;
        public bool IsMoveAction;
        [ShowIf("IsMoveAction")] public MoveCompositePart Part;
        public bool HasSecondaryBinding = true;

        [Title("Components")]
        public KeyPanel KeyPanel1;
        public KeyPanel KeyPanel2;
    }

    public enum MoveCompositePart { left, right, up, down };

    [Serializable]
    public class KeyPanel
    {
        public Button Button;
        public TMP_Text KeyText;
        public Image ButtonImage;
        //[HideInInspector] public 

        public void SetKeyName(string keyName)
        {
            Button.interactable = true;
            KeyText.enabled = true;
            KeyText.text = keyName;
            ButtonImage.enabled = true;
        }

        public void ResetKey()
        {
            Button.interactable = true;
            KeyText.enabled = true;
            KeyText.text = "None";
            ButtonImage.enabled = false;
        }

        public void SetPanelNull()
        {
            if (Button != null) { Button.interactable = false; }
            if (KeyText != null) { KeyText.enabled = false; }
            if (ButtonImage != null) { ButtonImage.enabled = false; }
        }
    }
}