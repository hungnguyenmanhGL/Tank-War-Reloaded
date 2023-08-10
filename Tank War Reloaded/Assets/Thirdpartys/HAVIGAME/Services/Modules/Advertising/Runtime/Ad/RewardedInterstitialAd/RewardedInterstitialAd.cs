using System;

namespace HAVIGAME.Services.Advertisings {
    public abstract class RewardedInterstitialAd : Ad {
        private AdId id;

        public override AdUnit Unit => AdUnit.RewardedInterstitialAd;

        public RewardedInterstitialAd(AdId id) : base(id.Reset()) {
            this.id = id;
        }

        public abstract bool Load();
        public abstract bool Show(Action onCompleted, Action onFailed, string placement);
        protected virtual void Reload() {
            Reload(false);
        }
        protected virtual void Reload(bool reset) {
            if (id != null) {
                if (reset) {
                    UpdateIdentifier(id.Reset());
                }
                else {
                    UpdateIdentifier(id.MoveNext());
                }
            }
            Load();
        }
    }
}
