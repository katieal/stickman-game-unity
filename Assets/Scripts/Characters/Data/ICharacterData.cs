using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


namespace Characters
{
    public interface ICharacter
    {
        [Title("Stats")]
        public float MoveSpeed { get; }

        [Title("Components")]
        [SerializeField] public Transform Position { get; }
        [SerializeField] public Rigidbody2D RigidBody { get; }
        [SerializeField] public Animator Animator { get; }

        [Title("Scripts")]
        [SerializeField] public MoveLogic MoveLogic { get; }
        [SerializeField] public AudioController Audio { get; }
        [SerializeField] public AnimationController Animation { get; }
    }

    public interface IDamagable
    {
        [Title("Damagable Stats")]
        public float MaxHealth { get; }

        [Tooltip("Number seconds to wait between each attack.")]
        public float AttackSpeed { get; }

        [Tooltip("The base amount of knockback this character takes after getting hit.")]
        public Vector2 Knockback { get; }

        [Tooltip("Character is frozen for this many seconds after getting hit.")]
        public float StaggerSeconds { get; }

        [Title("Scripts")]
        [SerializeField] public HealthPoints Health { get; }

        // method
        public void GetHit(Combat.DamageInstance damageInstance);
    }

    public interface INpc
    {
        public NpcName Name { get; }
    }

    public interface IManaUser
    {
        public float MaxMana { get; }
        public ManaPoints Mana { get; }
    }
}