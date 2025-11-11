using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    public class BaseUniTaskEventSO : ScriptableObject
    {

#if UNITY_EDITOR
        [Title("Info")]
        [PropertyOrder(1)][TextArea(3, 10)] 
        public string Description;

        [Title("Event Users")]
        [PropertyOrder(2)] public string TaskSource;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(3)] public List<string> StartInvokedBy;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(4)] public List<string> OnStartListeners;
        [ListDrawerSettings(ShowIndexLabels = false)]
        [PropertyOrder(5)] public List<string> ResultListeners;
#endif
    }

    public abstract class GenericVoidUniTaskEventSO<T> : BaseUniTaskEventSO
    {

#if UNITY_EDITOR
        [PropertyOrder(2.5f)] 
        public string InvokeStartWith;
#endif

        public UnityAction<T> OnStartEvent;
        private UniTaskCompletionSource UniTaskCS;
        public UniTask Task { get { return UniTaskCS.Task; } }

        /// <summary>
        /// Method to begin the event.
        /// </summary>
        /// <param name="param">Event arg</param>
        public void StartEvent(T param)
        {
            UniTaskCS = new UniTaskCompletionSource();
            OnStartEvent.Invoke(param);
        }

        public UniTask InvokeEventAsync(T param)
        {
            UniTaskCS = new UniTaskCompletionSource();
            OnStartEvent.Invoke(param);
            return UniTaskCS.Task;
        }

        public void CancelEvent()
        {
            UniTaskCS.TrySetCanceled();
        }

        /// <summary>
        /// Method to end the event.
        /// </summary>
        public void EndEvent()
        {
            UniTaskCS.TrySetResult();
        }
    }

    /// <summary>
    /// Call StartEvent() to start and EndEvent(args) to end
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VoidGenericUniTaskEventSO<T> : BaseUniTaskEventSO
    {

#if UNITY_EDITOR
        [PropertyOrder(2.5f)] public string InvokeResultWith;
#endif

        public UnityAction OnStartEvent;
        private UniTaskCompletionSource<T> UniTaskCS;
        public UniTask<T> Task { get { return UniTaskCS.Task; } }

        /// <summary>
        /// Method to begin the event.
        /// </summary>
        public void StartEvent()
        {
            UniTaskCS = new UniTaskCompletionSource<T>();
            OnStartEvent.Invoke();
        }

        public UniTask<T> InvokeEventAsync()
        {
            UniTaskCS = new UniTaskCompletionSource<T>();
            OnStartEvent.Invoke();
            return UniTaskCS.Task;
        }

        public void CancelEvent()
        {
            UniTaskCS.TrySetCanceled();
        }

        /// <summary>
        /// Method to end the event and broadcast result.
        /// </summary>
        /// <param name="result">Event result</param>
        public void EndEvent(T result)
        {
            UniTaskCS.TrySetResult(result);
        }
    }

    public abstract class GenericGenericUniTaskEventSO<T0, T1> : BaseUniTaskEventSO
    {

#if UNITY_EDITOR
        [PropertyOrder(2.5f)] public string InvokeStartWith;
        [PropertyOrder(2.6f)] public string InvokeResultWith;
#endif

        public UnityAction<T0> OnStartEvent;
        private UniTaskCompletionSource<T1> UniTaskCS;
        public UniTask<T1> Task { get { return UniTaskCS.Task; } }

        /// <summary>
        /// Method to start the event.
        /// </summary>
        /// <param name="param">Event arg</param>
        public void StartEvent(T0 param)
        {
            UniTaskCS = new UniTaskCompletionSource<T1>();
            OnStartEvent.Invoke(param);
        }

        public UniTask<T1> InvokeEventAsync(T0 param)
        {
            UniTaskCS = new UniTaskCompletionSource<T1>();
            OnStartEvent.Invoke(param);
            return UniTaskCS.Task;
        }

        public void CancelEvent()
        {
            UniTaskCS.TrySetCanceled();
        }

        /// <summary>
        /// Method to end the event and broadcast result.
        /// </summary>
        /// <param name="result">Event result</param>
        public void EndEvent(T1 result)
        {
            UniTaskCS.TrySetResult(result);
        }
    }
}