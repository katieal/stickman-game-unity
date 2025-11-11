using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "VoidVoidRequestEventSO", menuName = "Events/Request/VoidVoidRequestEvent")]
    public class VoidVoidRequestEventSO : BaseRequestEventSO
    {
        public UnityAction OnRequestEvent;
        public UnityAction OnResultEvent;

        public void RequestEvent()
        {
            OnRequestEvent?.Invoke();
        }

        public void SendResult()
        {
            OnResultEvent?.Invoke();
        }
    }
}
