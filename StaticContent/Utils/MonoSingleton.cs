using UnityEngine;

namespace Common.Singleton
{
    public abstract class MonoSingletonSimple<T> : MonoBehaviour where T : MonoSingletonSimple<T>
    {
        private static T _instance;

        private static bool _isInit = false;
        public static T Instance
        {
            get
            {
                if (!_isInit)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                        var parent = GameObject.Find("Boot");
                        if (parent == null)
                        {
                            parent = new GameObject("Boot");
                            DontDestroyOnLoad(parent);
                        }

                        if (parent != null)
                        {
                            go.transform.parent = parent.transform;
                        }
                    }

                    _isInit = true;
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }

            DontDestroyOnLoad(gameObject);
            Init();
        }

        protected virtual void Init()
        {
        }

        public virtual void StartUp()
        {
        }

        public static void Release()
        {
            _isInit = false;
            if (_instance != null)
            {
                _instance.Dispose();
                Destroy(_instance.gameObject);
                _instance = null;
            }
        }

        protected abstract void Dispose();
    }
}