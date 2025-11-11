using UnityEngine;
using UnityEngine.Events;

namespace Animation
{
    public class AnimationStateEventLogic : StateMachineBehaviour
    {
        public UnityAction OnAnimationFinishedEvent; 

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnAnimationFinishedEvent?.Invoke();
        }
    }
}