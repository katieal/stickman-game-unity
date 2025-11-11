using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Source: https://onewheelstudio.com/blog/2020/12/27/c-generics-and-unity
 */

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        public GameObject myPrefab;

        private void OnEnable()
        {
            // Pool<MyType>.AssignPrefab(myPrefab);
        }

        private GameObject CreateObject()
        {
            //return Pool<MyType>.GetFromPool().gameObject;
            return null;
        }
    }

    public static class Pool<T> where T : MonoBehaviour
    {
        private static Queue<T> objectQueue = new Queue<T>();
        private static GameObject prefab;

        public static void AssignPrefab(GameObject prefab)
        {
            Pool<T>.prefab = prefab;
        }

        public static void ReturnToPool(T _object)
        {
            objectQueue.Enqueue(_object);
            _object.gameObject.SetActive(false);
        }

        public static T GetFromPool()
        {
            if (objectQueue.Count > 0)
            {
                return objectQueue.Dequeue();
            }
            return GameObject.Instantiate(prefab).GetComponent<T>();
        }
    }
}