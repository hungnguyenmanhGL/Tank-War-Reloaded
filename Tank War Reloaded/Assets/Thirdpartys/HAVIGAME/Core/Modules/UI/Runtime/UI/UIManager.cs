using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace HAVIGAME.UI {

    [DisallowMultipleComponent]
    [AddComponentMenu("HAVIGAME/UI/UI Manager")]
    public class UIManager : Singleton<UIManager>, IEnumerable<UIFrame> {

        public static readonly InitializeEvent initializeEvent = new InitializeEvent();

        [System.NonSerialized] private Transform root;
        [System.NonSerialized] private FrameFactory factory;
        [System.NonSerialized] private EventSystem eventSystem;

        public event FrameDelegate onShowed;
        public event FrameDelegate onHidden;

        private Stack<UIFrame> frames;
        private bool autoOrderEnabled;
        private bool physicalBackButtonEnabled;
        private bool clearUIOnSceneLoad;

        public override bool IsDontDestroyOnLoad => true;
        public int Count => frames.Count;

        public static bool IsInitialized => initializeEvent.IsInitialized;

        protected override void OnAwake() {
            SceneManager.sceneLoaded += OnSceneLoaded;

            GameUISettings settings = GameUISettings.Instance;

            root = new GameObject("Root").transform;
            root.SetParent(transform);

            factory = settings.FrameFactory;
            factory.Initialize(root, settings.InitializeCapacity);

            eventSystem = EventSystem.current;

            if (eventSystem == null) {
                eventSystem = new GameObject("Event System").AddComponent<EventSystem>();
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }

            eventSystem.transform.SetParent(transform);

            frames = new Stack<UIFrame>(settings.InitializeCapacity);
            autoOrderEnabled = settings.OrderUIEnabled;
            physicalBackButtonEnabled = settings.PhysicalBackButton;
            clearUIOnSceneLoad = settings.ClearUIOnSceneLoad;

            Database.Unload(settings);

            Log.Debug("[UIManager] Initialize completed.");
            initializeEvent.Invoke(true);
        }

#if UNITY_EDITOR || UNITY_ANDROID
        private void Update() {
            if (physicalBackButtonEnabled && Input.GetKeyDown(KeyCode.Escape)) {
                if (Count > 0) {
                    UIFrame frame = Peek();

                    if (frame && frame.Interactable) {
                        frame.Back();
                    }
                }
            }
        }
#endif

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
            if (clearUIOnSceneLoad) {
                Clear(true);
            }
        }

        public void SetInteractable(bool interactable) {
            foreach (UIFrame frame in this) {
                frame.Interactable = interactable;
            }
        }

        public bool IsTop(UIFrame frame) {
            return frame && Peek() == frame;
        }

        public UIFrame Push(string name, Action<UIFrame> pushOption = null, bool instant = false) {
            UIFrame frame = factory.Spawn(name);
            if (frame) {
                Initialize(frame);
                pushOption?.Invoke(frame);
                frame.Show(instant);
            }
            return frame;
        }

        public F Push<F>(Action<F> pushOption = null, bool instant = false) where F : UIFrame {
            F frame = factory.Spawn<F>();
            if (frame) {
                Initialize(frame);
                pushOption?.Invoke(frame);
                frame.Show(instant);
            }
            return frame;
        }

        public UIFrame Peek() {
            if (frames.Count > 0) {
                return frames.Peek();
            }
            return null;
        }

        public UIFrame Pop(bool instant = false) {
            if (frames.Count > 0) {
                UIFrame frame = frames.Peek();
                frame.Hide(instant);
                return frame;
            }
            return null;
        }

        public UIFrame Back() {
            if (frames.Count > 0) {
                UIFrame frame = frames.Peek();
                frame.Back();
                return frame;
            }
            return null;
        }

        public F GetFrameShowed<F>() where F : UIFrame {
            return FrameFactory.GetFrame<F>(factory.GetAllReleased());
        }

        public bool Contains(UIFrame frame) {
            return frames.Contains(frame);
        }

        public void Clear(bool instant = false) {
            for (int i = 0; i <= Count; i++) {
                Pop(instant);
            }
        }

        public IEnumerator<UIFrame> GetEnumerator() {
            return frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return frames.GetEnumerator();
        }

        private void Initialize(UIFrame frame) {
            if (frame == null)
                return;

            if (!frame.Initialized) {
                frame.Initialize(this);
            }
        }

        internal void OnFrameShowed(UIFrame frame) {
            if (frame == null)
                return;

            UIFrame current = Peek();

            if (current) {
                current.Pause();
            }

            frames.Push(frame);
            onShowed?.Invoke(frame);

            if (autoOrderEnabled) {
                frame.transform.SetAsLastSibling();

                foreach (UIFrame item in UIManager.Instance) {
                    if (item) {
                        item.SortingOrder = item.transform.GetSiblingIndex();
                    }
                }
            }
        }

        internal void OnFrameHidden(UIFrame frame) {
            if (frame == null)
                return;

            frames.Pop();
            factory.Recycle(frame);
            onHidden?.Invoke(frame);

            UIFrame current = Peek();

            if (current) {
                current.Resume();
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModule() {
            GameManager.RegisterModule<Initializer>();
        }

        public class Initializer : ModuleInitializer {

            public override int Order => EXTEND_MODULE;
            public override InitializeEvent InitializeEvent => UIManager.initializeEvent;

            public override void Initialize() {
                UIManager.Instance.Create();
            }
        }
    }
}
