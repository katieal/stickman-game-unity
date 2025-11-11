using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "IntBoolRequestEventSO", menuName = "Events/Request/Int Bool")]
    public class IntBoolRequestEventSO : GenericGenericRequestEventSO<int, bool> { }
}