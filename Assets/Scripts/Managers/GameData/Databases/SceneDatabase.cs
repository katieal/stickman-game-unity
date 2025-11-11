using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scenes;

namespace GameData 
{ 
    public class SceneDatabase : SerializedMonoBehaviour
    {
        [Title("Components")]
        [AssetList]
        [SerializeField] private List<SceneSettingsSO> _sceneDatabase = new List<SceneSettingsSO>();

        #region Singleton
        // singleton reference
        private static SceneDatabase _instance;
        public static SceneDatabase Instance { get { return _instance; } }

        private void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion


        public SceneSettingsSO GetSceneSettings(SceneName name)
        {
            return _sceneDatabase.Find(x => x.SceneName == name);
        }

        public SceneSettingsSO GetSceneSettings(string name)
        {
            return _sceneDatabase.Find(x => x.SceneName.ToString() == name);
        }

        public string GetDisplayName(SceneName name)
        {
            return GetSceneSettings(name).DisplayName;
        }

        public string GetDisplayName(string name)
        {
            return GetSceneSettings(name).DisplayName;
        }


        public static SceneName GetCurrentSceneName()
        {
            string name = SceneManager.GetActiveScene().name;

            if (Enum.TryParse<SceneName>(name, out SceneName sceneName))
            {
                return sceneName;
            }
            else { return SceneName.None; }
        }
    }
}