using UnityEngine;

namespace HAVIGAME {
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        private const string singletonNameFormat = "{0} (singleton)";

        private static T instance = null;

        public static T Instance {
            get {
                if (instance == null) {
                    if (!GameManager.IsQuiting) {
                        string type = typeof(T).Name;
                        new GameObject(Utility.Text.Format(singletonNameFormat, type)).AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance => instance != null;

        public virtual bool IsDontDestroyOnLoad => true;

        public void Create() { }

        protected void Awake() {
            if (instance == null) {
                instance = this as T;
                if (IsDontDestroyOnLoad) DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else if (instance != this) {
                Log.Warning("[SINGLETON] There is more than one instance of class {0} in the scene.", typeof(T).Name);
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnAwake() {

        }

        protected virtual void OnDestroy() {
            instance = null;
        }
    }
}