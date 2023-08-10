using UnityEngine;
using DG.Tweening;
using System;

namespace HAVIGAME.UI {
    [RequireComponent(typeof(DOTweenAnimation))]
    [AddComponentMenu("HAVIGAME/UI/UI Transition/DOTween Transition")]
    public class UIDOTweenTransition : UITransition {
        [SerializeField] private DOTweenAnimation animator;
        [Header("[Options]")]
        [SerializeField] private DOTweenAnimation[] secondAnimators;

        private Action onPlayCompleted;
        private Action onRewindCompleted;

        public override bool IsPlaying => animator.tween != null && animator.tween.IsPlaying();

        public override void Initialize() {
            animator.hasOnRewind = true;
            animator.hasOnComplete = true;
            animator.onRewind.AddListener(OnRewindCompleted);
            animator.onComplete.AddListener(OnPlayCompleted);

            animator.CreateTween(false, false);

            foreach (var animator in secondAnimators) {
                animator.CreateTween(false, false);
            }
        }

        private void OnDestroy() {
            if (animator) {
                animator.DOKill();
            }

            foreach (var animator in secondAnimators) {
                if (animator) animator.DOKill();
            }
        }

        public override void PlayShowAnimation(Action onCompleted) {
            this.onPlayCompleted = onCompleted;
            //animator.tween.OnRewind(() => onCompleted?.Invoke());
            animator.DORestart();


            foreach (var animator in secondAnimators) {
                animator.DORestart();
            }
        }

        public override void PlayHideAnimation(Action onCompleted) {
            this.onRewindCompleted = onCompleted;
            //animator.tween.OnComplete(() => onCompleted?.Invoke());
            animator.DOPlayBackwards();

            foreach (var animator in secondAnimators) {
                animator.DOPlayBackwards();
            }
        }

        private void OnRewindCompleted() {
            onRewindCompleted?.Invoke();
        }

        private void OnPlayCompleted() {
            onPlayCompleted?.Invoke();
        }
    }
}
