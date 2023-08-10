using UnityEngine;
using UnityEngine.Events;

namespace HAVIGAME.UI {
    [DisallowMultipleComponent]
    [AddComponentMenu("HAVIGAME/UI/UI Frame")]
    public abstract class UIFrame : MonoBehaviour {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private UITransition transition;

        private UIManager manager;
        private bool initialized;
        private bool showed;
        private bool paused;
        private FrameEvent onHidden;

        public bool Initialized => initialized;
        public virtual string Name => gameObject.name;
        public bool Showed => showed;
        public bool Paused => paused;
        public bool IsOnTop => manager.IsTop(this);
        public bool HasTransition => transition != null;
        public bool IsInTransition => transition.IsPlaying;
        public float Alpha {
            get {
                if (canvasGroup) {
                    return canvasGroup.alpha;
                }

                return 1;
            }
            set {
                if (canvasGroup) {
                    canvasGroup.alpha = value;
                }
            }
        }
        public bool Interactable {
            get {
                if (canvasGroup) {
                    return canvasGroup.interactable;
                }

                return true;
            }
            set {
                if (canvasGroup) {
                    canvasGroup.interactable = value;
                }
            }
        }
        public bool BlocksRaycasts {
            get {
                if (canvasGroup) {
                    return canvasGroup.blocksRaycasts;
                }

                return true;
            }
            set {
                if (canvasGroup) {
                    canvasGroup.blocksRaycasts = value;
                }
            }
        }
        public int SortingOrder {
            get {
                if (canvas) {
                    return canvas.sortingOrder;
                }

                return 0;
            }
            set {
                if (canvas) {
                    canvas.sortingOrder = value;
                }
            }
        }

        protected virtual void Reset() {
            canvas = GetComponentInChildren<Canvas>();
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            transition = GetComponentInChildren<UITransition>();
        }

        internal void Initialize(UIManager manager) {
            if (!Initialized) {
                this.manager = manager;
                this.initialized = true;

                showed = false;
                paused = false;
            }

            if (transition) {
                transition.Initialize();
            }
        }

        internal void Show(bool instant = false) {
            if (!Showed) {
                showed = true;
                manager.OnFrameShowed(this);
                OnShow(instant);
            }
        }

        internal void Hide(bool instant = false) {
            if (Showed) {
                showed = false;
                manager.OnFrameHidden(this);
                OnHide(instant);
            }
        }

        internal void Pause() {
            if (!Paused) {
                paused = true;
                OnPause();
            }
        }

        internal void Resume() {
            if (Paused) {
                paused = false;
                OnResume();
            }
        }

        internal void Back() {
            OnBack();
        }

        public void OnHideCallback(UnityAction<UIFrame> callback) {
            this.onHidden.AddListener(callback);
        }

        protected virtual void OnShow(bool instant = false) {
            gameObject.SetActive(true);
            Interactable = false;

            if (!instant && HasTransition) {
                transition.PlayShowAnimation(OnShowCompleted);
            }
            else {
                OnShowCompleted();
            }
        }

        protected virtual void OnHide(bool instant = false) {
            Interactable = false;

            if (!instant && HasTransition) {
                transition.PlayHideAnimation(OnHideCompleted);
            }
            else {
                OnHideCompleted();
            }
        }

        protected virtual void OnShowCompleted() {
            Interactable = true;
        }

        protected virtual void OnHideCompleted() {
            gameObject.SetActive(false);
            onHidden?.Invoke(this);
        }

        protected virtual void OnPause() { }

        protected virtual void OnResume() { }

        protected virtual void OnBack() {
            Hide();
        }
    }
}
