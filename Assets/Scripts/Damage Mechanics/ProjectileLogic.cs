using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class ProjectileLogic : MonoBehaviour
    {
        //[Title("Info")]
        //public string Name;
        [Title("Components")]
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _lifespan = 5f;

        // variables from init
        private List<string> _tagsToHit;
        private DamageInstance _damage;

        // used to detect if projectile hits its parent
        private Rigidbody2D _parentBody;

        public UnityAction<DamageInstance, Collider2D> OnAttackConnectEvent;

        private void Start()
        {
            // destroy projectile after 5 seconds if it doesn't hit anything
            Destroy(gameObject, _lifespan);
        }

        public void Init(DamageInstance damage, float velocity, Rigidbody2D parentBody, List<string> tagsToHit)
        {
            _damage = damage;
            _parentBody = parentBody;

            _tagsToHit = tagsToHit;
            // arrow needs to fly in the correct direction
            _rigidbody.linearVelocity = transform.right * velocity;
        }

        // if collider isn't a trigger it causes the caster to get pushed back when
        // projectile is spawned
        public void OnTriggerEnter2D(Collider2D collider)
        {
            // not sure how to get a switch statement to work here
            //Debug.Log("Trigger entered");

            // prevent projectile from hitting self
            if (collider.attachedRigidbody == _parentBody) { return; }
            // destroy projectile if it comes in contact with environment
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Foreground"))
            {
                Destroy(gameObject);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Game Border"))
            {
                Destroy(gameObject);
            }
            // if projectile hits a hitbox and the hitbox belongs to an enemy
            else if (collider.gameObject.layer == LayerMask.NameToLayer("HitBox") && _tagsToHit.Contains(collider.tag))
            {
                //Debug.Log("target found");
                OnAttackConnectEvent.Invoke(_damage, collider);
                Destroy(gameObject);
            }
            // else, do nothing
        }


        private void OnDestroy()
        {
            OnAttackConnectEvent = null;
        }
    }
}
