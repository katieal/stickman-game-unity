using EventChannel;
using Sirenix.OdinInspector;
using UnityEngine;

public class BedData : MonoBehaviour, IInteractable
{
    [Title("Invoked Events")]
    [SerializeField] private VoidIntRequestEventSO _changeDaySO;

    public void Interact()
    {
        _changeDaySO.OnRequestEvent(); 
    }
}
