
using Scenes;
using System;
using UnityEngine.Events;

namespace Characters.StateMachine
{
    public class MobStateMachine : StateMachineBase
    {
        public UnityAction OnMobDeath;
        private MobData _mob;

        public override void Init()
        {
            _mob = GetComponentInParent<MobData>();
            foreach (StateType type in StateDict.Keys)
            {
                if (StateDict[type] != null)
                {
                    StateDict[type].Init(_mob);
                    StateDict[type].SetEnabled(false);
                }
            }

            // set spawn point to current position in return state if possible
            if (StateDict[StateType.Return] is ReturnStateMob returnState)
            {
                if (returnState != null) { returnState.SetSpawn(transform); }
            }

            // switch to starting state
            CurrentState = StartingState;
            StateDict[CurrentState].SetEnabled(true);
        }

        private void Start()
        {
            Init();
            _mob.Health.DeathSO.OnInvokeEvent += ProcessDeath;
        }
        private void OnDisable()
        {
            _mob.Health.DeathSO.OnInvokeEvent -= ProcessDeath;
        }

        public void SetPatrol(int index)
        {
            if (StateDict[StateType.Patrol] is PatrolStateMob patrol)
            {
                if (patrol != null) { patrol.SetRoute(SceneRefManager.Instance.MobSpawns[index]); }
            }
        }
    }
}
