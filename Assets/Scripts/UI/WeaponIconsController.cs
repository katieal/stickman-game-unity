using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

using EventChannel;

namespace PlayerUI
{
    public class WeaponIconsController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject[] _greyedPanels = new GameObject[4];
        [Header("Listening to Events")]
        [SerializeField] private IntBoolEventSO _enableWeaponSO;

        private void OnEnable()
        {
            UpdateIcons();
            _enableWeaponSO.OnInvokeEvent += EnableWeapon;
        }

        private void OnDisable()
        {
            _enableWeaponSO.OnInvokeEvent -= EnableWeapon;
        }

        #region LoadData
        // Load data: check which weapons are enabled
        // grey panel will be disabled if true and enabled if false
        private void UpdateIcons()
        {
            for (int index = 0; index < _greyedPanels.Length; index++)
            {
                //_greyedPanels[index].SetActive(!PlayerStaticData.Equipment.CheckEnabled(index));
            }
        }
        #endregion

        private void EnableWeapon(int index, bool enabled)
        {
            // weapon is greyed out if it is disabled
            _greyedPanels[index].SetActive(!enabled);
        }
    }
}

