using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "GameStateRequestEventSO", menuName = "Events/Request/GameState")]
    public class GameStateRequestEventSO : GenericGenericRequestEventSO<Managers.GameStateMachine.StateType, Managers.GameStateMachine.StateType> { }
}