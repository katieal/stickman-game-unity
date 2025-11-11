using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "GameStateDataEventSO", menuName = "Events/Single/GameStateData")]
    public class GameStateDataEventSO : GenericSingleEventSO<SaveData.GameStateData> { }
}