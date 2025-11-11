using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class DetectionLogic : MonoBehaviour
    {
        [Tooltip("The parent rigid body")]
        [SerializeField] private Rigidbody2D _parentBody;
        [Tooltip("Mob's detection range. Should start enabled.")]
        [SerializeField] private CircleCollider2D _detectionCollider;
        //[Tooltip("Layers for enemies to hit")]
        //[SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Characters.MoveLogic _moveLogic;
        [Tooltip("Tags of enemies that can be damaged")]
        [SerializeField] private List<string> _tagsToHit;

        public UnityAction EnemyFoundEvent;
        public UnityAction EnemyLostEvent;
        public UnityAction<Vector2> EnemyPositionEvent;


        // the hitbox this enemy is currently targeting
        public Collider2D Target { get; private set; }

        // number of seconds in between updates for searching and tracking
        private readonly float _updateFrequency = 0.2f;

        private LayerMask _raycastMask;
        private LayerMask _hitBoxMask;
        private ContactFilter2D _hitBoxFilter;

        private void Awake()
        {
            _raycastMask = LayerMask.GetMask("HitBox", "Game Border", "Foreground");
            _hitBoxMask = LayerMask.GetMask("HitBox");
            _hitBoxFilter = ContactFilter2D.noFilter;
            _hitBoxFilter.layerMask = _hitBoxMask;
            _hitBoxFilter.useLayerMask = true;
        }

        private void OnEnable()
        {
            _detectionCollider.enabled = true;
        }

        private void OnDisable()
        {
            _detectionCollider.enabled = false;
        }

        #region Searching
        public void StartSearching()
        {
            // cancel any current tracking
            CancelInvoke(nameof(TrackEnemy));
            Target = null;

            // start searching for enemies
            InvokeRepeating(nameof(SearchForEnemy), 0f, _updateFrequency);
        }

        private void SearchForEnemy()
        {
            List<Collider2D> results = new List<Collider2D>();

            // check if detection collider overlaps with any hitboxes
            int num = _detectionCollider.Overlap(_hitBoxFilter, results);

            if (num > 0)
            {
                foreach (Collider2D collider in results)
                {
                    // check if collider belongs to enemy and is within line of sight
                    if (_tagsToHit.Contains(collider.tag) && CheckLineOfSight(collider))
                    {
                        // if yes, target this collider and send enemy found event
                        Target = collider;
                        EnemyFoundEvent.Invoke();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Check if collider is within cone of vision and not blocked by obstacles
        /// </summary>
        /// <param name="collider"></param>
        /// <returns>True if self can see this collider, false if not.</returns>
        private bool CheckLineOfSight(Collider2D collider)
        {
            // check if enemy is behind any obstacles
            RaycastHit2D hit = Physics2D.Linecast(this.transform.position, collider.transform.position, _raycastMask);
            // return false if raycast hits nothing or hits an obstacle
            if (hit == false || hit.collider != collider) { return false; }

            // Cone of Vision = character facing angle +-45 degrees
            // calculate angle between self and enemy collider
            float angle = Mathf.Rad2Deg * Mathf.Atan2(this.transform.position.y - collider.transform.position.y, 
                this.transform.position.x - collider.transform.position.x);
            // check if enemy angle is within 45 degrees of facing angle
            return Mathf.Abs(Mathf.DeltaAngle(_moveLogic.MoveAngle, angle)) <= 45;
        }
        #endregion

        #region Tracking
        public void StartTracking()
        {
            // cancel search if needed
            CancelInvoke(nameof(SearchForEnemy));
            // begin tracking
            InvokeRepeating(nameof(TrackEnemy), 0f, _updateFrequency);
        }

        private void TrackEnemy()
        {
            if (_detectionCollider.IsTouching(Target))
            {
                // pathfinding will be in combat state
                EnemyPositionEvent.Invoke(Target.transform.position);
            }
            else
            {
                // if enemy is not in detection range, raise enemylostevent
                EnemyLostEvent.Invoke();
            }
        }


        private void SendEnemyPosition()
        {
            // get enemy position
            // if found, broadcast OnEnemyPosition
            // if not, raise enemylostevent
            // in other script: if recieves enemylostevent for amount of time,
            //                  call onenemylost


            // if target is in LOS, send its position and keep tracking its position
            if (CheckLineOfSight(Target))
            {
                EnemyPositionEvent.Invoke(Target.transform.position);
                //_trackTimer.Reset();
            }
            else
            {
                // if enemy isn't in line of sight, stop tracking and start searching
                //EnemyPositionEvent.Invoke(null);
                //_trackTimer.Stop();
                //_searchTimer.Restart();
            }
        }

        private void OnEnemyLost()
        {
            // (other script): stop moving
            //                turn in all four directions
            // raycast with same method as search for enemy
            // this should cover entire area
            // if enemy found, raise onenemyfound
            // (other script): if recieves enemyfoundevent, call start tracking
            //                  otherwise, call start searching and return to patrol route
        }
        #endregion

        private void Reset()
        {
            CancelInvoke();
            Target = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (_detectionCollider != null) Gizmos.DrawWireSphere(this.transform.position, _detectionCollider.radius);
        }
    }
}