using System;

namespace Common.Singleton
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        protected static bool _isInitialized = false;

        public static bool IsInitialized => _isInitialized;
        
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = Activator.CreateInstance<T>();
                    _instance.Init();
                    _isInitialized = true;
                }

                return _instance;
            }
        }

        public static void Release()
        {
            _isInitialized = false;
            _instance?.Dispose();
            _instance = null;
        }

        public virtual void StartUp()
        {
        }

        protected virtual void Init()
        {
        }

        protected abstract void Dispose();
    }
}