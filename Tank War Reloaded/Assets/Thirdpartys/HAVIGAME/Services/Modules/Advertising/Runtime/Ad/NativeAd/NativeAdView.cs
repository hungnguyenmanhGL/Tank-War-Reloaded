using System;

namespace HAVIGAME.Services.Advertisings {
    public class NativeAdView {
        private string viewId;
        private NativeAdElementView[] elementViews;
        private NativeAd nativeAd;
        public Action onNativeAdDisplayed;
        public Action<string> onNativeAdDisplayFailed;
        public Action onNativeAdClosed;

        public bool IsShowing => nativeAd != null;
        public string ViewId => viewId;

        public NativeAdView(string viewId, NativeAdElementView[] elementViews) {
            this.viewId = viewId;
            this.elementViews = elementViews;
            this.nativeAd = null;
            this.onNativeAdDisplayed = null;
            this.onNativeAdDisplayFailed = null;
            this.onNativeAdClosed = null;
        }

        public bool Show(NativeAd nativeAd) {
            if (IsShowing) {
                return false;
            }
            this.nativeAd = nativeAd;

            if (Register()) {

                foreach (NativeAdElementView elementView in elementViews) {
                    if (elementView) {
                        elementView.Show();
                    }
                }

                onNativeAdDisplayed?.Invoke();
                return true;
            }
            else {
                onNativeAdDisplayFailed?.Invoke("Cannot register with any GameObject.");

                this.nativeAd = null;
                this.onNativeAdDisplayed = null;
                this.onNativeAdDisplayFailed = null;
                this.onNativeAdClosed = null;

                return false;
            }
        }

        public bool Hide() {
            if (!IsShowing) {
                return false;
            }

            foreach (NativeAdElementView elementView in elementViews) {
                if (elementView) elementView.Hide();
            }

            onNativeAdClosed?.Invoke();

            this.nativeAd = null;
            this.onNativeAdDisplayed = null;
            this.onNativeAdDisplayFailed = null;
            this.onNativeAdClosed = null;

            return true;
        }

        private bool Register() {
            bool registed = false;

            foreach (NativeAdElementView elementView in elementViews) {
                registed |= elementView.Register(nativeAd);
            }

            return registed;
        }
    }
}