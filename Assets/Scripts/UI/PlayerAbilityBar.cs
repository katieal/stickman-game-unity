using EventChannel;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;


/*
* ================================
* Description: this class handles the functions of the player's ability bar UI, including:
*      - input detection (attack (LMB), equip weapon (1-3), use ability(Q,E,R))
*      - send OnAttackEvent when player attacks
*      - send OnWeaponSwitchEvent when player switches their equipped weapon
*      - set ability icons based on equipped weapon
*      - highlight ability icon when 1-3 is pressed
*      - handle icon cooldown animation
* Info needed:
*      - currently equipped weapon (from animator)
*      - enabled weapons (from player.equipment inventory)
*      - which abilities are enabled
*      - ability cooldown (func return)
* =================================
*/


namespace PlayerUI
{

    [Serializable]
    public struct AbilityBarSlot
    {
        [SerializeField] public Image Image;
        [SerializeField] public GameObject SelectedPanel;
        [SerializeField] public Image CooldownImage;
        public string Label;
    }

    // don't need to know if abilities are enabled, just need a method
    // to turn them grey or not

    public class PlayerAbilityBar : MonoBehaviour
    {
        [SerializeField] private AbilityBarSlot[] _abilityBar;
        [SerializeField] private SpriteLibrary _sprites;

        [Header("Listening to Events")]
        [SerializeField] private IntEventSO _selectAbilitySO;
        [SerializeField] private WeaponTypeEventSO _selectWeaponSO;
        [SerializeField] private ShowAllCooldownsEventSO _showAllCooldownsSO;
        [SerializeField] private ShowCooldownEventSO _showCooldownSO;
        [SerializeField] private VoidEventSO _stopAllCooldownsSO;

        private void OnEnable()
        {
            _selectAbilitySO.OnInvokeEvent += SetActiveAbility;
            _selectWeaponSO.OnInvokeEvent += SetAbilityIcons;
            _stopAllCooldownsSO.OnInvokeEvent += StopAllCooldowns;
            _showAllCooldownsSO.OnInvokeEvent += ShowCooldowns;
            _showCooldownSO.OnInvokeEvent += ShowCooldown;

        }
        private void OnDisable()
        {
            _selectAbilitySO.OnInvokeEvent -= SetActiveAbility;
            _selectWeaponSO.OnInvokeEvent -= SetAbilityIcons;
            _stopAllCooldownsSO.OnInvokeEvent -= StopAllCooldowns;
            _showAllCooldownsSO.OnInvokeEvent -= ShowCooldowns;
            _showCooldownSO.OnInvokeEvent -= ShowCooldown;

        }

        private void Start()
        {
            SetAbilityIcons(Items.WeaponType.Unarmed);
        }

        // highlight selected ability
        private void SetActiveAbility(int index)
        {
            ResetSlots();
            _abilityBar[index].SelectedPanel.SetActive(true);
        }

        // unhighlight all ability slots
        private void ResetSlots()
        {
            foreach (AbilityBarSlot slot in _abilityBar)
            {
                slot.SelectedPanel.SetActive(false);
            }
        }

        private void SetAbilityIcons(Items.WeaponType type)
        {
            // reset all ability bar visuals
            ResetAbilities();
            
            // set new weapon bar
            for (int index = 0; index < _abilityBar.Length; index++)
            {
                // set weapon icons
                _abilityBar[index].Image.sprite = _sprites.GetSprite(type.ToString(), _abilityBar[index].Label);

                // grey out all abilities by default
                _abilityBar[index].CooldownImage.fillAmount = 1;
            }
        }

        private void ResetAbilities()
        {
            // stop all ongoing ability bar cooldown visuals
            StopAllCoroutines();

            // unhighlight all slots
            ResetSlots();
        }

        private void StopAllCooldowns()
        {
            ResetAbilities();
            for (int index = 0; index < _abilityBar.Length; index++)
            {
                // grey out all abilities
                _abilityBar[index].CooldownImage.fillAmount = 1;
            }
        }

        private void ShowCooldowns(Combat.Cooldown[] cooldowns)
        {
            // stop all ongoing ability bar cooldown visuals
            //StopAllCoroutines();

            for (int index = 0; index < cooldowns.Length; index++)
            {
                ShowCooldown(index, cooldowns[index]);
            }
        }

        // display cooldown for designated ability slot
        private void ShowCooldown(int abilityIndex, Combat.Cooldown cooldown)
        {
            if (cooldown == null) { _abilityBar[abilityIndex].CooldownImage.fillAmount = 1; }
            else if (cooldown.End < Time.time) { _abilityBar[abilityIndex].CooldownImage.fillAmount = 0; }
            else
            {
                float timeRemaining = cooldown.End - Time.time;
                StartCoroutine(Cooldown(abilityIndex, timeRemaining / cooldown.Duration, timeRemaining));
            }
        }

        IEnumerator Cooldown (int index, float startFill, float seconds)
        {
            // gradually decrease fill
            for (float time = 0; time <= seconds; time += Time.deltaTime)
            {
                _abilityBar[index].CooldownImage.fillAmount = Mathf.Lerp(startFill, 0, time / seconds);
                yield return null;
            }
        }
    }
}