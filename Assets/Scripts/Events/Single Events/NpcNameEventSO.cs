using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "NpcNameEventSO", menuName = "Events/Single/NpcName")]
    public class NpcNameEventSO : GenericSingleEventSO<Characters.NpcName> { }
}