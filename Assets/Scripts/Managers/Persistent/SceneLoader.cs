using Cysharp.Threading.Tasks;
using EventChannel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public class SceneLoader : SerializedMonoBehaviour
    {
        [Title("Invoked Events")]
        [SerializeField] private SceneNameVoidUniTaskEventSO _movePlayerSceneEvent;
        //[SerializeField] private SceneName _respawnScene;

        [Title("Listening to Events")]
        [SerializeField] private SceneNameVoidUniTaskEventSO _loadNewSceneEvent;


        // should just call loadnewsceneevent instead of this - maybe add Respawn to a scene name to link it to a respawn scene
        // set in this script
        [SerializeField] private VoidUniTaskEventSO _respawnNextDayEvent;


        private void OnEnable()
        {
            //_respawnNextDayEvent.OnStartEvent += OnRespawnPlayer;
            _loadNewSceneEvent.OnStartEvent += OnLoadNewScene;
        }

        private void OnDisable()
        {
            //_respawnNextDayEvent.OnStartEvent -= OnRespawnPlayer;
            _loadNewSceneEvent.OnStartEvent -= OnLoadNewScene;
        }

        #region Event Callbacks
        private void OnLoadNewScene(SceneName name) { LoadNewSceneAsync(name).Forget(); }
        #endregion


        //private void OnRespawnPlayer()
        //{
        //    // respawn player at their bed
        //    LoadNewSceneAsync(_respawnScene, Location.Spawn).Forget();
        //}

        //private async UniTask ChangeScene(SceneName name)
        //{
        //    SceneSettingsSO newScene = SceneData.Instance.ScenesDict[name];

        //    // disallow scene activation


        //    await LoadGameplayAsync(newScene); // load gameplay scene if needed


        //    await LoadNewSceneAsync(newScene);

        //}

        // need to use this method for loading new scene and for respawning
        private async UniTask LoadNewSceneAsync(SceneName name)
        {
            SceneSettingsSO newScene = GameData.SceneDatabase.Instance.GetSceneSettings(name);
            Scene activeScene = SceneManager.GetActiveScene();

            await UniTask.WhenAll(
                SceneManager.UnloadSceneAsync(activeScene).ToUniTask(), // unload active scene
                LoadGameplayAsync(newScene), // load gameplay scene if needed
                                             // load new scene
                SceneManager.LoadSceneAsync(newScene.SceneName.ToString(), LoadSceneMode.Additive).ToUniTask()
            );

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene.SceneName.ToString()));

            // broadcast scene ready
            _loadNewSceneEvent.EndEvent();
        }

        //private async UniTask LoadNewSceneAsync(SceneName name)
        //{
        //    SceneSettingsSO settings = SceneData.Instance.ScenesDict[name];
        //    Scene oldScene = SceneManager.GetActiveScene();

        //    // load gameplay scene if needed
        //    await LoadGameplayAsync(settings);

        //    // load new scene - can use progress for loading screen in future
        //    await SceneManager.LoadSceneAsync(name.ToString(), LoadSceneMode.Additive).ToUniTask();
        //    Scene newScene = SceneManager.GetSceneByName(name.ToString());

        //    // ensure new scene requires player
        //    if (settings.Type == SceneType.Gameplay)
        //    {
        //        // switch player from old to new scene
        //        await _movePlayerSceneEvent.InvokeEventAsync(name);
        //    }

        //    // unload previous scene
        //    await SceneManager.UnloadSceneAsync(oldScene).ToUniTask(); 

        //    SceneManager.SetActiveScene(newScene);
        //    // broadcast scene ready
        //    _loadNewSceneEvent.EndEvent();
        //}

        private async UniTask LoadGameplayAsync(SceneSettingsSO scene)
        {
            // load gameplay scene if needed
            if (scene.Type == SceneType.Gameplay && !SceneManager.GetSceneByName("Gameplay").isLoaded)
            {
                await SceneManager.LoadSceneAsync(SceneName.Gameplay.ToString(), LoadSceneMode.Additive);
            }
            // if returning to the main menu, unload gameplay scene
            //else if (scene.Type == SceneType.MAINMENU)
            //{
            //    await SceneManager.UnloadSceneAsync("Gameplay");
            //}
        }
    }
}
