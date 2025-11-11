using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "ShowCooldownEventSO", menuName = "Events/Single/Combat/Show Cooldown Event")]
    public class ShowCooldownEventSO : GenericGenericSingleEventSO<int, Combat.Cooldown> { }
}