using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EventChannel;
using Items;

/*
 * This class:
 *      - init/load/send weapon instances
 *      - input callbacks for weapon, ability, attack
 *      - determine which weapon is selected
 *      - determine which ability is selected
 *      - determine if should attack or not
 *      - update ui with cooldowns
 * 
 *      TODO: add in brief buffer in between attacks/abilities
 * 
 */


namespace Combat
{
    public class PlayerCombatManager : SerializedMonoBehaviour
    {
        [Title("Player Fields")]
        [SerializeField] private Characters.PlayerData _player;

        [Title("Invoked Events")]
        [SerializeField] private WeaponTypeEventSO _selectWeaponSO; // updates animation and ui
        [SerializeField] private IntEventSO _selectAbilitySO;
        [SerializeField] private WeaponTypeEventSO _useWeaponSO;
        [SerializeField] private BoolEventSO _onCombatStateSO;
        [Title("Cooldowns", horizontalLine: false)]
        [SerializeField] private ShowAllCooldownsEventSO _showAllCooldownsSO;
        [SerializeField] private ShowCooldownEventSO _showCooldownSO;
        [SerializeField] private VoidEventSO _stopAllCooldownsSO;
        [Title("Notifications", horizontalLine: false)]
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _equipErrorKey;
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _manaErrorKey;
        [SerializeField] private StringBoolEventSO _notificationSO;

        [Title("Listening to Events")]
        [SerializeField] private VoidPlayerDataRequestEventSO _loadPlayerDataEvent;
        [SerializeField] private WeaponTypeBoolEventSO _weaponBeltStatusEvent;
        [SerializeField] private WeaponTypeBoolEventSO _toggleWeaponAbilitiesEvent;
        [Title("Input", horizontalLine: false)]
        [SerializeField] private Vector2EventSO _lookInputSO;
        [SerializeField] private IntEventSO _weaponInputSO;
        [SerializeField] private VoidEventSO _attackInputSO;
        [SerializeField] private IntEventSO _abilityInputSO;

        public Dictionary<WeaponType, WeaponInstance> Weapons { get; private set; }
        // current weapon
        public WeaponType HeldWeapon { get; private set; }
        // current ability
        public int AbilityIndex { get; private set; }

        public bool IsHoldingWeapon { get; private set; } = false;
        private bool _isAttacking = false;
        private bool _isFollowingMouse = false;

        /// <summary>
        /// Universal delay between attacks of 0.5 seconds
        /// </summary>
        private bool _isBuffered = false;

        [Button]
        public void DisableBowAbilities()
        {
            Weapons[WeaponType.Bow].Abilities[2].SetEnabled(false);
            Weapons[WeaponType.Bow].Abilities[1].SetEnabled(false);
        }

        private void OnEnable()
        {
            _loadPlayerDataEvent.OnResultEvent += Init;
            _weaponBeltStatusEvent.OnInvokeEvent += OnWeaponBeltStatusChanged;
            _toggleWeaponAbilitiesEvent.OnInvokeEvent += OnToggleWeaponAbilities;
            
            // input
            _weaponInputSO.OnInvokeEvent += SelectWeapon;
            _abilityInputSO.OnInvokeEvent += SelectAbility;
        }

        private void OnDisable()
        {
            _lookInputSO.OnInvokeEvent -= AimAtMouse;
            _loadPlayerDataEvent.OnResultEvent -= Init;
            _weaponBeltStatusEvent.OnInvokeEvent -= OnWeaponBeltStatusChanged;
            _toggleWeaponAbilitiesEvent.OnInvokeEvent -= OnToggleWeaponAbilities;

            // input
            _weaponInputSO.OnInvokeEvent -= SelectWeapon;
            _abilityInputSO.OnInvokeEvent -= SelectAbility;
            _attackInputSO.OnInvokeEvent -= StartAttack;

        }

        #region Save/Load Data
        private void Init(SaveData.PlayerSaveData data)
        {
            // load save data if possible, else load defaults
            Weapons = new Dictionary<WeaponType, WeaponInstance> {
                { WeaponType.Sword, null },
                { WeaponType.Bow, null },
                { WeaponType.Staff, null },
            };

            // init held weapon and selected ability
            HeldWeapon = (data != null) ? data.HeldWeapon : WeaponType.Unarmed;
            AbilityIndex = (data != null) ? data.SelectedAbility : 0;

            // init ability data
            if (data != null && data.Abilities != null)
            {
                // init with saved ability data if possible, otherwise use default ability
                for (int i = 0; i < 3; i++)
                {
                    WeaponType type = (WeaponType)i;
                    Weapons[type] = (data.Abilities.GetData(type) != null) ? 
                        new WeaponInstance(type, data.Abilities.GetData(type)) : new WeaponInstance(type, _player.Settings.DefaultAbilities[type]);
                }
            }
            else
            {
                // load default abilities
                foreach (WeaponType type in _player.Settings.DefaultAbilities.Keys)
                {
                    // load saved ability data for this weapon if possible, otherwise load default abilities
                    Weapons[type] = new WeaponInstance(type, _player.Settings.DefaultAbilities[type]);
                }
            }

            UpdateCombatState();
            CheckFollowMouse();
        }

        public SaveData.AbilitySaveData GetData()
        {
            SaveData.AbilitySaveData data = new()
            {
                Sword = Weapons[WeaponType.Sword].GetAbilityData(),
                Bow = Weapons[WeaponType.Bow].GetAbilityData(),
                Staff = Weapons[WeaponType.Staff].GetAbilityData()
            };
            return data;
        }
        #endregion

        #region Event Callbacks
        private void AimAtMouse(Vector2 mousePos)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0;
            _player.LaunchOrigin.right = pos - _player.LaunchOrigin.position;
        }

        private void StartAttack()
        {
            StartCoroutine(Attack(HeldWeapon, AbilityIndex));
        }

        private void OnWeaponBeltStatusChanged(WeaponType type, bool isEquipped)
        {
            Weapons[type].IsEquipped = isEquipped;

            // if held weapon is unequipped, change to unarmed
            if (type == HeldWeapon && isEquipped == false)
            {
                HeldWeapon = WeaponType.Unarmed;
                _selectWeaponSO.InvokeEvent(WeaponType.Unarmed);
            }
        }

        // enable or disable abilities for a weapon type
        private void OnToggleWeaponAbilities(WeaponType type, bool isEnabled)
        {
            Weapons[type].IsEnabled = isEnabled;
            if (type == HeldWeapon)
            {
                if (!isEnabled)
                {
                    // if held weapon is disabled, grey out all abilities
                    _stopAllCooldownsSO.InvokeEvent();
                }
                else
                {
                    // if held weapon is changed to enabled, display all cooldowns
                    _showAllCooldownsSO.InvokeEvent(Weapons[type].GetCooldowns());
                }
            }
        }
        #endregion

        #region Status Updates
        private void CheckFollowMouse()
        {
            // ranged weapons should follow mouse
            if (HeldWeapon == WeaponType.Bow || HeldWeapon == WeaponType.Staff)
            {
                // only subscribe if it isn't already subscribed
                if (!_isFollowingMouse)
                {
                    _lookInputSO.OnInvokeEvent += AimAtMouse;
                    _isFollowingMouse = true;
                }
            }
            else
            {
                _lookInputSO.OnInvokeEvent -= AimAtMouse;
                _isFollowingMouse = false;
            }
        }

        private void UpdateCombatState()
        {
            // if player is holding a weapon, tell FSM to enter combat state
            if (HeldWeapon != WeaponType.Unarmed && IsHoldingWeapon == false)
            {
                IsHoldingWeapon = true;
                _attackInputSO.OnInvokeEvent += StartAttack;
                _onCombatStateSO.InvokeEvent(true);
            }
            // otherwise, if no weapon is held, start a timer to exit combat state
            else if (HeldWeapon == WeaponType.Unarmed && IsHoldingWeapon == true)
            {
                IsHoldingWeapon = false;
                _attackInputSO.OnInvokeEvent -= StartAttack;
                _onCombatStateSO.InvokeEvent(false);
            }
        }
        #endregion

        #region Combat
        // called by InputManager
        private void SelectWeapon(int index)
        {
            // can't swap weapons while attacking
            if (_isAttacking) { return; }

            WeaponType type = (WeaponType)index;
            // make player unarmed
            if (type == WeaponType.Unarmed || type == HeldWeapon)
            {
                // drop weapon if weapon is already held
                HeldWeapon = WeaponType.Unarmed;
                _selectWeaponSO.InvokeEvent(HeldWeapon);
                // update FSM
                UpdateCombatState();
            }
            // can only switch if weapon of that type is equipped 
            else if (Weapons[type].IsEquipped)
            {
                HeldWeapon = type;
                // update UI/animation
                _selectWeaponSO.InvokeEvent(HeldWeapon);

                // aim according to mouse pos if weapon is ranged
                CheckFollowMouse();
                UpdateCombatState();

                // only send cds if weapon abilities are enabled
                if (Weapons[type].IsEnabled)
                {
                    _showAllCooldownsSO.InvokeEvent(Weapons[type].GetCooldowns());
                }

                // select basic attack by default
                SelectAbility(0);
            }
            else
            {
                // notify player that no weapon of this type is equipped
                _notificationSO.InvokeEvent(_equipErrorKey, false);
            }
        }

        // called by InputManager, Attack()
        private void SelectAbility(int index)
        {
            // do nothing if no weapon is equipped
            if (HeldWeapon == WeaponType.Unarmed) { return; }

            // if selected weapon is broken, return
            if (!Weapons[HeldWeapon].IsEnabled) { return; }

            // if selected ability is not enabled, return
            if (!Weapons[HeldWeapon].Abilities[index].IsEnabled) { return; }

            // deselect ability if key is pressed twice
            if (AbilityIndex == index)
            {
                // set current ability to basic attack
                AbilityIndex = 0;
            }
            else { AbilityIndex = index; }
            _selectAbilitySO.InvokeEvent(AbilityIndex);
        }

        IEnumerator Attack(WeaponType type, int abilityIndex)
        {
            // if player is already attacking or is buffered, return
            if (_isAttacking || _isBuffered) { yield break; }
            // if player is unarmed or weapon type isn't equipped or enabled, return
            if (type == WeaponType.Unarmed || !Weapons[type].IsEquipped || !Weapons[type].IsEnabled) { yield break; }
            // if ability cooldown is not finished, return
            if (!Weapons[type].CheckCooldown(abilityIndex)) { yield break; }

            // if ability costs mana to use, check if player has enough
            if (Weapons[type].Abilities[abilityIndex].ManaCost > 0)
            {
                if (!_player.Mana.CheckCost(Weapons[type].Abilities[abilityIndex].ManaCost))
                {
                    // if player doesn't have enough mana to cast ability, send error messsage and return
                    _notificationSO.InvokeEvent(_manaErrorKey, false);
                    yield break;
                }
                else
                {
                    // subtract cost from player mana
                    _player.Mana.UseMana(-Weapons[type].Abilities[abilityIndex].ManaCost);
                }
            }

            _isAttacking = true;
            _isBuffered = true;

            // TODO: WHAT HAPPENS IF THIS COROUTINE IS DESTROYED???
            yield return _player.AttackLogic.Attack(type, Weapons[type].Abilities[abilityIndex]);

            // set cooldown (after animation finishes?)
            Weapons[type].StartCooldown(abilityIndex);

            // show cooldowon if weapon is not broken
            if (Weapons[type].IsEnabled)
            {
                _showCooldownSO.InvokeEvent(abilityIndex, Weapons[type].Abilities[abilityIndex].Cooldown);
            }
            SelectAbility(0);
            _isAttacking = false;

            // end attack buffer after 0.5 seconds
            Invoke(nameof(EndBuffer), 0.5f);
        }

        private void EndBuffer()
        {
            _isBuffered = false;
        }
        #endregion
    }
}