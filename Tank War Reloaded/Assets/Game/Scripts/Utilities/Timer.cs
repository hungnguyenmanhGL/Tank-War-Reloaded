using DG.Tweening;
using System;

public class Timer {
    private Tween tween;

    public void StartCountdown(int duration, Action<int> onUpdate = null, Action onCompleted = null, bool ignoreTimeScale = false) {
        tween?.Kill();

        int current = duration;
        onUpdate?.Invoke(current);

        tween = DOVirtual.DelayedCall(1, Empty, ignoreTimeScale)
                         .SetLoops(duration)
                         .SetRecyclable(true)
                         .OnStepComplete(() => {
                             current--;
                             onUpdate?.Invoke(current);
                         })
                         .OnComplete(() => {
                             tween = null;
                             onCompleted?.Invoke();
                         });
    }

    public void Start(int duration, Action<int> onUpdate = null, Action onCompleted = null, bool ignoreTimeScale = false) {
        tween?.Kill();

        int current = 0;
        onUpdate?.Invoke(current);

        tween = DOVirtual.DelayedCall(1, Empty, ignoreTimeScale)
                         .SetLoops(duration)
                         .SetRecyclable(true)
                         .OnStepComplete(() => {
                             current++;
                             onUpdate?.Invoke(current);
                         })
                         .OnComplete(() => {
                             tween = null;
                             onCompleted?.Invoke();
                         });
    }

    private void Empty() { }

    public void Stop() {
        tween?.Kill();
    }
}
