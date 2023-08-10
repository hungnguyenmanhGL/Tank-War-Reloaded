using UnityEngine;

namespace HAVIGAME.Services.Advertisings {
    public abstract class BannerAd : Ad {
        public override AdUnit Unit => AdUnit.BannerAd;
        public bool Created { get; protected set; }
        public BannerAdPosition Position { get; protected set; }
        public Vector2Int Offset { get; protected set; }
        public abstract float Height { get; }

        public BannerAd(string id) : base(id) {
            this.Created = false;
            this.Position = BannerAdPosition.Null;
            this.Offset = Vector2Int.zero;
        }

        public abstract bool Create();
        public abstract bool Show(BannerAdPosition position, string placement, Vector2Int offset);
        public abstract bool Hide();
        public abstract bool Destroy();
    }

    public enum BannerAdPosition {
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
