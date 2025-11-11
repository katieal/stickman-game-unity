using UnityEngine;

using EventChannel;

public class MainMenuUI : MonoBehaviour
{

    [Header("Invokes")]
    [SerializeField] private StringEventSO _sceneLoadRequestSO;

    public void OnStartButton()
    {
        _sceneLoadRequestSO.InvokeEvent("Village1_Scene");
    }
}
