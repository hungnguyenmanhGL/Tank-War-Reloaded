using System.Collections;
using UnityEngine;

namespace HAVIGAME.Scenes {
    public abstract class LoadingView : MonoBehaviour {
        [SerializeField] private int id;

        public int Id => id;

        public abstract void Initialize();
        public abstract void OnStart();
        public abstract void OnStartLoading();
        public abstract void OnLoading(float progress);
        public abstract void OnFinishLoading();
        public abstract void OnFinish();
        public abstract IEnumerator FadeIn();
        public abstract IEnumerator FadeOut();
    }
}
