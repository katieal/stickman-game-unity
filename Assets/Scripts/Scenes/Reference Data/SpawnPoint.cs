using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes
{
    public class SpawnPoint : MonoBehaviour
    {
        // patrol route
        // index 0 is spawn point
        [ChildGameObjectsOnly]
        public List <Transform> PatrolRoute = new();
        public Transform Spawn { get { return transform; } }

        #region Inspector
        private List<Color> _colors = new() { Color.red, new Color(255, 165, 0), Color.yellow, Color.green, Color.blue, Color.gray };

        private void OnDrawGizmosSelected()
        {
            if (PatrolRoute.Count > 0)
            {
                Gizmos.color = _colors[0];
                Gizmos.DrawLine(Spawn.position, PatrolRoute[0].position);
            }

            if (PatrolRoute.Count > 1)
            {
                for (int index = 0; index < PatrolRoute.Count - 1; index++)
                {
                    Gizmos.color = _colors[index + 1];
                    Gizmos.DrawLine(PatrolRoute[index].position, PatrolRoute[index + 1].position);
                }
            }
        }
        #endregion
    }
}
