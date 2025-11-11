using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    public class BaseSingleEventSO : ScriptableObject
    {
#if UNITY_EDITOR
        [Title("Event Info")]
        [PropertyOrder(1)][TextArea(3, 10)] public string Description;

        [Title("Event Users")]
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(2)] public List<string> InvokedBy;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(3)] public List<string> Listeners;
#endif
    }

    public abstract class GenericSingleEventSO<T> : BaseSingleEventSO 
    {
#if UNITY_EDITOR
        [PropertyOrder(1.5f)] public string InvokeWith;
#endif

        public UnityAction<T> OnInvokeEvent;

        public void InvokeEvent(T data)
        {
            OnInvokeEvent?.Invoke(data);
        }
    }

    public abstract class GenericGenericSingleEventSO<T0, T1> : BaseSingleEventSO
    {
#if UNITY_EDITOR
        [PropertyOrder(1.5f)] public string InvokeWith;
#endif

        public UnityAction<T0, T1> OnInvokeEvent;

        public void InvokeEvent(T0 arg0, T1 arg1)
        {
            OnInvokeEvent?.Invoke(arg0, arg1);
        }
    }
}