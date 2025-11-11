using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "VoidPlayerDataRequestEventSO", menuName = "Events/Request/VoidPlayerData")]
    public class VoidPlayerDataRequestEventSO : VoidGenericRequestEventSO<SaveData.PlayerSaveData>  { }
}