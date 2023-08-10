using System;

namespace HAVIGAME.Services.Advertisings {
    public abstract class AppOpenAd : Ad {
        private AdId id;

        public override AdUnit Unit => AdUnit.AppOpenAd;

        public AppOpenAd(AdId id) : base(id.Reset()) {
            this.id = id;
        }

        public abstract bool Load();
        public abstract bool Show(Action onCompleted, string placement);
        protected virtual void Reload() {
            Reload(false);
        }
        protected virtual void Reload(bool reset) {
            if (reset) {
                UpdateIdentifier(id.Reset());
            }
            else {
                UpdateIdentifier(id.MoveNext());
            }
            Load();
        }
    }
}
