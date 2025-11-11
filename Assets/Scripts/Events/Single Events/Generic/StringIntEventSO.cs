using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "StringIntEventSO", menuName = "Events/Single/Generic/String Int")]
    public class StringIntEventSO : GenericGenericSingleEventSO<string, int> { }
}