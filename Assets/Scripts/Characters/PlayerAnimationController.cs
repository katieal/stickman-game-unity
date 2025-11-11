using Combat;
using UnityEngine;

using EventChannel;
using Characters;

public class PlayerAnimationController : AnimationController
{
    // all animations should happen in state machine?????
    // maybe state animations happen in state machine and
    // everything else happens here?

    // Movement
    //[SerializeField] private BoolEventSO _onMoveSO;


    [Header("Listening to Events")]
    //[SerializeField] private VoidEventSO _attackSO;
    [SerializeField] private VoidPlayerDataRequestEventSO _sendPlayerDataSO;
    [SerializeField] private WeaponTypeEventSO _selectWeaponSO;
    [SerializeField] private StringEventSO _animationTriggerSO;

    private void OnEnable()
    {
        _sendPlayerDataSO.OnResultEvent += LoadHeldWeapon;
        // Combat
        _selectWeaponSO.OnInvokeEvent += SetWeapon;
        _animationTriggerSO.OnInvokeEvent += TriggerAnimataion;
    }

    private void OnDisable()
    {
        _sendPlayerDataSO.OnResultEvent -= LoadHeldWeapon;
        // Combat
        _selectWeaponSO.OnInvokeEvent -= SetWeapon;
        _animationTriggerSO.OnInvokeEvent -= TriggerAnimataion;
    }

    #region Save Data
    private void LoadHeldWeapon(SaveData.PlayerSaveData data)
    {
        if (data != null) { SetWeapon(data.HeldWeapon); }
        else { SetWeapon(Items.WeaponType.Unarmed); }
    }
    #endregion

    private void SetWeapon(Items.WeaponType type)
    {
        _animator.SetInteger("EquippedWeapon", (int)type);
    }
}
