using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "VoidUniTaskEventSO", menuName = "Events/UniTask/Void")]
    public class VoidUniTaskEventSO : BaseUniTaskEventSO
    {
        public UnityAction OnStartEvent;
        private UniTaskCompletionSource UniTaskCS;
        public UniTask Task { get { return UniTaskCS.Task; } }

        public void StartEvent()
        {
            UniTaskCS = new UniTaskCompletionSource();
            OnStartEvent.Invoke();
        }

        public void CancelEvent()
        {
            UniTaskCS.TrySetCanceled();
        }

        public void EndEvent()
        {
            UniTaskCS.TrySetResult();
        }
    }
}