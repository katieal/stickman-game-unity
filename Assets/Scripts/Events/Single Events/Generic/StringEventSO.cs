using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "StringEventSO", menuName = "Events/Single/Generic/String Event")]
    public class StringEventSO : GenericSingleEventSO<string> { }
}