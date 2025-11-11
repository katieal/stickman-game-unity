using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "FloatEventSO", menuName = "Events/Single/Generic/Float Event")]
    public class FloatEventSO : GenericSingleEventSO<float> { }
}