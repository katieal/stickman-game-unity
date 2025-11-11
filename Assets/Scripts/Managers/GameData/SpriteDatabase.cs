using Scenes;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GameData
{
    public class SpriteDatabase : SerializedMonoBehaviour
    {
        [field: Title("Components")]
        [field: SerializeField] public SpriteLibrary Sprites { get; private set; }

        #region Singleton
        // singleton reference
        private static SpriteDatabase _instance;
        public static SpriteDatabase Instance { get { return _instance; } }

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
    }
}
