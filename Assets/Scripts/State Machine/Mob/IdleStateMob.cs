using UnityEngine;

namespace Characters.StateMachine
{
    public class IdleStateMob : IdleStateBase
    {

        // data
        private MobData _mob;


        #region Interface Methods
        public override void Init(ICharacter character)
        {
            base.Init(character);
            _mob = character as MobData;
        }
        public override void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            _mob.DetectionLogic.enabled = false;
        }

    }
}