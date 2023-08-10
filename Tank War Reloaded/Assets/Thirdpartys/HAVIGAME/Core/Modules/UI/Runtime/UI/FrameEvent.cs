using UnityEngine.Events;

namespace HAVIGAME.UI {
    [System.Serializable]
    public class FrameEvent : UnityEvent<UIFrame> { }

    public delegate void FrameDelegate(UIFrame frame);
}
