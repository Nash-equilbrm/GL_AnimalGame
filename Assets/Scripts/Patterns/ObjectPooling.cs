using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Patterns
{
    public class ObjectPooling : Singleton<ObjectPooling>
    {
        [Serializable]
        public class ObjectPool
        {
            [Tooltip("Pool's name"), SerializeField]
            private string name;
            public string Name { get => name; }
            [Tooltip("The object to instantiate"), SerializeField]
            private GameObject prefab;
            [Tooltip("The pool of instantated objects"), SerializeField]
            private List<GameObject> pool = new List<GameObject>();


            /// <summary>
            /// Returns a game object from the pool. Instantiate a new one if none is available
            /// </summary>
            /// <param name="position">Position of the object</param>
            /// <param name="rotation">Rotation of the object</param>
            public GameObject Get(Vector3 position = default(Vector3), Vector3 rotation = default(Vector3), Transform parent = null)
            {
                for (int i = 0; i < pool.Count; i++)
                {
                    if (!pool[i].activeInHierarchy)
                    {
                        pool[i].transform.SetParent(parent);
                        pool[i].transform.position = position;
                        pool[i].transform.rotation = Quaternion.Euler(rotation);
                        pool[i].SetActive(true);
                        return pool[i];
                    }
                }
                var newObject = Instantiate(prefab, position, Quaternion.Euler(rotation));
#if UNITY_EDITOR
                newObject.name = $"{name}_{pool.Count}";
#endif
                pool.Add(newObject);
                pool[pool.Count - 1].transform.SetParent(parent);
                return pool[pool.Count - 1];
            }

            /// <summary>
            /// Destroyes currently disabled objects
            /// </summary>
            public void DestroyUnused()
            {
                for (int i = 0; i < pool.Count; i++)
                {
                    if (!pool[i].activeInHierarchy)
                    {
                        Destroy(pool[i]);
                        pool.Remove(pool[i]);
                    }
                }
            }

            /// <summary>
            /// Destroyes all objects
            /// </summary>
            public void DestroyAll()
            {
                for (int i = 0; i < pool.Count; i++)
                    Destroy(pool[i]);

                pool.Clear();
            }


            /// <summary>
            /// Destroy one object from its pool
            /// </summary>
            public void DestroyObject(GameObject gameObj)
            {
                pool.Remove(gameObj);
                Destroy(gameObj);
            }
            


            /// <summary>
            /// Recycle all objects in pool
            /// </summary>
            public void RecycleAll()
            {
                for (int i = 0; i < pool.Count; i++)
                {
                    //pool[i].transform.SetParent(InactiveObjects);
                    //pool[i].SetActive(false);
                    RemoveToInactive(pool[i]);
                }
            }


            /// <summary>
            /// Instantiates new objects to the pool
            /// </summary>
            /// <param name="count">Number of objects to prepare</param>
            public void Prepare(int count = 0)
            {
                if (pool.Count >= count)
                {
                    RecycleAll();
                    return;
                }
                for (int i = 0; i < count; i++)
                {
                    var newObject = Instantiate(prefab, InactiveObjects);
#if UNITY_EDITOR
                    newObject.name = $"{name}_{pool.Count}";
#endif
                    pool.Add(newObject);
                    pool[pool.Count - 1].SetActive(false);
                }
            }


            /// <summary>
            /// Find the first object that satisfied a condition
            /// </summary>
            public GameObject Find(Func<GameObject, bool> condition)
            {
                return pool.FirstOrDefault(condition);
            }

        }

        //A transform to store inactive objects
        private static Transform inactiveObjects;
        private static Transform InactiveObjects
        {
            get
            {
                if (!inactiveObjects)
                {
                    inactiveObjects = new GameObject("Inactive Objects").transform;
                    inactiveObjects.transform.SetParent(Instance.transform);
                }
                return inactiveObjects;
            }
        }

        public List<ObjectPool> pools = new List<ObjectPool>();


        /// <summary>
        /// Get a pool by its name
        /// </summary>
        /// <param name="name"></param>
        public ObjectPool GetPool(string name)
        {
            foreach (ObjectPool pool in pools)
            {
                if (pool.Name == name)
                {
                    return pool;
                }
            }
            return null;
        }


        /// <summary>
        /// Disables the object and moves it under the Pool object
        /// </summary>
        /// <param name="obj">Object to disable</param>
        public static void RemoveToInactive(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(InactiveObjects);
            obj.transform.localPosition = Vector3.zero;
        }
    }
}