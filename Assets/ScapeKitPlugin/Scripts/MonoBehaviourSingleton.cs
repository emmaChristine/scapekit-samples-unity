using UnityEngine;
using System.Collections;

namespace ScapeKitUnity
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T BehaviourInstance
        {
            get
            {
                if (instance == null)
                {
                    GameObject coreGameObject = new GameObject(typeof(T).Name);

                    instance = coreGameObject.AddComponent<T>();
                }

                return instance;
            }
        }

        private static T instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = GetComponent<T>();
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }
}
