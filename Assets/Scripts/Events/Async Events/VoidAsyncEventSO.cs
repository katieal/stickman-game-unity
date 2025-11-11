using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "VoidAsyncEventSO", menuName = "Events/Async/Void Async Event")]
    public class VoidAsyncEventSO : BaseAsyncEventSO
    {
        public UnityAction OnStartEvent;
        public UnityAction OnEndEvent;

        public void StartEvent()
        {
            OnStartEvent?.Invoke();
        }

        public void EndEvent()
        {
            IsComplete = true;
            OnEndEvent?.Invoke();

            // reset event
            ResetFlagAsync().Forget();
        }
    }
}
