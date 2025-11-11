using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "Vector2EventSO", menuName = "Events/Single/Generic/Vector2 Event")]
    public class Vector2EventSO : GenericSingleEventSO<Vector2> { }
}