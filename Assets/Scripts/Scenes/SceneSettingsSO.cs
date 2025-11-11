using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes
{
    public enum SceneType { Menu, Manager, Gameplay };
    public enum SceneName { None = -1, Gameplay, Start, Village, ShopInterior, InnInterior, SlimeFarm }

    [Serializable]
    public struct EnemySpawn
    {
        public Characters.MobType Type;
        [HideIf("Type", Characters.MobType.None)]
        public int MinSpawnCount;
        [HideIf("Type", Characters.MobType.None)]
        public int MaxSpawnCount;
    }

    [CreateAssetMenu(fileName = "SceneSettingsSO", menuName = "Scriptable Objects/Settings/Scene Settings")]
    public class SceneSettingsSO : ScriptableObject
    {
        public Scenes.SceneName SceneName;
        public Scenes.SceneType Type;
        [Tooltip("The scene's build index in build settings")]
        public int BuildIndex;

        [Tooltip("Name to be shown to user")]
        public string DisplayName;

        [Tooltip("Type of mob to spawn in the scene")]
        public EnemySpawn MobToSpawn;
    }
}
