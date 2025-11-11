using Player;
using UnityEngine;

using EventChannel;

namespace Items
{
    public class ItemPickup : MonoBehaviour
    {
        // no tooltip for now
        [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private CircleCollider2D _collider;

        [Header("Invoked Events")]
        [SerializeField] private ItemEventSO _addItemSO;
        //[SerializeField] private MaterialEventSO _addMaterialSO;
        //[SerializeField] private WeaponEventSO _addWeaponSO;


        private ItemInstance _item = null;

        // item can be picked up by player starting from this many seconds
        // after it has been spawned
        private float _pickupTimer = 1f;

        // seconds until item despawns
        private float _despawnTimer = 5f;

        public void Init(ItemInstance item)
        {
            _item = item;
            _spriteRenderer.sprite = GameData.ItemDatabase.Instance.GetIcon(item.ItemId);
        }

        private void OnEnable()
        {
            _collider.enabled = false;
            Invoke(nameof(EnablePickup), _pickupTimer);
            Invoke(nameof(DestroySelf), _despawnTimer);
        }
        private void EnablePickup() { _collider.enabled = true; }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            // add to player's inventory
            if (collider.CompareTag("Player"))
            {
                // if (get playerInventory.hasenoughroom, add and destroy)

                if (_item != null) 
                { 
                    if (Player.Inventory.Instance.HasRoom(_item))
                    {
                        _addItemSO.InvokeEvent(_item);
                        // destroy self after being picked up
                        DestroySelf();
                    }
                    else
                    {
                        // else, display inventory full notification and do nothing
                        Debug.Log("inventory full!");
                    }

                }
            }
        }

        private void DestroySelf() { Destroy(gameObject); }

        private void OnDrawGizmosSelected()
        {
            // multiply radius by scale
            Gizmos.DrawWireSphere(transform.position, 0.5f * 0.5f);
        }
    }
}
