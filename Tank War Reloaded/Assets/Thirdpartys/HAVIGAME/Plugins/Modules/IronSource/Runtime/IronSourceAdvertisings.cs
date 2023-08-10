using HAVIGAME.Services.Advertisings;
using System;
using UnityEngine;

namespace HAVIGAME.Plugins.IronSources {
#if IRON_SOURCE
    public class IronSourceAdvertisings : AdService {

        private IronSourceInterstitialAd[] interstitialAds;
        private IronSourceRewardedAd[] rewardedAds;
        private IronSourceBannerAd[] bannerAds;

        public override AdNetwork Network => AdNetwork.IronSource;
        public override AdUnit AdUnitSupported => AdUnit.IronSourceAdUnits;

        protected override AppOpenAd[] AppOpenAds => null;
        protected override InterstitialAd[] InterstitialAds => interstitialAds;
        protected override RewardedAd[] RewardedAds => rewardedAds;
        protected override BannerAd[] BannerAds => bannerAds;
        protected override RewardedInterstitialAd[] RewardedInterstitialAds => null;
        protected override MediumRectangleAd[] MediumRectangleAds => null;
        protected override NativeAd[] NativeAds => null;

        public override void Initialize() {
            if (InitializeEvent.IsRunning) {
                Log.Warning("[IronSourceAdvertisings] IronSource advertisings is running with initialize state {0}.", IsInitialized);
                return;
            }

            IronSourceManager.initializeEvent.AddListener(OnIronSourceInitializedCallback);
        }

        private void OnIronSourceInitializedCallback(bool isInitialized) {
            if (isInitialized) {

                IronSource.Agent.validateIntegration();

                IronSourceEvents.onImpressionDataReadyEvent += OnAdRevenuePaidEvent;

                IronSourceSettings settings = IronSourceSettings.Instance;

                if (settings.UseInterstitialAd) {
                    interstitialAds = new IronSourceInterstitialAd[1];
                    IronSourceInterstitialAd interstitialAd = new IronSourceInterstitialAd(this);
                    Log.Debug("[IronSourceAdvertisings] Create interstitial ad {0}", interstitialAd);
                    RegisterAdEvents(interstitialAd);
                    interstitialAds[0] = interstitialAd;
                    interstitialAd.Load();
                }

                if (settings.UseRewardedAd) {
                    rewardedAds = new IronSourceRewardedAd[1];

                    IronSourceRewardedAd rewardedAd = new IronSourceRewardedAd(this, settings.ManualLoadRewardedAd);
                    Log.Debug("[IronSourceAdvertisings] Create rewarded ad {0}", rewardedAd);
                    RegisterAdEvents(rewardedAd);
                    rewardedAds[0] = rewardedAd;
                    rewardedAd.Load();
                }

                if (settings.UseBannerAd) {
                    bannerAds = new IronSourceBannerAd[1];

                    IronSourceBannerAd bannerAd = new IronSourceBannerAd(this);
                    Log.Debug("[IronSourceAdvertisings] Create banner ad {0}", bannerAd);
                    RegisterAdEvents(bannerAd);
                    bannerAds[0] = bannerAd;
                    bannerAd.Create();
                }

                Database.Unload(settings);

                Log.Info("[IronSourceAdvertisings] IronSource advertisings initialize completed.");
                InitializeEvent.Invoke(true);
            }
            else {
                Log.Error("[IronSourceAdvertisings] IronSource advertisings initialize failed because IronSource initialize failed");
                InitializeEvent.Invoke(false);
            }
        }

        private void OnAdRevenuePaidEvent(IronSourceImpressionData impressionData) {
            AdRevenuePaid adImpression = new AdRevenuePaid() {
                adPlatform = Network.ToString(),
                adSource = impressionData.adNetwork,
                adUnitName = impressionData.instanceName,
                adFormat = impressionData.adUnit,
                value = impressionData.revenue ?? 0,
                currency = "USD"
            };

            InvokeAdRevenuePaidEvent(AdUnit.Unknow, adImpression);
        }

        private class IronSourceInterstitialAd : InterstitialAd {
            private IronSourceAdvertisings client;
            private string placement;
            private Action onCompleted;
            private int retry;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return IronSource.Agent.isInterstitialReady();
                }
            }

            public IronSourceInterstitialAd(IronSourceAdvertisings client) : base(SingleAdId.Empty) {
                this.client = client;
                this.retry = 0;

                IronSourceInterstitialEvents.onAdReadyEvent += OnAdReadyEvent;
                IronSourceInterstitialEvents.onAdLoadFailedEvent += OnAdLoadFailedEvent;
                IronSourceInterstitialEvents.onAdOpenedEvent += OnAdOpenedEvent;
                IronSourceInterstitialEvents.onAdShowSucceededEvent += OnAdShowSucceededEvent;
                IronSourceInterstitialEvents.onAdShowFailedEvent += OnAdShowFailedEvent;
                IronSourceInterstitialEvents.onAdClickedEvent += OnAdClickedEvent;
                IronSourceInterstitialEvents.onAdClosedEvent += OnAdClosedEvent;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[IronSourceInterstitialAd] Load failed! Interstitial ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[IronSourceInterstitialAd] Load failed! Interstitial ad is loading!");
                    return false;
                }

                //if (string.IsNullOrEmpty(Identifier)) {
                //    Log.Warning("[IronSourceInterstitialAd] Load failed! Interstitial ad id is null or empty!");
                //    return false;
                //}

                IsLoading = true;
                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                IronSource.Agent.loadInterstitial();
                return true;
            }

            public override bool Show(Action onCompleted, string placement) {
                if (!IsReady) {
                    Log.Warning("[IronSourceInterstitialAd] Show failed! Interstitial ad not avaliable!");
                    return false;
                }

                if (IsShowing) {
                    Log.Warning("[IronSourceInterstitialAd] Show failed! Interstitial ad is showing!");
                    return false;
                }

                IsShowing = true;
                this.onCompleted = onCompleted;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                if (string.IsNullOrEmpty(placement)) {
                    IronSource.Agent.showInterstitial();
                }
                else {
                    IronSource.Agent.showInterstitial(placement);
                }
                return true;
            }

            private void OnAdReadyEvent(IronSourceAdInfo info) {
                IsLoading = false;

                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                retry = 0;
            }

            private void OnAdLoadFailedEvent(IronSourceError error) {
                IsLoading = false;

                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                retry++;
                float delay = 2 * Math.Min(6, retry);
                Invoke(Reload, delay);
            }

            private void OnAdOpenedEvent(IronSourceAdInfo info) {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdShowSucceededEvent(IronSourceAdInfo info) {
            }

            private void OnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info) {
                IsShowing = false;
                Invoke(onCompleted);

                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString())
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnAdClickedEvent(IronSourceAdInfo info) {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdClosedEvent(IronSourceAdInfo info) {
                IsShowing = false;
                Invoke(onCompleted);

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                Reload(true);
            }
        }

        private class IronSourceRewardedAd : RewardedAd {
            private IronSourceAdvertisings client;
            private bool manualLoadRewardedAd;
            private string placement;
            private Action onCompleted;
            private Action onFailed;
            private int retry;
            private bool receivedReward;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return IronSource.Agent.isRewardedVideoAvailable();
                }
            }

            public IronSourceRewardedAd(IronSourceAdvertisings client, bool manualLoadRewardedAd) : base(SingleAdId.Empty) {
                this.client = client;
                this.manualLoadRewardedAd = manualLoadRewardedAd;
                this.retry = 0;
                this.receivedReward = false;

                IronSourceRewardedVideoEvents.onAdAvailableEvent += OnAdAvailableEvent;
                IronSourceRewardedVideoEvents.onAdUnavailableEvent += OnAdUnavailableEvent;
                IronSourceRewardedVideoEvents.onAdReadyEvent += OnAdReadyEvent;
                IronSourceRewardedVideoEvents.onAdLoadFailedEvent += OnAdLoadFailedEvent;
                IronSourceRewardedVideoEvents.onAdOpenedEvent += OnnAdOpenedEvent;
                IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnAdShowFailedEvent;
                IronSourceRewardedVideoEvents.onAdClickedEvent += OnAdClickedEvent;
                IronSourceRewardedVideoEvents.onAdRewardedEvent += OnAdReceivedRewardEvent;
                IronSourceRewardedVideoEvents.onAdClosedEvent += OnAdClosedEvent;
            }

            public override bool Load() {
                if (manualLoadRewardedAd) {
                    if (IsReady) {
                        Log.Warning("[IronSourceInterstitialAd] Load failed! Rewaded ad is ready!");
                        return false;
                    }

                    if (IsLoading) {
                        Log.Warning("[IronSourceInterstitialAd] Load failed! Rewaded ad is loading!");
                        return false;
                    }

                    //if (string.IsNullOrEmpty(Identifier)) {
                    //    Log.Warning("[IronSourceInterstitialAd] Load failed! Interstitial ad id is null or empty!");
                    //    return false;
                    //}

                    IsLoading = true;
                    InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                        .Add(AdEventArgs.placement, placement));

                    IronSource.Agent.loadRewardedVideo();
                    return true;
                }
                else {
                    return true;
                }
            }

            public override bool Show(Action onCompleted, Action onFailed, string placement) {
                if (!IsReady) {
                    Log.Warning("[IronSourceRewardedAd] Show failed! Rearded ad not avaliable!");
                    return false;
                }

                if (IsShowing) {
                    Log.Warning("[IronSourceRewardedAd] Show failed! Rearded ad is showing!");
                    return false;
                }

                IsShowing = true;
                receivedReward = false;
                this.onCompleted = onCompleted;
                this.onFailed = onFailed;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.placement, placement));

                if (string.IsNullOrEmpty(placement)) {
                    IronSource.Agent.showRewardedVideo();
                }
                else {
                    IronSource.Agent.showRewardedVideo(placement);
                }
                return true;
            }

            private void OnAdAvailableEvent(IronSourceAdInfo info) {

            }

            private void OnAdUnavailableEvent() {

            }

            private void OnAdReadyEvent(IronSourceAdInfo info) {
                IsLoading = false;

                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                retry = 0;
            }

            private void OnAdLoadFailedEvent(IronSourceError error) {
                IsLoading = false;

                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                retry++;
                float delay = 2 * Math.Min(6, retry);
                Invoke(Reload, delay);
            }

            private void OnnAdOpenedEvent(IronSourceAdInfo info) {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info) {
                IsShowing = false;
                receivedReward = false;
                Invoke(onFailed);

                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString())
                    .Add(AdEventArgs.error, error.ToString()));

                Reload();
            }

            private void OnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo info) {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.placement, placement.getPlacementName())
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdReceivedRewardEvent(IronSourcePlacement placement, IronSourceAdInfo info) {
                receivedReward = true;
                CheckReceiveRewardProcess();
            }

            private void OnAdClosedEvent(IronSourceAdInfo info) {
                IsShowing = false;
                CheckReceiveRewardProcess();

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                Reload();
            }

            private void CheckReceiveRewardProcess() {
                if (!IsShowing) {
                    if (receivedReward) {
                        Invoke(onCompleted);
                    }
                    else {
                        Invoke(onFailed);
                    }
                    receivedReward = false;
                }
            }
        }

        private class IronSourceBannerAd : BannerAd {
            private IronSourceAdvertisings client;
            private string placement;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return true;
                }

            }
            public override float Height {
                get {

                    if (IsShowing) {
#if UNITY_ANDROID
                        if (Screen.dpi > 720) {
                            return ConvertDpToPixels(90);
                        }
                        else {
                            return ConvertDpToPixels(50);
                        }
#elif UNITY_IOS
                        return ConvertDpToPixels(50);
#else
                        return 0;
#endif
                    }
                    else {
                        return 0;
                    }
                }
            }

            public IronSourceBannerAd(IronSourceAdvertisings client) : base(string.Empty) {
                this.client = client;

                IronSourceBannerEvents.onAdLoadedEvent += OnAdLoadedEvent;
                IronSourceBannerEvents.onAdLoadFailedEvent += OnAdLoadFailedEvent;
                IronSourceBannerEvents.onAdClickedEvent += OnAdClickedEvent;
                IronSourceBannerEvents.onAdScreenPresentedEvent += OnAdScreenPresentedEvent;
                IronSourceBannerEvents.onAdScreenDismissedEvent += OnAdScreenDismissedEvent;
                IronSourceBannerEvents.onAdLeftApplicationEvent += OnAdLeftApplicationEvent;
            }
            
            public override bool Create() {
                if (Created) {
                    Log.Warning("[IronSourceBannerAd] Banner ad has created!");
                    return false;
                }

                Created = true;

                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, ToIronSourceBannerAdPosition(BannerAdPosition.Centered));

                IronSource.Agent.hideBanner();
                return true;
            }

            public override bool Show(BannerAdPosition position, string placement, Vector2Int offset) {
                if (!Created) {
                    Log.Warning("[IronSourceBannerAd] Banner ad not been created!");
                    return false;
                }

                if (Position != position) {
                    IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, ToIronSourceBannerAdPosition(position));
                }

                IsLoading = true;
                IsShowing = true;
                this.Position = position;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.placement, placement));

                IronSource.Agent.displayBanner();
                return true;
            }

            public override bool Hide() {
                if (!IsShowing) {
                    Log.Warning("[IronSourceBannerAd] Banner ad not showing!");
                    return false;
                }

                IsShowing = false;
                IronSource.Agent.hideBanner();
                return true;
            }

            public override bool Destroy() {
                if (!Created) {
                    Log.Warning("[IronSourceBannerAd] Banner ad not been created!");
                    return false;
                }

                Created = false;
                IronSource.Agent.destroyBanner();
                return true;
            }

            private void OnAdLoadedEvent(IronSourceAdInfo info) {
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdLoadFailedEvent(IronSourceError error) {
                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));
            }

            private void OnAdClickedEvent(IronSourceAdInfo info) {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdScreenPresentedEvent(IronSourceAdInfo info) {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdScreenDismissedEvent(IronSourceAdInfo info) {
                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdLeftApplicationEvent(IronSourceAdInfo info) {
            }

            private IronSourceBannerPosition ToIronSourceBannerAdPosition(BannerAdPosition position) {
                switch (position) {
                    case BannerAdPosition.TopCenter:
                        return IronSourceBannerPosition.TOP;
                    case BannerAdPosition.TopLeft:
                        return IronSourceBannerPosition.TOP;
                    case BannerAdPosition.TopRight:
                        return IronSourceBannerPosition.TOP;
                    case BannerAdPosition.Centered:
                        return IronSourceBannerPosition.BOTTOM;
                    case BannerAdPosition.CenterLeft:
                        return IronSourceBannerPosition.BOTTOM;
                    case BannerAdPosition.CenterRight:
                        return IronSourceBannerPosition.BOTTOM;
                    case BannerAdPosition.BottomCenter:
                        return IronSourceBannerPosition.BOTTOM;
                    case BannerAdPosition.BottomLeft:
                        return IronSourceBannerPosition.BOTTOM;
                    case BannerAdPosition.BottomRight:
                        return IronSourceBannerPosition.BOTTOM;
                    default:
                        return IronSourceBannerPosition.BOTTOM;
                }
            }

            public float ConvertDpToPixels(float dp) {
                return dp * (Screen.dpi / 160f);
            }

            public float ConverPixelsToDp(float pixel) {
                return pixel / (Screen.dpi / 160f);
            }
        }
    }
#endif

    [CategoryMenu("IronSource Advertisings")]
    [System.Serializable]
    public class IronSourceAdServiceProvider : AdServiceProvider {
        public override AdService GetService() {
#if IRON_SOURCE
            return new IronSourceAdvertisings();
#else
            return null;
#endif
        }
    }
}
