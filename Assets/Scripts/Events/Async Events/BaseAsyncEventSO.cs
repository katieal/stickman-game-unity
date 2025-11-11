using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    public class BaseAsyncEventSO : ScriptableObject
    {
        [Title("Info")]
        [PropertyOrder(1)][TextArea(3, 10)] public string Description;

        [Title("Event Users")]
        [PropertyOrder(2)] public string EventSource;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(3)] public List<string> StartInvokedBy;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(4)] public List<string> OnStartListeners;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(5)] public List<string> OnEndListeners;

        public bool IsComplete { get; protected set; } = false;


        protected async UniTaskVoid ResetFlagAsync()
        {
            await UniTask.WaitForSeconds(1, ignoreTimeScale: true);
            IsComplete = false;
        }
    }

    public abstract class GenericVoidAsyncEventSO<T> : BaseAsyncEventSO
    {
        [PropertyOrder(2.5f)] public string InvokeStartWith;

        public UnityAction<T> OnStartEvent;
        public UnityAction OnEndEvent;

        public void StartEvent(T param)
        {
            OnStartEvent?.Invoke(param);
        }

        public void EndEvent()
        {
            IsComplete = true;
            OnEndEvent?.Invoke();

            // reset event
            ResetFlagAsync().Forget();
        }
    }

    public abstract class GenericGenericAsyncEventSO<T0, T1> : BaseAsyncEventSO
    {
        [PropertyOrder(2.5f)] public string InvokeStartWith;
        [PropertyOrder(2.5f)] public string InvokeEndWith;

        public UnityAction<T0> OnStartEvent;
        public UnityAction<T1> OnEndEvent;

        public void StartEvent(T0 param)
        {
            OnStartEvent?.Invoke(param);
        }

        public void EndEvent(T1 param)
        {
            IsComplete = true;
            OnEndEvent?.Invoke(param);

            // reset event
            ResetFlagAsync().Forget();
        }
    }
}