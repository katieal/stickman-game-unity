using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "ShowAllCooldownsEventSO", menuName = "Events/Single/Combat/Show All Cooldowns Event")]
    public class ShowAllCooldownsEventSO : GenericSingleEventSO<Combat.Cooldown[]> { }
}
