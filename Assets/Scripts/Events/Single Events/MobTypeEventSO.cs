using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "MobTypeEventSO", menuName = "Events/Single/MobTypeEvent")]
    public class MobTypeEventSO : GenericSingleEventSO<Characters.MobType> { }
}