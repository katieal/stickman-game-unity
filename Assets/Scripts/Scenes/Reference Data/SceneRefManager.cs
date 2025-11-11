using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

using EventChannel;

namespace Scenes
{
    public enum Location { Null = -1, Main, Left, Right, Top, Bottom, Door1, Door2, Door3, Door4, Spawn };

    [Serializable]
    public struct Entrance
    {
        public Location Location;
        public Transform Transform;
    }

    public class SceneRefManager : MonoBehaviour
    {
        [BoxGroup("Spawn Points", CenterLabel = true)]
        [TitleGroup("Spawn Points/Scene Entrances")]
        public List<Entrance> Entrances;
        [TitleGroup("Spawn Points/Character Spawns")]
        [Tooltip("Gameobject holding all npc spawn points.")]
        public GameObject CharacterSpawnPoints;
        [TitleGroup("Spawn Points/Mob Spawns")]
        [Tooltip("Gameobject holding all mob spawn points.")]
        public GameObject MobSpawnPoints;

        public SpawnPoint[] CharacterSpawns { get; private set; }
        public SpawnPoint[] MobSpawns { get; private set; }


        // singleton reference
        private static SceneRefManager _instance;
        public static SceneRefManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                // if instance isn't this, destroy old instance
                DestroySelf();
                Init();
            }
            else
            {
                Init();
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            DestroySelf();
        }

        private void Init()
        {
            _instance = this;

            CharacterSpawns = CharacterSpawnPoints.GetComponentsInChildren<SpawnPoint>();
            MobSpawns = MobSpawnPoints.GetComponentsInChildren<SpawnPoint>();
        }

        public Transform GetTransform(Location location)
        {
            foreach (Entrance entrance in Entrances)
            {
                if (entrance.Location == location) { return entrance.Transform; }
            }

            Debug.Assert(false, "Entrance location not found!");
            return null;
        }

        /// <summary>
        /// Finds the entrance location that is the shortest distance away from position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Location GetClosestLocation(Vector3 position)
        {
            // init variables with the first entry in entrance list
            Location closestLocation = Entrances[0].Location;
            float distance = Vector3.Distance(position, Entrances[0].Transform.position);

            // find the entrance location closest to position
            foreach (Entrance entrance in Entrances)
            {
                // replace vars if a shorter distance is found
                if (Vector3.Distance(position, entrance.Transform.position) < distance)
                {
                    closestLocation = entrance.Location;
                    distance = Vector3.Distance(position, entrance.Transform.position);
                }
            }

            return closestLocation;
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
            _instance = null;

        }
    }
}