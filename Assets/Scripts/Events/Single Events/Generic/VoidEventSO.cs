using UnityEngine;
using UnityEngine.Events;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "VoidEventSO", menuName = "Events/Single/Generic/Void Event")]
    public class VoidEventSO : BaseSingleEventSO
    {
        public UnityAction OnInvokeEvent;

        public void InvokeEvent()
        {
            OnInvokeEvent?.Invoke();
        }
    }
}