using UnityEngine;

public class DoorData : MonoBehaviour, IInteractable
{
    //[SerializeField] private LoadSceneLogic _loadScene;
    private bool _isTriggered = false;

    public void Interact()
    {
        // triggers only once
        if (!_isTriggered)
        {
            _isTriggered = true;
            //_loadScene.LoadScene();
        }
    }
}
