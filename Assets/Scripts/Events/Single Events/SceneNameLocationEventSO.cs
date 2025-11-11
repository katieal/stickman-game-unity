using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "SceneNameLocationEventSO", menuName = "Events/Single/SceneName Location")]
    public class SceneNameLocationEventSO : GenericGenericSingleEventSO<Scenes.SceneName, Scenes.Location> { }
}