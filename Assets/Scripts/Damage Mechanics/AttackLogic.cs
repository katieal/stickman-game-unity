using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UIElements;

using Characters;
using Sirenix.OdinInspector;

namespace Combat
{
    public class AttackLogic : MonoBehaviour
    {
        [Title("Components")]
        [Tooltip("The rigid body of the main parent object, used to filter self from contacts.")]
        [SerializeField] protected Rigidbody2D _parentRigidbody;
        [Tooltip("Transform of main parent object, used to align projectiles and calculate knockback direction.")]
        [SerializeField] private Transform _parentOrigin;
        [Tooltip("Location that projectiles will launch from.")]
        [SerializeField] protected Transform _launchOrigin;

        [Tooltip("Capsule collider used to visualize the size of the Physics2D OverlapAll query (should always be disabled).")]
        [SerializeField] protected CapsuleCollider2D _attackAreaGuide;

        [Title("Attack Targets")]
        [Tooltip("Tags that this entity can damage.")]
        [SerializeField] protected List<string> _enemyTags;


        // return new damage instance based on ability info
        protected virtual DamageInstance GetDamageInstance(AbilityInstance ability)
        {
            return new DamageInstance(ability.Damage, ability.KnockbackMultiplier, ability.StunSeconds);
        }

        // formatted this way to let player subtract weapon durability after hitting an enemy
        protected virtual void MeleAttack(AbilityInstance ability)
        {
            Collider2D[] colliders = FilterResults(SwingWeapon());
            if (colliders == null || colliders.Length == 0) { return; }
            else 
            {
                HitEnemies(GetDamageInstance(ability), ability.IsSingleTarget, colliders);
            }
        }

        // get all colliders within attack range
        protected virtual Collider2D[] SwingWeapon()
        {
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll(transform.position +
                new Vector3(_attackAreaGuide.offset.x, _attackAreaGuide.offset.y), _attackAreaGuide.size,
                CapsuleDirection2D.Horizontal, 0f, LayerMask.GetMask("HitBox"));
            
            return colliders;
        }

        protected Collider2D[] FilterResults(Collider2D[] results)
        {
            // convert to list in order to remove items
            List<Collider2D> colliders = results.ToList();

            // remove colliders that aren't enemies or are part of self
            for (int index = (colliders.Count - 1); index >= 0; index--)
            {
                if (!_enemyTags.Contains(colliders[index].tag) || colliders[index].attachedRigidbody == _parentRigidbody)
                {
                    colliders.RemoveAt(index);
                }
            }
            if (colliders.Count == 0) { return null; }
            else
            {
                return colliders.ToArray();
            }
        }

        // call dealdamage on all valid colliders
        protected void HitEnemies(DamageInstance damage, bool isSingleTarget, Collider2D[] colliders)
        {
            foreach (Collider2D collider in colliders)
            {
                DealDamage(damage, collider);

                // if ability only damages one enemy at a time, break at the first collider damaged
                if (isSingleTarget) { break; }
            }
        }

        protected virtual void RangedAttack(AbilityInstance ability)
        {
            // add animation trigger
            DamageInstance damageInstance = GetDamageInstance(ability);
            FireProjectile(ability, damageInstance);
        }

        protected virtual void FireProjectile(AbilityInstance ability, DamageInstance damage)
        {
            // spawn projectile
            GameObject prefab = GameData.SpawnableData.Instance.GetProjectile(ability.ProjectileName);
            GameObject projectile = Instantiate(prefab, _launchOrigin.position, _launchOrigin.rotation);

            // set projectile scale according to launch origin
            projectile.transform.localScale = new Vector2(_launchOrigin.localScale.x, _launchOrigin.localScale.y);

            // initialize projectile
            ProjectileLogic logic = projectile.GetComponent<ProjectileLogic>();
            logic.Init(damage, ability.Velocity, _parentRigidbody, _enemyTags);

            logic.OnAttackConnectEvent += DealDamage;
        }

        // deals damageinstance damage to collider
        protected virtual void DealDamage(DamageInstance damageInstance, Collider2D collider)
        {
            // calculate knockback direction
            var kbdir = Mathf.Sign(collider.transform.position.x - _parentOrigin.position.x);
            damageInstance.KnockbackMultiplier *= kbdir;

            ICharacter character = collider.attachedRigidbody.gameObject.GetComponent<ICharacter>();
            if (character is IDamagable damagable)
            {
                damagable.GetHit(damageInstance);
            }
            else
            {
                Debug.Log("not damagable!");
            }
        }
    }
}
