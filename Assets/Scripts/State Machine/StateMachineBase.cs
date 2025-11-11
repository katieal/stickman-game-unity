using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Characters.StateMachine
{
    public abstract class StateMachineBase : SerializedMonoBehaviour
    {
        [field: Title("State Machine Fields")]
        [field: SerializeField] [ChildGameObjectsOnly(IncludeSelf = true)]
        public Dictionary<StateType, IState> StateDict { get; protected set; } = new()
        {
            { StateType.Idle, null },
            { StateType.Patrol, null },
            { StateType.Combat, null },
            { StateType.Return, null },
            { StateType.Death, null },
        };
        protected StateType StartingState;
        public StateType CurrentState { get; protected set; }
        public StateType PreviousState { get; protected set; }

        public abstract void Init();

        protected virtual void Update()
        {
            if (StateDict[CurrentState] == null || StateDict[CurrentState].Transitions == null) { return; }

            foreach (Transition transition in StateDict[CurrentState].Transitions)
            {
                if (transition.Condition())
                {
                    ChangeState(transition.NewState);
                    break;
                }
            }
        }

        protected void ChangeState(StateType newState)
        {
            // exit current state
            StateDict[CurrentState].SetEnabled(false);

            // determine new state
            if (newState == StateType.Previous) { CurrentState = PreviousState; }
            else
            {
                PreviousState = CurrentState;
                CurrentState = newState;
            }

            #region Debug
            Debug.Assert(StateDict[CurrentState] != null, "State not found!!");
            #endregion

            StateDict[CurrentState].SetEnabled(true);
        }

        public void GetHit(Combat.DamageInstance damageInstance)
        {
            // change to combat state if not already
            if (CurrentState != StateType.Combat) { ChangeState(StateType.Combat); }
            // take kb/stagger/stun
            (StateDict[CurrentState] as CombatStateBase).GetHit(damageInstance); // double check this works!!!
        }

        protected void ProcessDeath()
        {
            ChangeState(StateType.Death);
        }
    }
}
