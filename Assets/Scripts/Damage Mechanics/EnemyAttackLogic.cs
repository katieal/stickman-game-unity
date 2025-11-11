using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;

using Characters;

/*
 * 
 * TODO: 
 *  - test angled attacks
 *  - rework interactions between this and detection logic regarding enemy tracking
 *  - implement ability prio
 *  - add capacity to handle multiple abilities
 *  - add ranged attack functions
 * 
 */

namespace Combat
{
    public class EnemyAttackLogic : AttackLogic
    {
        //[Title("Mob Fields")]
        //[Tooltip("Mob can attack enemies within this collider. Should be located on this object.")]
        //[SerializeField] protected CircleCollider2D _attackRangeCollider;

        [Title("Debug Only")]
        [Tooltip("Can the entity attack? (Serialized for debug)")]
        [SerializeField] private bool _canAttack = true;
        [SerializeField] private bool _showAttackGizmos = false;

        // array to hold abilities
        private AbilityInstance[] _abilities;

        // mob's data script
        private MobData _mobData;

        // serialized for debug
        [field: SerializeField] public bool HasEnemyInRange { get; private set; } = false;
        public bool IsAttacking { get; private set; } = false;

        public AbilityInstance CurrentAbility;
        /// <summary>
        /// the timestamp when the mob can next attack
        /// </summary>
        private float _attackTime;

        // track when windup animation finishes
        private Animation.AnimationStateEventLogic _animationEvent;
        private bool _animationFlag = false;

        //// track one enemy at a time
        //private Collider2D _target;
        //// angle between self and target
        //private float _angle;

        private void Awake()
        {
            _mobData = GetComponentInParent<MobData>();

            //List<AbilitySettingsSO> abilitySettings = _mobData.Settings.Abilities;

            // initialize abilities array
            //_abilities = new AbilityInstance[abilitySettings.Count];
            //for (int i = 0; i < abilitySettings.Count; i++)
            //{
            //    if (abilitySettings[i] is MeleAbilitySettingsSO mele)
            //    {
            //        _abilities[i] = new MeleAbility(mele);
            //    }
            //    else if (abilitySettings[i] is RangedAbilitySettingsSO ranged)
            //    {
            //        _abilities[i] = new RangedAbility(ranged);
            //    }
            //}

            // turn off attack collider
           // _attackRangeCollider.enabled = false;

            // initialize attack time
            _attackTime = Time.time;

            _animationEvent = _mobData.Animator.GetBehaviour<Animation.AnimationStateEventLogic>();
        }

        private void OnEnable()
        {
            if (!_canAttack) { this.enabled = false; return; }
            //_attackRangeCollider.enabled = true;
        }
        private void OnDisable()
        {
            //_attackRangeCollider.enabled = false;
            StopAllCoroutines();
        }

        private void Update()
        {
            // limit mob attack speed
            if (_attackTime <= Time.time && HasEnemyInRange)
            {
                // ability order in list determines priority
                for (int index = 0; index < _abilities.Length; index++)
                {
                    if (_abilities[index].Cooldown.End < Time.time)
                    {
                        _attackTime += _mobData.Settings.AttackSpeed;
                        StartCoroutine(Attack(index));
                        break;
                    }
                }
            }

        }

        IEnumerator Attack(int index)
        {
            //// if has cast animation, trigger it and wait for animation to finish
            //if (ability.HasCastAnimation)
            //{
            //    _animationEvent.InvokeEvent(ability.CastAnimationTrigger);
            //    // wait for animation
            //    yield return new WaitUntil(() => _animationFinished);
            //    // reset flag
            //    _animationFinished = false;

            //    // TODO: IMPLEMENT!!!!!!!!!!!!!!!!!!!
            //    //_animationEvent.InvokeEvent(ability.AnimationTrigger);
            //}
            //// otherwise, trigger regular animation and wait for cast seconds
            //else
            //{
            //    _animationEvent.InvokeEvent(ability.AnimationTrigger);
            //    yield return new WaitForSeconds(ability.AttackDelay);
            //}

            //// execute ability based on type
            //switch (ability.Type)
            //{
            //    case AbilityType.Mele: MeleAttack(ability, type); break;
            //    case AbilityType.Ranged:
            //        RangedAttack(ability, type);
            //        // subtract durability after casting
            //        _useWeaponSO.InvokeEvent(type);
            //        break;
            //    case AbilityType.Charge: break;
            //    case AbilityType.Spell: break;
            //}


            //Debug.Log("winding up!");
            // trigger windup animation
            _mobData.Animator.SetTrigger(_abilities[index].AnimationTrigger);
            _abilities[index].Cooldown.Begin();

            _animationEvent.OnAnimationFinishedEvent += AnimationFlag;

            while (_animationFlag == false) { yield return null; }

            ////Debug.Log("windup animation finished!");
            //if (_abilities[index] is MeleAbility mele)
            //{
            //    MeleAttack(mele);
            //}
            //else if (_abilities[index] is RangedAbility ranged)
            //{
            //    RangedAttack(ranged);
            //}

            // reset flag
            _animationFlag = false;
        }

        private void AnimationFlag()
        {
            _animationFlag = true;
            _animationEvent.OnAnimationFinishedEvent -= AnimationFlag;
        }

        // get all colliders within attack range
        protected override Collider2D[] SwingWeapon()
        {
            //Collider2D[] colliders = Physics2D.OverlapCapsuleAll(transform.position +
            //    new Vector3(_attackAreaGuide.offset.x, _attackAreaGuide.offset.y), _attackAreaGuide.size,
            //    _attackAreaGuide.direction, _angle, LayerMask.GetMask("HitBox"));

            // TODO: TEST
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll(transform.position +
                new Vector3(_attackAreaGuide.offset.x, _attackAreaGuide.offset.y), _attackAreaGuide.size,
                _attackAreaGuide.direction, _mobData.MoveLogic.MoveAngle, LayerMask.GetMask("HitBox"));

            return colliders;
        }


        // attack state in SM will constantly try to move collider so it touches an enemy
        //private void OnTriggerEnter2D(Collider2D collider)
        //{
        //    if (_target == null && _enemyTags.Contains(collider.tag)) 
        //    {
        //        _target = collider;
        //        HasEnemyInRange = true;
        //    }
        //}

        //private void OnTriggerStay2D(Collider2D collider)
        //{
        //    if (collider != _target) { return; }

        //    // calculate angle in degrees between self and enemy
        //    // used to correctly angle physics2d overlapcapsule call
        //    Vector3 direction = collider.transform.position - this.transform.position;
        //    _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //}

        //private void OnTriggerExit2D(Collider2D collider)
        //{
        //    // check if this collider is still touching an enemy's hitbox before disabling
        //    if (collider == _target) 
        //    { 
        //        HasEnemyInRange = false;
        //        _target = null;
        //    }
        //}

        //private void OnDrawGizmos()
        //{
        //    if (_showAttackGizmos)
        //    {
        //        // drawr sphere representing mob's attack range
        //        Gizmos.DrawWireSphere(this.transform.position, _attackRangeCollider.radius);

        //        // draw line representing how the overlapcapsule call will be angled
        //        Gizmos.color = Color.red;
        //        Vector3 point2 = transform.position + (Quaternion.Euler(0, 0, _angle) * new Vector2(_attackAreaGuide.size.x / 2, 0));
        //        Gizmos.DrawLine(transform.position, point2);
        //    }
        //}
    }
}

