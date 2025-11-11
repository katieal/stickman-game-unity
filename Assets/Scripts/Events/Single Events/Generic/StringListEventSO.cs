using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "StringListEventSO", menuName = "Events/Single/Generic/String List Event")]
    public class StringListEventSO : GenericSingleEventSO<List<string>> { }
}