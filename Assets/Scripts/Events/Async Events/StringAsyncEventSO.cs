using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "StringAsyncEventSO", menuName = "Events/Async/String Void")]
    public class StringAsyncEventSO : GenericVoidAsyncEventSO<string> { }
}