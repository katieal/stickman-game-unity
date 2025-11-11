using UnityEngine;
using System.Collections.Generic;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "StringListVoidUniTaskEventSO", menuName = "Events/UniTask/StringList Void")]
    public class StringListVoidUniTaskEventSO : GenericVoidUniTaskEventSO<List<string>> { }
}