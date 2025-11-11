using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "BoolEventSO", menuName = "Events/Single/Generic/Bool Event")]
    public class BoolEventSO : GenericSingleEventSO<bool> { }
}
