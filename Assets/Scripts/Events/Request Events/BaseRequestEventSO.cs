using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    public class BaseRequestEventSO : ScriptableObject
    {
        [Title("Info")]
        [PropertyOrder(1)][TextArea(3, 10)] public string Description;

        [Title("Event Data")]
        [PropertyOrder(2)] public string EventResponder;

        [Title("Event Users")]
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(3)] public List<string> InvokedBy;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(4)] public List<string> ResultListeners;
    }

    public abstract class VoidGenericRequestEventSO<T> : BaseRequestEventSO
    {
        [PropertyOrder(2.5f)] public string InvokeResultWith;

        public UnityAction OnRequestEvent;
        public UnityAction<T> OnResultEvent;

        public void RequestEvent()
        {
            OnRequestEvent?.Invoke();
        }
        public void SendResult(T result)
        {
            OnResultEvent?.Invoke(result);
        }
    }

    public abstract class GenericGenericRequestEventSO<T0, T1> : BaseRequestEventSO
    {
        [PropertyOrder(2.3f)] public string InvokeRequestWith;
        [PropertyOrder(2.5f)] public string InvokeResultWith;

        public UnityAction<T0> OnRequestEvent;
        public UnityAction<T1> OnResultEvent;

        public void RequestEvent(T0 input)
        {
            OnRequestEvent?.Invoke(input);
        }
        public void SendResult(T1 result)
        {
            OnResultEvent?.Invoke(result);
        }
    }

    public abstract class FourGenericRequestEventSO<T0, T1, T2, T3> : BaseRequestEventSO
    {
        [PropertyOrder(2.3f)] public string InvokeRequestWith;
        [PropertyOrder(2.5f)] public string InvokeResultWith;

        public UnityAction<T0, T1> OnRequestEvent;
        public UnityAction<T2, T3> OnResultEvent;

        public void RequestEvent(T0 arg0, T1 arg1)
        {
            OnRequestEvent?.Invoke(arg0, arg1);
        }
        public void SendResult(T2 arg2, T3 arg3)
        {
            OnResultEvent?.Invoke(arg2, arg3);
        }
    }
}