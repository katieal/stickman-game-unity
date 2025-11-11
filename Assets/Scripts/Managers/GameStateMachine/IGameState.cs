using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers.GameStateMachine
{
    public enum StateType { Menu, Run = 1, Pause = 2, Load = 3, Start, Exit }
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

    public interface IGameState
    {
        public List<Transition> Transitions { get; set; }
        public StateType Type { get; }
        public void SetEnabled(bool enabled);
    }
}
