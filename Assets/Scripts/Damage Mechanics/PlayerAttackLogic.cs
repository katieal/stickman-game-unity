using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using EventChannel;
using Sirenix.Utilities;

/*
 * Current Functions:
 *      - trigger attack animation
 *      - execute attack 
 *      - damage colliders
 *      - subtract weapon durability
 * 
 */


namespace Combat
{
    public class PlayerAttackLogic : AttackLogic
    {
        [Header("Invoked Events")]
        [SerializeField] private WeaponTypeEventSO _useWeaponSO;
        [SerializeField] private StringEventSO _animationEvent;

        // flag 
        private bool _animationFinished = false;

        private void OnEnable()
        {
            ProjectileASM.OnAttackFinishEvent += RaiseAnimationFlag;
        }

        private void OnDisable()
        {
            ProjectileASM.OnAttackFinishEvent -= RaiseAnimationFlag;
        }

        #region Event Callbacks
        private void RaiseAnimationFlag()
        {
            _animationFinished = true;
        }
        #endregion

        // use weapontype param to prevent errors from held weapon changing in the middle of method execution
        public IEnumerator Attack(WeaponType type, AbilityInstance ability)
        {
            // if has cast animation, trigger it and wait for animation to finish
            if (ability.HasCastAnimation) 
            { 
                _animationEvent.InvokeEvent(ability.CastAnimationTrigger);
                // wait for animation
                yield return new WaitUntil(() => _animationFinished);
                // reset flag
                _animationFinished = false;

                // TODO: IMPLEMENT!!!!!!!!!!!!!!!!!!!
                //_animationEvent.InvokeEvent(ability.AnimationTrigger);
            }
            // otherwise, trigger regular animation and wait for cast seconds
            else 
            { 
                _animationEvent.InvokeEvent(ability.AnimationTrigger);
                yield return new WaitForSeconds(ability.AttackDelay);
            }

            // execute ability based on type
            switch (ability.Type)
            {
                case AbilityType.Mele: MeleAttack(ability, type); break;
                case AbilityType.Ranged: 
                    RangedAttack(ability, type);
                    // subtract durability after casting
                    _useWeaponSO.InvokeEvent(type);
                    break;
                case AbilityType.Charge: break;
                case AbilityType.Spell: break;
            }
        }

        // method overload to add weapon damage to damage instance
        protected DamageInstance GetDamageInstance(AbilityInstance ability, WeaponType type)
        {
            DamageInstance damage = new DamageInstance(ability.Damage, ability.KnockbackMultiplier, ability.StunSeconds);

            // add weapon damage
            damage.Damage += Player.Inventory.Instance.GetDamage(type);
            return damage;
        }

        // method overload to pass weapon index and subtract weapon durability
        protected void MeleAttack(AbilityInstance ability, WeaponType type)
        {
            Collider2D[] colliders = FilterResults(SwingWeapon());

            if (colliders == null || colliders.Length == 0) { return; }
            else
            {
                HitEnemies(GetDamageInstance(ability, type), ability.IsSingleTarget, colliders);

                // subtract weapon durability
                _useWeaponSO.InvokeEvent(type);
            }
        }

        // method overload to pass weapon index
        protected void RangedAttack(AbilityInstance ability, WeaponType type)
        {
            // add weapon damage to ability
            DamageInstance damageInstance = GetDamageInstance(ability, type);
            FireProjectile(ability, damageInstance);
        }
    }
}
