using UnityEngine;
using System.Collections.Generic;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "StringListAsyncEventSO", menuName = "Events/Async/String List")]
    public class StringListAsyncEventSO : GenericVoidAsyncEventSO<List<string>> { }
}