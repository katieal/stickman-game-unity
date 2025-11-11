using System;
using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "GuidIntEventSO", menuName = "Events/Single/GuidIntEvent")]
    public class GuidIntEventSO : GenericGenericSingleEventSO<Guid, int> { }
}