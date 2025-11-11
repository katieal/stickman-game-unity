using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using EventChannel;

namespace Scenes
{
    public class SceneTransitionLogic : MonoBehaviour, IInteractable
    {
        [SerializeField] private SceneName _nextScene;
        [SerializeField] private Location _entranceLocation;

        [Header("Invoked Events")]
        [SerializeField] private SceneNameLocationEventSO _changeSceneEvent;

        public void Interact()
        {
            // insert any transition effects or stuff here ?

            _changeSceneEvent.InvokeEvent(_nextScene, _entranceLocation);

            // prevent double clicking
            if (gameObject != null) { gameObject.SetActive(false); }
        }
    }
}
