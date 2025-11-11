using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;


namespace Characters
{
    public class MoveLogic : SerializedMonoBehaviour
    {

        /// <summary>
        /// This event is raised when character reaches the designated point
        /// in the MoveToPoint() method.
        /// </summary>
        public UnityAction OnPointReached;

        [ShowInInspector] public float MoveSpeed { get; protected set; }

        /// <summary>
        /// The angle in degrees of the character's movement relative to Vector2.left
        /// </summary>
        public float MoveAngle { get; protected set; }

        /// <summary>
        /// The target location of MoveToPoint()
        /// </summary>
        protected Vector3 _targetPoint;

        protected float _distanceVariance;

        /// <summary>
        /// Character cannot move if it is frozen.
        /// </summary>
        protected bool _isFrozen = false;

        /// <summary>
        /// Bool to track if the MoveCoroutine coroutine is running.
        /// </summary>
        protected bool _isMovingToPoint = false;

        /// <summary>
        /// Track MoveToPoint Coroutine
        /// </summary>
        protected Coroutine _moveToPointCoroutine;

        protected ICharacter _character;


        protected void Start()
        {
            _character = gameObject.GetComponent<ICharacter>();
            MoveSpeed = _character.MoveSpeed;
        }

        public void SetSpeed(float speed) { MoveSpeed = speed; }
        public void Freeze(bool isFrozen) 
        { 
            _isFrozen = isFrozen; 
            if (_isFrozen) { StopMoving(); } 
        }

        protected virtual void StartMoving()
        {
            _character.Animation.Walk(true);
            _character.Audio.PlayFootsteps(true);
        }

        public virtual void StopMoving()
        {
            if (_moveToPointCoroutine != null) { StopCoroutine(_moveToPointCoroutine); }
            _character.RigidBody.linearVelocity = Vector2.zero;
            _character.Animation.Walk(false);
            _character.Audio.PlayFootsteps(false);
        }

        /// <summary>
        /// Move mob to specified point. The coroutine ends once mob is within the specified distance of the point. 
        /// Call with zero for the lowest margin of error.
        /// </summary>
        /// <param name="point">The point to travel to</param>
        /// <param name="distanceVariance">The acceptable margin of error</param>
        public void MoveToPoint(Vector3 point, float distanceVariance = 0.1f)
        {
            if (_isFrozen) { return; }

            // update target point
            _targetPoint = point;
            // update variance
            _distanceVariance = distanceVariance;
            // update facing angle
            MoveAngle = Mathf.Atan2(transform.position.y - point.y, transform.position.x - point.x) * Mathf.Rad2Deg;

            // if coroutine isn't already running, start it
            if (!_isMovingToPoint) 
            { 
                StartMoving();
                _moveToPointCoroutine = StartCoroutine(MoveToPointCoroutine());
            }
        }

        /// <summary>
        /// Coroutine to move char to _targetPoint. _targetPoint and _distanceVariance can change without stopping
        /// the coroutine.
        /// </summary>
        /// <param name="variance"></param>
        /// <returns></returns>
        protected IEnumerator MoveToPointCoroutine()
        {
            if (_isFrozen) { yield break; }

            // indicate coroutine is running
            _isMovingToPoint = true;

            while (transform.position != _targetPoint || Vector2.Distance(transform.position, _targetPoint) < _distanceVariance)
            {
                // stop if character gets frozen
                if (_isFrozen) 
                {
                    _isMovingToPoint = false;
                    yield break; 
                }

                // step towards point
                float step = MoveSpeed * Time.deltaTime;
                _character.RigidBody.MovePosition(Vector2.MoveTowards(transform.position, _targetPoint, step));

                // flip body to face the correct direction
                if (_targetPoint.x < transform.position.x)
                {
                    _character.Position.localScale = new Vector2(-1, _character.Position.localScale.y);
                }
                else { _character.Position.localScale = new Vector2(1, _character.Position.localScale.y); }

                // wait for fixed update
                yield return new WaitForFixedUpdate();
            }

            // once character reaches point
            _isMovingToPoint = false;
            OnPointReached.Invoke();
            StopMoving();
        }



        /// <summary>
        /// Makes character move at the given velocity.
        /// When finished moving, call Walk(Vector2.zero) or StopMoving() to stop moving.
        /// </summary>
        /// <param name="velocity"></param>
        //public virtual void Walk(Vector2 velocity)
        //{
        //    if (_isFrozen) { return; }

        //    // if movement is starting
        //    if (_character.RigidBody.linearVelocity == Vector2.zero && velocity != Vector2.zero)
        //    {
        //        StartMoving();
        //    }

        //    // if movement is stopping
        //    if (velocity == Vector2.zero && _character.RigidBody.linearVelocity != Vector2.zero)
        //    {
        //        StopMoving();
        //    }

        //    _character.RigidBody.linearVelocity = velocity * MoveSpeed;

        //    // flips sprite based on direction it moves
        //    //if (Mathf.Abs(_entity.RigidBody.linearVelocity.x) > Mathf.Epsilon) {}
        //}
    }
}