using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

using EventChannel;

namespace Characters
{
    public class PlayerMoveLogic : MoveLogic
    {
        [Title("Invoked Events")]
        [SerializeField] private BoolEventSO _onMoveSO;

        [Title("Listening to events")]
        [SerializeField] protected Vector2EventSO _moveInputSO;
        [SerializeField] private GameStateRequestEventSO _gameStateChangedSO;

        /// <summary>
        /// Tracks if the player is currently being moved by setting velocity.
        /// </summary>
        private bool _isMovingWithVelocity = false;
        private bool _inputBuffer = false;

        private void OnEnable()
        {
            _moveInputSO.OnInvokeEvent += Walk;
            _gameStateChangedSO.OnResultEvent += OnGameStateChanged;
        }
        private void OnDisable()
        {
            _gameStateChangedSO.OnResultEvent -= OnGameStateChanged;
            _moveInputSO.OnInvokeEvent -= Walk;
            _moveInputSO.OnInvokeEvent -= InputBuffer;
        }

        protected override void StartMoving()
        {
            base.StartMoving();
            _onMoveSO.InvokeEvent(true);
        }

        public override void StopMoving()
        {
            base.StopMoving();
            _isMovingWithVelocity = false;
            _onMoveSO.InvokeEvent(false);
        }

        private void OnGameStateChanged(Managers.GameStateMachine.StateType newState)
        {
            if (newState == Managers.GameStateMachine.StateType.Pause || newState == Managers.GameStateMachine.StateType.Load)
            {
                StopMoving();
            }
        }

        public void Walk(Vector2 velocity)
        {
            if (_isFrozen) { return; }

            // if movement is starting
            if (!_isMovingWithVelocity && velocity != Vector2.zero)
            {
                StartMoving();
                _isMovingWithVelocity = true;
            }
            // if movement is stopping
            else if (velocity == Vector2.zero && _isMovingWithVelocity)
            {
                StartCoroutine(InputBufferCoroutine(7));
            }

            _character.RigidBody.linearVelocity = velocity * MoveSpeed;

            // flips sprite based on direction it moves
            //if (Mathf.Abs(_entity.RigidBody.linearVelocity.x) > Mathf.Epsilon) {}
        }


        /// <summary>
        /// This method will be subscribed to catch movement input if it ever reaches zero.
        /// If a specified number of frames pass and velocity is still zero, fully stop character movement
        /// and resubscribe to walk.
        /// If velocity changes while it is being tracked, simply resubscribe to walk without stopping
        /// character movement.
        /// </summary>
        /// <param name="velocity"></param>
        private void InputBuffer(Vector2 velocity)
        {
            if (velocity != Vector2.zero)
            {
                _inputBuffer = true;
                // start moving character while walk is being resubscribed
                _character.RigidBody.linearVelocity = velocity * MoveSpeed;
            }
        }

        /// <summary>
        /// Coroutine to add a buffer between no input and when the character stops moving.
        /// </summary>
        /// <param name="frames">Number of frames to wait before stopping movement.</param>
        /// <returns></returns>
        private IEnumerator InputBufferCoroutine(int frames)
        {
            _inputBuffer = false;
            _moveInputSO.OnInvokeEvent -= Walk;
            _moveInputSO.OnInvokeEvent += InputBuffer;

            for (int i = 0; i < frames; i++) 
            { 
                // resub to walk and end coroutine if player gets frozen
                if (_isFrozen) 
                {
                    _moveInputSO.OnInvokeEvent += Walk;
                    _moveInputSO.OnInvokeEvent -= InputBuffer;
                    yield break; 
                } 

                // if buffer is true, resubscribe to walk and continue moving
                if (_inputBuffer)
                {
                    _moveInputSO.OnInvokeEvent += Walk;
                    _moveInputSO.OnInvokeEvent -= InputBuffer;
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }

            // if all frames pass and buffer has not been changed to true, stop movement
            _moveInputSO.OnInvokeEvent += Walk;
            _moveInputSO.OnInvokeEvent -= InputBuffer;
            StopMoving();
        }
    }
}