using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationLoader : MonoBehaviour
{


    private void Start()
    {
        InitGame().Forget();

    }

    private async UniTaskVoid InitGame()
    {
        await UniTask.WhenAll(
            SceneManager.LoadSceneAsync("PersistentManagers", LoadSceneMode.Additive).ToUniTask(),
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive).ToUniTask());

        UnloadSelf();
    }
     
    private void UnloadSelf()
    {
        // unload self
        SceneManager.UnloadSceneAsync("Initialization");
    }
}
