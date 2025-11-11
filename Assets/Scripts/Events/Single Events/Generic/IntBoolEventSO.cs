using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "IntBoolEventSO", menuName = "Events/Single/Generic/IntBool")]
    public class IntBoolEventSO : GenericGenericSingleEventSO<int, bool> { }
}