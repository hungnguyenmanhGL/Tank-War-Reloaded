using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HAVIGAME {
    public enum GameState : byte {
        Unknown,
        Initializing,
        Running,
        Quiting,
    }

    public sealed class GameManager : Singleton<GameManager> {
        public delegate void BooleanDelegate(bool value);
        public delegate void EmptyDelegate();

        public static event BooleanDelegate onApplicationFocus;
        public static event BooleanDelegate onApplicationPause;
        public static event EmptyDelegate onApplicationQuit;

        private static GameState currentState = GameState.Unknown;
        private static List<ModuleInitializer> moduleInitializers = new List<ModuleInitializer>(16);

        public static bool IsRunning => currentState == GameState.Running;
        public static bool IsQuiting => currentState == GameState.Quiting;
        public static GameState CurrentState => currentState;

        public static void RegisterModule<T>() where T : ModuleInitializer, new() {
            moduleInitializers.Add(new T());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            currentState = GameState.Initializing;
            GameManager.Instance.Create();
        }

        public static void Quit() {
            currentState = GameState.Quiting;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public override bool IsDontDestroyOnLoad => true;

        protected override void OnAwake() {
            base.OnAwake();

            Application.lowMemory += OnApplicationLowMemory;
        }

        private void Start() {
            BaseSettings settings = BaseSettings.Instance;

            if (settings.AutoInitialize) {
                StartCoroutine(IEInitialize());
            }
        }

        public void ManualInitialize() {
            StartCoroutine(IEInitialize());
        }

        private IEnumerator IEInitialize() {
            BaseSettings settings = BaseSettings.Instance;

            Log.SetLogLevel(settings.LogLevel);
            Application.runInBackground = settings.RunInBackground;
            Application.targetFrameRate = settings.TargetFrameRate;
            Input.multiTouchEnabled = settings.MultiTouchEnabled;

            moduleInitializers.Sort(ModuleInitializer.comparer);

            foreach (ModuleInitializer initializer in moduleInitializers) {
                if (initializer.WaitForInitialized) {
                    bool initialized = false;

                    initializer.InitializeEvent.AddListener((result) => {
                        initialized = true;
                    });

                    initializer.Initialize();

                    while (!initialized) {
                        yield return null;
                    }
                }
                else {
                    initializer.Initialize();
                }
            }

            currentState = GameState.Running;

            settings.Launcher.Launch();
            Database.Unload(settings);
        }

        private void OnApplicationFocus(bool focus) {
            onApplicationFocus?.Invoke(focus);
        }

        private void OnApplicationPause(bool pause) {
            onApplicationPause?.Invoke(pause);
        }

        private void OnApplicationQuit() {
            currentState = GameState.Quiting;
            onApplicationQuit?.Invoke();
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            currentState = GameState.Quiting;

            Application.lowMemory -= OnApplicationLowMemory;
        }

        private void OnApplicationLowMemory() {
            if (ReferencePool.IsInitialized) {
                ReferencePool.Remove();
            }

            if (GameObjectPool.IsInitialized) {
                GameObjectPool.Instance.DestroyUnusedGameObjects();
            }

            Resources.UnloadUnusedAssets();
        }
    }
}
