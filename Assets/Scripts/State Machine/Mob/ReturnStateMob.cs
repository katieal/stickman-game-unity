using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.StateMachine
{
    // Basic return state implementation for mobs
    public class ReturnStateMob : ReturnStateBase
    {
        private MobData _mob;

        #region Interface Fields
        public override void Init(ICharacter character)
        {
            Character = character;
            _mob = character as MobData;

            Transitions.Add(new Transition(StateType.Idle, () => _hasReachedSpawn));
        }
        public override void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion

        private void FixedUpdate()
        {
            // check if body reached its spawn point
            if (Vector2.Distance(_mob.RigidBody.position, _spawn) < 0.5f)
            {
                _hasReachedSpawn = true;
            }

            // move body towards its spawn point
            float step = _mob.Settings.MoveSpeed * Time.deltaTime;
            _mob.RigidBody.MovePosition(Vector2.MoveTowards(transform.position, _spawn, step));

            // flip body to face the correct direction
            if (_spawn.x < transform.position.x)
            {
                _mob.Position.localScale = new Vector2(-1, _mob.Position.localScale.y);
            }
            else { _mob.Position.localScale = new Vector2(1, _mob.Position.localScale.y); }
        }
    }
}