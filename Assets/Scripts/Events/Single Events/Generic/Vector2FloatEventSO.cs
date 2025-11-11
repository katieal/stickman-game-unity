using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "Vector2EventSO", menuName = "Events/Single/Generic/Vector2 Float Event")]
    public class Vector2FloatEventSO : GenericGenericSingleEventSO<Vector2, float> { }
}