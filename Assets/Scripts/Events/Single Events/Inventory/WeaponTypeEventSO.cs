using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "WeaponTypeEventSO", menuName = "Events/Single/Items/WeaponTypeEvent")]
    public class WeaponTypeEventSO : GenericSingleEventSO<Items.WeaponType> { }
}