using System.Collections.Generic;
using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "VoidStringListUniTaskEventSO", menuName = "Events/UniTask/Void StringList")]
    public class VoidStringListUniTaskEventSO : VoidGenericUniTaskEventSO<List<string>> { }
}