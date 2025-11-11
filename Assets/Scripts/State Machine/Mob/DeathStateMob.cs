using Sirenix.OdinInspector;
using UnityEngine;

using EventChannel;
using UnityEngine.Events;

namespace Characters.StateMachine
{
    public class DeathStateMob : DeathStateBase
    {
        [Title("Mob Death State Fields")]
        [Tooltip("Number of seconds after being killed until the corpse despawns and loot drops.")]
        [SerializeField] private float _despawnTime = 2f;

        [Title("Invoked Events")]
        [SerializeField] private ItemVector3EventSO _spawnItemDropSO;

        public UnityAction OnDeath;

        private MobData _mob;

        #region Interface Methods
        public override void Init(ICharacter character)
        {
            Character = character;
            _mob = character as MobData;
        }
        public override void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion

        private void OnEnable()
        {
            OnDeath.Invoke();
            // TODO: play death animation

            // start death sequence after specified num of seconds
            Invoke(nameof(DeathSequence), _despawnTime);
        }
        private void DeathSequence()
        {
            // spawn loot using spawn manager
            foreach (Items.ItemInstance item in _mob.LootDrops)
            {
                _spawnItemDropSO.InvokeEvent(item, transform.position);
            }

            // destroy self after spawning loot
            Destroy(transform.parent.gameObject);
        }
    }
}