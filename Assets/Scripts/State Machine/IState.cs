using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.StateMachine
{
    public enum StateType { Previous = -1, Idle = 0, Patrol = 1, Combat, Return, Death, GamePause }
    public struct Transition
    {
        public StateType NewState;
        public Func<bool> Condition;
        public Transition(StateType newState, Func<bool> condition)
        {
            NewState = newState;
            Condition = condition;
        }
    }

    public interface IState
    {
        [HideInInspector] public List<Transition> Transitions { get; }
        public ICharacter Character { get; }
        public void Init(ICharacter character);
        public void SetEnabled(bool enabled);
        public void ResetState() { }
    }
}