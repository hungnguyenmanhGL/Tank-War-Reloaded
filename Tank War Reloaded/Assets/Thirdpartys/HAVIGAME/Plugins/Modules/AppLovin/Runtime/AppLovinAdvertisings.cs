using System;
using UnityEngine;
using HAVIGAME.Services.Advertisings;

namespace HAVIGAME.Plugins.AppLovin {
#if APPLOVIN
    public class AppLovinAdvertisings : AdService {

        private AppLovinAppOpenAd[] appOpenAds;
        private AppLovinInterstitialAd[] interstitialAds;
        private AppLovinRewardedAd[] rewardedAds;
        private AppLovinBannerAd[] bannerAds;
        private AppLovinMediumRectangleAd[] mediumRectangleAds;

        public override AdNetwork Network => AdNetwork.AppLovin;
        public override AdUnit AdUnitSupported => AdUnit.AppLovinAdUnits;
        protected override AppOpenAd[] AppOpenAds => appOpenAds;
        protected override InterstitialAd[] InterstitialAds => interstitialAds;
        protected override RewardedAd[] RewardedAds => rewardedAds;
        protected override BannerAd[] BannerAds => bannerAds;
        protected override RewardedInterstitialAd[] RewardedInterstitialAds => null;
        protected override MediumRectangleAd[] MediumRectangleAds => mediumRectangleAds;
        protected override NativeAd[] NativeAds => null;

        public override void Initialize() {
            if (InitializeEvent.IsRunning) {
                Log.Warning("[AppLovinAdvertisings] AppLovin advertisings is running with initialize state {0}.", IsInitialized);
                return;
            }

            AppLovinManager.initializeEvent.AddListener(OnAppLovinInitializedCallback);
        }

        private void OnAppLovinInitializedCallback(bool isInitialized) {

            if (isInitialized) {

                AppLovinSetting settings = AppLovinSetting.Instance;

                if (settings.AppOpenAdIds != null && settings.AppOpenAdIds.Length > 0) {
                    appOpenAds = new AppLovinAppOpenAd[settings.AppOpenAdIds.Length];

                    for (int i = 0; i < settings.AppOpenAdIds.Length; i++) {
                        AppLovinAppOpenAd appOpenAd = new AppLovinAppOpenAd(this, settings.AppOpenAdIds[i]);
                        Log.Debug("[AppLovinAdvertisings] Create app open ad {0}", appOpenAd);
                        RegisterAdEvents(appOpenAd);
                        appOpenAds[i] = appOpenAd;
                        appOpenAd.Load();
                    }
                }

                if (settings.InterstitialAdIds != null && settings.InterstitialAdIds.Length > 0) {
                    interstitialAds = new AppLovinInterstitialAd[settings.InterstitialAdIds.Length];

                    for (int i = 0; i < settings.InterstitialAdIds.Length; i++) {
                        AppLovinInterstitialAd interstitialAd = new AppLovinInterstitialAd(this, settings.InterstitialAdIds[i]);
                        Log.Debug("[AppLovinAdvertisings] Create interstitial ad {0}", interstitialAd);
                        RegisterAdEvents(interstitialAd);
                        interstitialAds[i] = interstitialAd;
                        interstitialAd.Load();
                    }
                }

                if (settings.RewardedAdIds != null && settings.RewardedAdIds.Length > 0) {
                    rewardedAds = new AppLovinRewardedAd[settings.RewardedAdIds.Length];

                    for (int i = 0; i < settings.RewardedAdIds.Length; i++) {
                        AppLovinRewardedAd rewardedAd = new AppLovinRewardedAd(this, settings.RewardedAdIds[i]);
                        Log.Debug("[AppLovinAdvertisings] Create rewarded ad {0}", rewardedAd);
                        RegisterAdEvents(rewardedAd);
                        rewardedAds[i] = rewardedAd;
                        rewardedAd.Load();
                    }
                }

                if (settings.BannerAdIds != null && settings.BannerAdIds.Length > 0) {
                    bannerAds = new AppLovinBannerAd[settings.BannerAdIds.Length];

                    for (int i = 0; i < settings.BannerAdIds.Length; i++) {
                        AppLovinBannerAd bannerAd = new AppLovinBannerAd(this, settings.BannerAdIds[i], settings.AdaptiveBannerEnable);
                        Log.Debug("[AppLovinAdvertisings] Create banner ad {0}", bannerAd);
                        RegisterAdEvents(bannerAd);
                        bannerAds[i] = bannerAd;
                        bannerAd.Create();
                    }
                }

                if (settings.MediumRectangleAdIds != null && settings.MediumRectangleAdIds.Length > 0) {
                    mediumRectangleAds = new AppLovinMediumRectangleAd[settings.MediumRectangleAdIds.Length];

                    for (int i = 0; i < settings.MediumRectangleAdIds.Length; i++) {
                        AppLovinMediumRectangleAd mediumRectangle = new AppLovinMediumRectangleAd(this, settings.MediumRectangleAdIds[i]);
                        Log.Debug("[AppLovinAdvertisings] Create medium rectangle ad {0}", mediumRectangle);
                        RegisterAdEvents(mediumRectangle);
                        mediumRectangleAds[i] = mediumRectangle;
                        mediumRectangle.Create();
                    }
                }

                Log.Debug("[AppLovinAdvertisings] AppLovin advertisings initialize completed.");
                InitializeEvent.Invoke(true);
                
                Database.Unload(settings);

                if (MaxSdk.IsVerboseLoggingEnabled()) {
                    MaxSdk.ShowMediationDebugger();
                }
            }
            else {
                Log.Error("[AppLovinAdvertisings] AppLovin advertisings initialize failed because AppLovin initialize failed");
                InitializeEvent.Invoke(false);
            }
        }

        private class AppLovinAppOpenAd : AppOpenAd {
            private AppLovinAdvertisings client;
            private string placement;
            private Action onCompleted;
            private int retry;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return MaxSdk.IsAppOpenAdReady(Identifier);
                }
            }

            public AppLovinAppOpenAd(AppLovinAdvertisings client, AdId id) : base(id) {
                this.client = client;
                this.retry = 0;
                this.placement = "NULL";

                MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAdClickedEvent;
                MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAdDisplayedEvent;
                MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
                MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAdHiddenEvent;
                MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAdLoadedEvent;
                MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
                MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AppLovinAppOpenAd] Load failed! App open ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AppLovinAppOpenAd] Load failed! App open ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AppLovinAppOpenAd] Load failed! App open ad id is null or empty!");
                    return false;
                }

                IsLoading = true;
                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
                MaxSdk.LoadAppOpenAd(Identifier);
                return true;
            }

            public override bool Show(Action onCompleted, string placement) {
                if (!IsReady) {
                    Log.Warning("[AppLovinAppOpenAd] Show failed! App open ad not avaliable!");
                    return false;
                }

                IsShowing = true;
                this.onCompleted = onCompleted;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                MaxSdk.ShowAppOpenAd(Identifier, placement);
                return true;
            }

            private void OnAdRevenuePaidEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = info.NetworkName,
                    adUnitName = info.AdUnitIdentifier,
                    adFormat = info.AdFormat,
                    value = info.Revenue,
                    currency = "USD",
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private void OnAdLoadFailedEvent(string id, MaxSdkBase.ErrorInfo error) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                retry++;
                float delay = 2 * Math.Min(6, retry);
                Invoke(Reload, delay);
            }

            private void OnAdLoadedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                retry = 0;
            }

            private void OnAdDisplayFailedEvent(string id, MaxSdkBase.ErrorInfo error, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString())
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnAdDisplayedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdClickedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdHiddenEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                Reload(true);
            }
        }

        private class AppLovinInterstitialAd : InterstitialAd {
            private AppLovinAdvertisings client;
            private string placement;
            private Action onCompleted;
            private int retry;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return MaxSdk.IsInterstitialReady(Identifier);
                }
            }

            public AppLovinInterstitialAd(AppLovinAdvertisings client, AdId id) : base(id) {
                this.client = client;
                this.retry = 0;
                this.placement = "NULL";

                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnAdLoadedEvent;
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnAdDisplayedEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
                MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnAdClickedEvent;
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdHiddenEvent;
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AppLovinInterstitialAd] Load failed! Interstitial ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AppLovinInterstitialAd] Load failed! Interstitial ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AppLovinInterstitialAd] Load failed! Interstitial ad id is null or empty!");
                    return false;
                }

                IsLoading = true;
                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                MaxSdk.LoadInterstitial(Identifier);
                return true;
            }

            public override bool Show(Action onCompleted, string placement) {
                if (!IsReady) {
                    Log.Warning("[AppLovinInterstitialAd] Show failed! Interstitial ad not avaliable!");
                    return false;
                }

                IsShowing = true;
                this.onCompleted = onCompleted;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                MaxSdk.ShowInterstitial(Identifier, placement);
                return true;
            }

            private void OnAdLoadedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                retry = 0;
            }

            private void OnAdLoadFailedEvent(string id, MaxSdkBase.ErrorInfo error) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                retry++;
                float delay = 2 * Math.Min(6, retry);
                Invoke(Reload, delay);
            }

            private void OnAdDisplayedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdDisplayFailedEvent(string id, MaxSdkBase.ErrorInfo error, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString())
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnAdClickedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdHiddenEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                Reload(true);
            }

            private void OnAdRevenuePaidEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = info.NetworkName,
                    adUnitName = info.AdUnitIdentifier,
                    adFormat = info.AdFormat,
                    value = info.Revenue,
                    currency = "USD",
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }
        }

        private class AppLovinRewardedAd : RewardedAd {
            private AppLovinAdvertisings client;
            private string placement;
            private Action onCompleted;
            private Action onFailed;
            private int retry;
            private bool receivedReward;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return MaxSdk.IsRewardedAdReady(Identifier);
                }
            }

            public AppLovinRewardedAd(AppLovinAdvertisings client, AdId id) : base(id) {
                this.client = client;

                this.retry = 0;
                this.placement = "NULL";
                this.receivedReward = false;

                MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnAdLoadedEvent;
                MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnAdDisplayedEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
                MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnAdClickedEvent;
                MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
                MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdHiddenEvent;
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AppLovinRewardedAd] Load failed! Rearded ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AppLovinRewardedAd] Load failed! Rearded ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AppLovinRewardedAd] Load failed! Rearded ad id is null or empty!");
                    return false;
                }

                IsLoading = true;
                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                MaxSdk.LoadRewardedAd(Identifier);

                return true;
            }

            public override bool Show(Action onCompleted, Action onFailed, string placement) {
                if (!IsReady) {
                    Log.Warning("[AppLovinRewardedAd] Show failed! Rearded ad not avaliable!");
                    return false;
                }

                receivedReward = false;
                IsShowing = true;
                this.onCompleted = onCompleted;
                this.onFailed = onFailed;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                MaxSdk.ShowRewardedAd(Identifier, placement);
                return true;
            }

            private void OnAdLoadedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                retry = 0;
            }

            private void OnAdLoadFailedEvent(string id, MaxSdkBase.ErrorInfo error) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                retry++;
                float delay = 2 * Math.Min(6, retry);
                Invoke(Reload, delay);
            }

            private void OnAdDisplayedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdDisplayFailedEvent(string id, MaxSdkBase.ErrorInfo info, MaxSdkBase.AdInfo error) {
                if (!Identifier.Equals(id)) return;

                IsShowing = false;
                receivedReward = false;
                Invoke(onFailed);
                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString())
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnAdClickedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdHiddenEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsShowing = false;
                CheckReceiveRewardProcess();

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));

                Reload(true);
            }

            private void OnAdReceivedRewardEvent(string id, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                receivedReward = true;
                CheckReceiveRewardProcess();
            }

            private void OnAdRevenuePaidEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = info.NetworkName,
                    adUnitName = info.AdUnitIdentifier,
                    adFormat = info.AdFormat,
                    value = info.Revenue,
                    currency = "USD",
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
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

        private class AppLovinBannerAd : BannerAd {
            private AppLovinAdvertisings client;
            private bool adativeBanner;
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
                        Rect bannerRect = MaxSdk.GetBannerLayout(Identifier);
                        return ConvertDpToPixels(bannerRect.height);
                    }
                    else {
                        return 0;
                    }
                }
            }

            public AppLovinBannerAd(AppLovinAdvertisings client, AdId id, bool adativeBanner) : base(id.Reset()) {
                this.client = client;
                this.adativeBanner = adativeBanner;
                this.placement = "NULL";

                MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoadedEvent;
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
                MaxSdkCallbacks.Banner.OnAdClickedEvent += OnAdClickedEvent;
                MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdCollapsedEvent;
                MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdExpandedEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            }

            public override bool Create() {
                if (Created) {
                    Log.Warning("[AppLovinBannerAd] Banner ad has created!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AppLovinBannerAd] Create ad failed! Banner ad id is null or empty!");
                    return false;
                }

                Created = true;

                MaxSdk.CreateBanner(Identifier, ToAppLovinBannerAdPosition(BannerAdPosition.Centered));
                MaxSdk.SetBannerExtraParameter(Identifier, "adaptive_banner", adativeBanner ? "true" : "false");
                MaxSdk.SetBannerBackgroundColor(Identifier, new Color(1, 1, 1, 0));

                MaxSdk.HideBanner(Identifier);

                return true;
            }

            public override bool Show(BannerAdPosition position, string placement, Vector2Int offset) {
                if (!Created) {
                    Log.Warning("[AppLovinBannerAd] Banner ad not been created!");
                    return false;
                }

                if (Position != position || Offset != offset) {

                    if (offset == Vector2Int.zero) {
                        MaxSdk.UpdateBannerPosition(Identifier, ToAppLovinBannerAdPosition(position));
                    }
                    else {
                        float screenDensity = MaxSdkUtils.GetScreenDensity();

                        float screenWidthDp = Screen.width / screenDensity;
                        float screenHeightDp = Screen.height / screenDensity;

                        float adWidthDp = 300;
                        float adHeightDp = 250;

                        float xOffsetDp = offset.x / screenDensity;
                        float yOffsetDp = offset.y / screenDensity;

                        float xPositionDp = 0;
                        float yPositionDp = 0;

                        switch (position) {
                            case BannerAdPosition.TopCenter:
                                xPositionDp = screenWidthDp / 2 - adWidthDp / 2;
                                yPositionDp = 0;
                                break;
                            case BannerAdPosition.TopLeft:
                                xPositionDp = 0;
                                yPositionDp = 0;
                                break;
                            case BannerAdPosition.TopRight:
                                xPositionDp = screenWidthDp - adWidthDp;
                                yPositionDp = 0;
                                break;
                            case BannerAdPosition.Centered:
                                xPositionDp = screenWidthDp / 2 - adWidthDp / 2;
                                yPositionDp = screenHeightDp / 2 - adHeightDp / 2;
                                break;
                            case BannerAdPosition.CenterLeft:
                                xPositionDp = 0;
                                yPositionDp = screenHeightDp / 2 - adHeightDp / 2;
                                break;
                            case BannerAdPosition.CenterRight:
                                xPositionDp = screenWidthDp - adWidthDp;
                                yPositionDp = screenHeightDp / 2 - adHeightDp / 2;
                                break;
                            case BannerAdPosition.BottomCenter:
                                xPositionDp = screenWidthDp / 2 - adWidthDp / 2;
                                yPositionDp = screenHeightDp - adHeightDp;
                                break;
                            case BannerAdPosition.BottomLeft:
                                xPositionDp = 0;
                                yPositionDp = screenHeightDp - adHeightDp;
                                break;
                            case BannerAdPosition.BottomRight:
                                xPositionDp = screenWidthDp - adWidthDp;
                                yPositionDp = screenHeightDp - adHeightDp;
                                break;
                            default:
                                break;
                        }

                        xPositionDp += xOffsetDp;
                        yPositionDp += yOffsetDp;

                        MaxSdk.UpdateBannerPosition(Identifier, xPositionDp, yPositionDp);
                    }
                }

                IsLoading = true;
                IsShowing = true;
                this.Position = position;
                this.Offset = offset;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                MaxSdk.ShowBanner(Identifier);
                return true;
            }

            public override bool Hide() {
                if (!IsShowing) {
                    Log.Warning("[AppLovinBannerAd] Banner ad not showing!");
                    return false;
                }

                IsShowing = false;
                MaxSdk.HideBanner(Identifier);
                return true;
            }

            public override bool Destroy() {
                if (!Created) {
                    Log.Warning("[AppLovinBannerAd] Banner ad not been created!");
                    return false;
                }

                Created = false;
                MaxSdk.DestroyBanner(Identifier);
                return true;
            }

            private void OnAdLoadedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdLoadFailedEvent(string id, MaxSdkBase.ErrorInfo error) {
                if (!Identifier.Equals(id)) return;

                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));
            }

            private void OnAdClickedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdExpandedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdCollapsedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdRevenuePaidEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = info.NetworkName,
                    adUnitName = info.AdUnitIdentifier,
                    adFormat = info.AdFormat,
                    value = info.Revenue,
                    currency = "USD",
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private MaxSdk.BannerPosition ToAppLovinBannerAdPosition(BannerAdPosition position) {
                switch (position) {
                    case BannerAdPosition.TopCenter: return MaxSdk.BannerPosition.TopCenter;
                    case BannerAdPosition.TopLeft: return MaxSdk.BannerPosition.TopLeft;
                    case BannerAdPosition.TopRight: return MaxSdk.BannerPosition.TopRight;
                    case BannerAdPosition.Centered: return MaxSdk.BannerPosition.Centered;
                    case BannerAdPosition.CenterLeft: return MaxSdk.BannerPosition.CenterLeft;
                    case BannerAdPosition.CenterRight: return MaxSdk.BannerPosition.CenterRight;
                    case BannerAdPosition.BottomCenter: return MaxSdk.BannerPosition.BottomCenter;
                    case BannerAdPosition.BottomLeft: return MaxSdk.BannerPosition.BottomLeft;
                    case BannerAdPosition.BottomRight: return MaxSdk.BannerPosition.BottomRight;
                    default: return MaxSdk.BannerPosition.Centered;
                }
            }

            public float ConvertDpToPixels(float dp) {
                return dp * (Screen.dpi / 160f);
            }

            public float ConverPixelsToDp(float pixel) {
                return pixel / (Screen.dpi / 160f);
            }
        }

        private class AppLovinMediumRectangleAd : MediumRectangleAd {
            private AppLovinAdvertisings client;
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
                        Rect adRect = MaxSdk.GetMRecLayout(Identifier);
                        return ConvertDpToPixels(adRect.height);
                    }
                    else {
                        return 0;
                    }
                }
            }

            public AppLovinMediumRectangleAd(AppLovinAdvertisings client, AdId id) : base(id.Reset()) {
                this.client = client;
                this.placement = "NULL";

                MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnAdLoadedEvent;
                MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
                MaxSdkCallbacks.MRec.OnAdClickedEvent += OnAdClickedEvent;
                MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnAdCollapsedEvent;
                MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnAdExpandedEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            }

            public override bool Create() {
                if (Created) {
                    Log.Warning("[AppLovinMediumRectangleAd] Medium rectangle ad has created!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AppLovinMediumRectangleAd] Create ad failed! Medium rectangle ad id is null or empty!");
                    return false;
                }

                Created = true;
                MaxSdk.CreateMRec(Identifier, ToAppLovinMRecAdPosition(MediumRectangleAdPosition.Centered));
                MaxSdk.HideBanner(Identifier);
                return true;
            }

            public override bool Show(MediumRectangleAdPosition position, string placement, Vector2Int offset) {
                if (!Created) {
                    Log.Warning("[AppLovinMediumRectangleAd] Medium rectangle ad not been created!");
                    return false;
                }

                if (Position != position || Offset != offset) {
                    if (offset == Vector2Int.zero) {
                        MaxSdk.UpdateMRecPosition(Identifier, ToAppLovinMRecAdPosition(position));
                    }
                    else {

                        float screenDensity = MaxSdkUtils.GetScreenDensity();

                        float screenWidthDp = Screen.width / screenDensity;
                        float screenHeightDp = Screen.height / screenDensity;

                        float adWidthDp = 300;
                        float adHeightDp = 250;

                        float xOffsetDp = offset.x / screenDensity;
                        float yOffsetDp = offset.y / screenDensity;

                        float xPositionDp = 0;
                        float yPositionDp = 0;

                        switch (position) {
                            case MediumRectangleAdPosition.TopCenter:
                                xPositionDp = screenWidthDp / 2 - adWidthDp / 2;
                                yPositionDp = 0;
                                break;
                            case MediumRectangleAdPosition.TopLeft:
                                xPositionDp = 0;
                                yPositionDp = 0;
                                break;
                            case MediumRectangleAdPosition.TopRight:
                                xPositionDp = screenWidthDp - adWidthDp;
                                yPositionDp = 0;
                                break;
                            case MediumRectangleAdPosition.Centered:
                                xPositionDp = screenWidthDp / 2 - adWidthDp / 2;
                                yPositionDp = screenHeightDp / 2 - adHeightDp / 2;
                                break;
                            case MediumRectangleAdPosition.CenterLeft:
                                xPositionDp = 0;
                                yPositionDp = screenHeightDp / 2 - adHeightDp / 2;
                                break;
                            case MediumRectangleAdPosition.CenterRight:
                                xPositionDp = screenWidthDp - adWidthDp;
                                yPositionDp = screenHeightDp / 2 - adHeightDp / 2;
                                break;
                            case MediumRectangleAdPosition.BottomCenter:
                                xPositionDp = screenWidthDp / 2 - adWidthDp / 2;
                                yPositionDp = screenHeightDp - adHeightDp;
                                break;
                            case MediumRectangleAdPosition.BottomLeft:
                                xPositionDp = 0;
                                yPositionDp = screenHeightDp - adHeightDp;
                                break;
                            case MediumRectangleAdPosition.BottomRight:
                                xPositionDp = screenWidthDp - adWidthDp;
                                yPositionDp = screenHeightDp - adHeightDp;
                                break;
                            default:
                                break;
                        }

                        xPositionDp += xOffsetDp;
                        yPositionDp += yOffsetDp;

                        MaxSdk.UpdateMRecPosition(Identifier, xPositionDp, yPositionDp);
                    }
                }

                IsLoading = true;
                IsShowing = true;
                this.Offset = offset;
                this.Position = position;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                MaxSdk.ShowMRec(Identifier);
                MaxSdk.StartMRecAutoRefresh(Identifier);
                return true;
            }

            public override bool Hide() {
                if (!IsShowing) {
                    Log.Warning("[AppLovinMediumRectangleAd] Medium rectangle ad not showing!");
                    return false;
                }

                IsShowing = false;
                MaxSdk.HideMRec(Identifier);
                return true;
            }

            public override bool Destroy() {
                if (!Created) {
                    Log.Warning("[AppLovinMediumRectangleAd] Medium rectangle ad not been created!");
                    return false;
                }

                Created = false;
                MaxSdk.DestroyMRec(Identifier);
                return true;
            }

            private void OnAdLoadedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                IsLoading = false;
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdLoadFailedEvent(string id, MaxSdkBase.ErrorInfo error) {
                if (!Identifier.Equals(id)) return;

                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));
            }

            private void OnAdClickedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdExpandedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdCollapsedEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, id)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, info.ToString()));
            }

            private void OnAdRevenuePaidEvent(string id, MaxSdkBase.AdInfo info) {
                if (!Identifier.Equals(id)) return;

                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = info.NetworkName,
                    adUnitName = info.AdUnitIdentifier,
                    adFormat = info.AdFormat,
                    value = info.Revenue,
                    currency = "USD",
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private MaxSdkBase.AdViewPosition ToAppLovinMRecAdPosition(MediumRectangleAdPosition position) {
                switch (position) {
                    case MediumRectangleAdPosition.TopCenter: return MaxSdkBase.AdViewPosition.TopCenter;
                    case MediumRectangleAdPosition.TopLeft: return MaxSdkBase.AdViewPosition.TopLeft;
                    case MediumRectangleAdPosition.TopRight: return MaxSdkBase.AdViewPosition.TopRight;
                    case MediumRectangleAdPosition.Centered: return MaxSdkBase.AdViewPosition.Centered;
                    case MediumRectangleAdPosition.CenterLeft: return MaxSdkBase.AdViewPosition.CenterLeft;
                    case MediumRectangleAdPosition.CenterRight: return MaxSdkBase.AdViewPosition.CenterRight;
                    case MediumRectangleAdPosition.BottomCenter: return MaxSdkBase.AdViewPosition.BottomCenter;
                    case MediumRectangleAdPosition.BottomLeft: return MaxSdkBase.AdViewPosition.BottomLeft;
                    case MediumRectangleAdPosition.BottomRight: return MaxSdkBase.AdViewPosition.BottomRight;
                    default: return MaxSdkBase.AdViewPosition.Centered;
                }
            }

            public float ConvertDpToPixels(float dp) {
                return dp * MaxSdkUtils.GetScreenDensity();
            }

            public float ConverPixelsToDp(float pixel) {
                return pixel / MaxSdkUtils.GetScreenDensity();
            }
        }
    }
#endif


    [CategoryMenu("AppLovin Advertisings")]
    [System.Serializable]
    public class AppLovinAdServiceProvider : AdServiceProvider {
        public override AdService GetService() {
#if APPLOVIN
            return new AppLovinAdvertisings();
#else
            return null;
#endif
        }
    }
}

