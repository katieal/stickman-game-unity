using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "PlayerStaticDataEventSO", menuName = "Events/Single/PlayerStaticData")]
    public class PlayerStaticDataEventSO : GenericSingleEventSO<SaveData.PlayerStaticData> { }
}