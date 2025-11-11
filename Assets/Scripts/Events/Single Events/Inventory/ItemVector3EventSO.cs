using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "ItemVector3EventSO", menuName = "Events/Single/Items/ItemVector3")]
    public class ItemVector3EventSO : GenericGenericSingleEventSO<Items.ItemInstance, Vector3> { }
}