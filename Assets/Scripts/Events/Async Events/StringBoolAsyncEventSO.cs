using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "StringBoolAsyncEventSO", menuName = "Events/Async/String Bool")]
    public class StringBoolAsyncEventSO : GenericGenericAsyncEventSO<string, bool> { }
}