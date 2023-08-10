using UnityEngine;

namespace HAVIGAME.Services.Advertisings {
    public abstract class MediumRectangleAd : Ad {
        public override AdUnit Unit => AdUnit.MediumRectangle;
        public bool Created { get; protected set; }
        public MediumRectangleAdPosition Position { get; protected set; }
        public Vector2Int Offset { get; protected set; }
        public abstract float Height { get; }

        public MediumRectangleAd(string id) : base(id) {
            this.Created = false;
            this.Position = MediumRectangleAdPosition.Null;
            this.Offset = Vector2Int.zero;
        }

        public abstract bool Create();
        public abstract bool Show(MediumRectangleAdPosition position, string placement, Vector2Int offset);
        public abstract bool Hide();
        public abstract bool Destroy();
    }

    public enum MediumRectangleAdPosition {
        TopCenter,
        TopLeft,
        TopRight,
        Centered,
        CenterLeft,
        CenterRight,
        BottomCenter,
        BottomLeft,
        BottomRight,
        Null,
    }
}
