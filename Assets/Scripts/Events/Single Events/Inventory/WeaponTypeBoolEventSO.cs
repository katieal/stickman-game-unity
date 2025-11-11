using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "WeaponTypeBoolEventSO", menuName = "Events/Single/Items/WeaponTypeBoolEvent")]
    public class WeaponTypeBoolEventSO: GenericGenericSingleEventSO<Items.WeaponType, bool>  { }
}