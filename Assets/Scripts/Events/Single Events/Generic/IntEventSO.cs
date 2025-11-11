using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "IntEventSO", menuName = "Events/Single/Generic/Int Event")]
    public class IntEventSO : GenericSingleEventSO<int> { }
}