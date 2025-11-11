using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class NewDetectionLogic : MonoBehaviour
    {

        [Tooltip("The parent rigid body")]
        [SerializeField] private Rigidbody2D _parentBody;
        [SerializeField] private CircleCollider2D _detectionCollider;

        [Tooltip("Tags of enemies that can be damaged")]
        [SerializeField] private List<string> _tagsToHit;

        public UnityAction<Vector2> EnemyPositionEvent;
        public UnityAction EnemyFoundEvent;
        public UnityAction EnemyLostEvent;


        private ContactFilter2D _contactFilter;

        // the hitbox this enemy is currently targeting
        private Collider2D _target;

        // number of seconds in between updates for searching and tracking
        private readonly float _updateFrequency = 0.2f;

        // raycast frequency
        private Timer.FixedTimer _searchTimer;
        private Timer.FixedTimer _trackTimer;




        private void Awake()
        {
            _contactFilter = new ContactFilter2D();
            _contactFilter.layerMask = LayerMask.GetMask("Foreground, HitBox, GameBorder");
            _contactFilter.useLayerMask = true;

            
        }

        private void OnEnable()
        {

            _searchTimer = new(_updateFrequency);
            _trackTimer = new(_updateFrequency);

            //_trackTimer.OnTimerFinished += SendEnemyPosition;

            // start searching for enemies
            //_searchTimer.Start();
        }
        private void OnDisable()
        {

            //_trackTimer.OnTimerFinished -= SendEnemyPosition;
        }

        private void Update()
        {

        }


        #region Public Methods
        
        public void StartSearching()
        {
            _trackTimer.Stop();
            _searchTimer.Start();
        }

        public void StartTracking()
        {
            _searchTimer.Stop();
            _trackTimer.Start();
        }

        #endregion


        private bool CheckLineOfSight(Collider2D collider)
        {
            List<RaycastHit2D> colliders = new List<RaycastHit2D>();
            //int hits = Physics2D.Linecast(this.transform.position, collider.transform.position, _layerMask);

            //return (hit != false && hit.collider == collider);

            // TODO: FIND A WAY TO RAYCAST WITHOUT HITTING SELF
            return false;
        }


        #region Searching

        // 
        public void OnSearchForEnemy()
        {
            
            // scan LOS and look for enemies
            // if found, raise onenemyfound event
            // otherwise, reset timer

            //_detectionCollider.
            
        }
        #endregion


        #region Tracking

        public void SendEnemyPosition()
        {

            // get enemy position
            // if found, broadcast OnEnemyPosition
            // if not, raise enemylostevent
            // in other script: if recieves enemylostevent for amount of time,
            //                  call onenemylost
        }

        public void OnEnemyLost()
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



        private void OnTriggerStay2D(Collider2D collider)
        {
            // will only execute when searching for a new target
            if (_searchTimer.Status == Timer.Status.Complete)
            {
                // focus on a single target at a time
                if (_target != null && collider != _target) { return; }

                // don't collide with self
                if (collider.attachedRigidbody == _parentBody) { return; }
                else if (_tagsToHit.Contains(collider.tag))
                {
                    if (CheckLineOfSight(collider))
                    {
                        // if valid target found, stop searching and start tracking
                        _target = collider;
                        _searchTimer.Stop();
                        _trackTimer.Start();
                        // initial call immediately after enemy is found
                        SendEnemyPosition();
                    }
                }
                _searchTimer.Restart();
            }


        }

    }
}