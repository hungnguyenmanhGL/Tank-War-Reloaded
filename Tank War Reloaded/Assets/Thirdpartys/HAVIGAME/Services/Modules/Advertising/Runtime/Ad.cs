namespace HAVIGAME.Services.Advertisings {
    public abstract class Ad {
        internal event AdRevenuePaidDelegate onRevenuePaid;
        internal event AdDelegate onLoad;
        internal event AdDelegate onLoaded;
        internal event AdDelegate onLoadFailed;
        internal event AdDelegate onDisplay;
        internal event AdDelegate onDisplayed;
        internal event AdDelegate onDisplayFailed;
        internal event AdDelegate onClicked;
        internal event AdDelegate onClosed;

        private string identifier;
        private bool isShowing;
        private bool isLoading;

        public string Identifier => identifier;
        public abstract AdService Client { get; }
        public abstract AdUnit Unit { get; }
        public abstract bool IsReady { get; }
        public bool IsShowing {
            get {
                return isShowing;
            }
            protected set {
                if (isShowing != value) {
                    isShowing = value;
                }
            }
        }
        public bool IsLoading {
            get {
                return isLoading;
            }
            protected set {
                if (isLoading != value) {
                    isLoading = value;
                }
            }
        }

        public Ad(string identifier) {
            this.identifier = identifier;
            this.isShowing = false;
            this.isLoading = false;
        }

        protected void UpdateIdentifier(string identifier) {
            this.identifier = identifier;
        }

        protected void InvokeOnRevenuePaidEvent(AdRevenuePaid args) {
            onRevenuePaid?.Invoke(Unit, args);
        }
        protected void InvokeOnLoadEvent(AdEventArgs args) {
            onLoad?.Invoke(Unit, args);
        }
        protected void InvokeOnLoadedEvent(AdEventArgs args) {
            onLoaded?.Invoke(Unit, args);
        }
        protected void InvokeOnLoadFailedEvent(AdEventArgs args) {
            onLoadFailed?.Invoke(Unit, args);
        }
        protected void InvokeOnDisplayEvent(AdEventArgs args) {
            onDisplay?.Invoke(Unit, args);
        }
        protected void InvokeOnDisplayedEvent(AdEventArgs args) {
            onDisplayed?.Invoke(Unit, args);
        }
        protected void InvokeOnDisplayFailedEvent(AdEventArgs args) {
            onDisplayFailed?.Invoke(Unit, args);
        }
        protected void InvokeOnClickedEvent(AdEventArgs args) {
            onClicked?.Invoke(Unit, args);
        }
        protected void InvokeOnClosedEvent(AdEventArgs args) {
            onClosed?.Invoke(Unit, args);
        }

        protected void Invoke(System.Action action) {
            Executor.Instance.RunOnMainTheard(action);
        }
        protected void Invoke(System.Action action, float delay) {
            Executor.Instance.RunOnMainTheard(action, delay);
        }

        public override string ToString() {
            return string.Format("{0}: {1}", Unit, Identifier);
        }
    }

    [System.Flags]
    [System.Serializable]
    public enum AdUnit {
        Unknow = 0,
        AppOpenAd = 1 << 0,
        BannerAd = 1 << 1,
        RewardedAd = 1 << 2,
        InterstitialAd = 1 << 3,
        RewardedInterstitialAd = 1 << 4,
        MediumRectangle = 1 << 5,
        NativeAd = 1 << 6,

        AdMobAdUnits = AppOpenAd | BannerAd | RewardedAd | InterstitialAd | RewardedInterstitialAd | NativeAd | MediumRectangle,
        AppLovinAdUnits = AppOpenAd | BannerAd | RewardedAd | InterstitialAd | MediumRectangle,
        IronSourceAdUnits = BannerAd | RewardedAd | InterstitialAd,
        All = AppOpenAd | BannerAd | RewardedAd | InterstitialAd | MediumRectangle | RewardedInterstitialAd | NativeAd,
    }
}
