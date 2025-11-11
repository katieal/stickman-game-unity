using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "ItemEventSO", menuName = "Events/Single/Items/Item Event")]
    public class ItemEventSO : GenericSingleEventSO<Items.ItemInstance> { }
}
