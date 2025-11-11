using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;


namespace Characters.StateMachine
{
    public class PatrolStateMob : PatrolStateBase
    {
        // data
        private MobData _mob;

        // mob spawn point
        [ShowInInspector] private Scenes.SpawnPoint _spawnPoint;
        // index of next point destination
        private int _index = 0;
        // transform of destination
        private Transform _nextPoint;

        // flags
        private bool _isPatrolFinished = false;
        private bool _isEnemyFound = false;

        #region Interface Methods
        public override void Init(ICharacter character)
        {
            Character = character;
            _mob = character as MobData;
            Transitions.Add(new Transition(StateType.Idle, () => _isPatrolFinished));
            Transitions.Add(new Transition(StateType.Combat, () => _isEnemyFound == true));
        }

        public override void SetEnabled(bool enabled) { this.enabled = enabled; }

        // reset all variables 
        public override void ResetState()
        {
            StopAllCoroutines();
            _mob.MoveLogic.StopMoving();
            _isPatrolFinished = false;
            _index = 0;
            _isEnemyFound = false;
        }
        #endregion

        public void SetRoute(Scenes.SpawnPoint route) { this._spawnPoint = route; }

        private void OnEnable()
        {
            if (_spawnPoint.PatrolRoute.Count > 0)
            {
                _mob.DetectionLogic.enabled = true;
                _mob.DetectionLogic.EnemyFoundEvent += OnEnemyFound;
                _nextPoint = _spawnPoint.PatrolRoute[0];

                // begin searching for enemies
                _mob.DetectionLogic.StartSearching();
                // start patrolling points
                StartPatrol();
            }
            else
            {
                // if mob has no patrol route, automatically exit patrol state
                _isPatrolFinished = true;
            }
        }

        private void OnDisable()
        {
            //if (_isEnemyFound != true) { _mob.DetectionLogic.enabled = false; }
            _mob.DetectionLogic.EnemyFoundEvent -= OnEnemyFound;
            ResetState();
        }

        private void OnEnemyFound()
        {
            _isEnemyFound = true;
        }

        // begin walking to next patrol point
        private void StartPatrol()
        {
            _mob.MoveLogic.MoveToPoint(_nextPoint.position);
            _mob.MoveLogic.OnPointReached += StartGuarding;
        }

        private void StartGuarding()
        {
            _mob.MoveLogic.OnPointReached -= StartGuarding;
            StartCoroutine(GuardPoint());
        }

        // stop for a specified amount of time at each patrol point
        private IEnumerator GuardPoint()
        {
            // if the destination was the original spawn point, exit patrol state
            if (_nextPoint.position.Equals(_spawnPoint.Spawn.position))
            {
                _isPatrolFinished = true;
                yield break;
            }

            // pause briefly at patrol point
            yield return new WaitForSeconds(5);

            // go to next patrol point
            _index += 1;

            // if all points have been patrolled, return to spawn point
            if (_spawnPoint.PatrolRoute.Count <= _index)
            {
                // reset index
                _index = 0;
                _nextPoint = _spawnPoint.Spawn;
            }
            else { _nextPoint = _spawnPoint.PatrolRoute[_index]; }

            // resume patrol
            StartPatrol();
        }
    }
}