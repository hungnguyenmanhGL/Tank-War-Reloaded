using System;
using UnityEngine;
using System.Collections.Generic;
using HAVIGAME.Services.Advertisings;

#if ADMOB
using GoogleMobileAds.Api;
#endif

namespace HAVIGAME.Plugins.AdMob {

#if ADMOB
    public class AdMobAdvertisings : AdService {

        private AdMobAppOpenAd[] appOpenAds;
        private AdMobInterstitialAd[] interstitialAds;
        private AdMobRewardedAd[] rewardedAds;
        private AdMobBannerAd[] bannerAds;
        private AdMobMediumRectangleAd[] mediumRectangleAds;
        private AdMobRewardedInterstitialAd[] rewardedInterstitialAds;
#if ADMOB_NATIVE
        private AdMobNativeAd[] nativeAds;
#endif

        public override AdNetwork Network => AdNetwork.AdMob;
        public override AdUnit AdUnitSupported => AdUnit.AdMobAdUnits;
        protected override Services.Advertisings.AppOpenAd[] AppOpenAds => appOpenAds;
        protected override Services.Advertisings.InterstitialAd[] InterstitialAds => interstitialAds;
        protected override Services.Advertisings.RewardedAd[] RewardedAds => rewardedAds;
        protected override Services.Advertisings.BannerAd[] BannerAds => bannerAds;
        protected override Services.Advertisings.RewardedInterstitialAd[] RewardedInterstitialAds => rewardedInterstitialAds;
        protected override Services.Advertisings.MediumRectangleAd[] MediumRectangleAds => mediumRectangleAds;
#if ADMOB_NATIVE
        protected override Services.Advertisings.NativeAd[] NativeAds => nativeAds;
#else
        protected override Services.Advertisings.NativeAd[] NativeAds => null;
#endif

        public override void Initialize() {
            if (InitializeEvent.IsRunning) {
                Log.Warning("[AdMobAdvertisings] AdMob advertisings is running with initialize state {0}.", IsInitialized);
                return;
            }

            AdMobManager.initializeEvent.AddListener(OnAdMobInitializeCallback);
        }

        private void OnAdMobInitializeCallback(bool isInitialized) {
            if (isInitialized) {
                AdMobSettings settings = AdMobSettings.Instance;

                if (settings.AppOpenAdIds != null && settings.AppOpenAdIds.Length > 0) {
                    appOpenAds = new AdMobAppOpenAd[settings.AppOpenAdIds.Length];

                    for (int i = 0; i < settings.AppOpenAdIds.Length; i++) {
                        AdMobAppOpenAd appOpenAd = new AdMobAppOpenAd(
                            this,
                            settings.AppOpenAdIds[i],
                            settings.AppOpenAdScreenOrientation);

                        Log.Debug("[AdMobAdvertisings] Create app open ad {0}", appOpenAd);
                        RegisterAdEvents(appOpenAd);
                        appOpenAds[i] = appOpenAd;
                        appOpenAd.Load();
                    }
                }

                if (settings.InterstitialAdIds != null && settings.InterstitialAdIds.Length > 0) {
                    interstitialAds = new AdMobInterstitialAd[settings.InterstitialAdIds.Length];

                    for (int i = 0; i < settings.InterstitialAdIds.Length; i++) {
                        AdMobInterstitialAd interstitialAd = new AdMobInterstitialAd(
                            this,
                            settings.InterstitialAdIds[i]);

                        Log.Debug("[AdMobAdvertisings] Create interstitial ad {0}", interstitialAd);
                        RegisterAdEvents(interstitialAd);
                        interstitialAds[i] = interstitialAd;
                        interstitialAd.Load();
                    }
                }

                if (settings.RewardedAdIds != null && settings.RewardedAdIds.Length > 0) {
                    rewardedAds = new AdMobRewardedAd[settings.RewardedAdIds.Length];

                    for (int i = 0; i < settings.RewardedAdIds.Length; i++) {
                        AdMobRewardedAd rewardedAd = new AdMobRewardedAd(this, settings.RewardedAdIds[i]);
                        Log.Debug("[AdMobAdvertisings] Create rewarded ad {0}", rewardedAd);
                        RegisterAdEvents(rewardedAd);
                        rewardedAds[i] = rewardedAd;
                        rewardedAd.Load();
                    }
                }

                if (settings.BannerAdIds != null && settings.BannerAdIds.Length > 0) {
                    bannerAds = new AdMobBannerAd[settings.BannerAdIds.Length];

                    for (int i = 0; i < settings.BannerAdIds.Length; i++) {
                        AdMobBannerAd bannerAd = new AdMobBannerAd(this, settings.BannerAdIds[i]);
                        Log.Debug("[AdMobAdvertisings] Create banner ad {0}", bannerAd);
                        RegisterAdEvents(bannerAd);
                        bannerAds[i] = bannerAd;
                        bannerAd.Create();
                    }
                }

                if (settings.MediumRectangleAdIds != null && settings.MediumRectangleAdIds.Length > 0) {
                    mediumRectangleAds = new AdMobMediumRectangleAd[settings.MediumRectangleAdIds.Length];

                    for (int i = 0; i < settings.MediumRectangleAdIds.Length; i++) {
                        AdMobMediumRectangleAd mediumRectangleAd = new AdMobMediumRectangleAd(this, settings.MediumRectangleAdIds[i]);
                        Log.Debug("[AdMobAdvertisings] Create medium rectangle ad {0}", mediumRectangleAd);
                        RegisterAdEvents(mediumRectangleAd);
                        mediumRectangleAds[i] = mediumRectangleAd;
                        mediumRectangleAd.Create();
                    }
                }


                if (settings.RewardedInterstitialAdIds != null && settings.RewardedInterstitialAdIds.Length > 0) {
                    rewardedInterstitialAds = new AdMobRewardedInterstitialAd[settings.RewardedInterstitialAdIds.Length];

                    for (int i = 0; i < settings.RewardedInterstitialAdIds.Length; i++) {
                        AdMobRewardedInterstitialAd rewardedInterstitialAd = new AdMobRewardedInterstitialAd(this, settings.RewardedInterstitialAdIds[i]);
                        Log.Debug("[AdMobAdvertisings] Create rewarded interstitial ad {0}", rewardedInterstitialAd);
                        RegisterAdEvents(rewardedInterstitialAd);
                        rewardedInterstitialAds[i] = rewardedInterstitialAd;
                        rewardedInterstitialAd.Load();
                    }
                }

#if ADMOB_NATIVE
                if (settings.NativeAdIds != null && settings.NativeAdIds.Length > 0) {
                    nativeAds = new AdMobNativeAd[settings.NativeAdIds.Length];

                    for (int i = 0; i < settings.NativeAdIds.Length; i++) {
                        AdMobNativeAd nativeAd = new AdMobNativeAd(this, settings.NativeAdIds[i]);
                        Log.Debug("[AdMobAdvertisings] Create native ad {0}", nativeAd);
                        RegisterAdEvents(nativeAd);
                        nativeAds[i] = nativeAd;
                        nativeAd.Load();
                    }
                }
#endif

                Log.Info("[AdMobAdvertisings] AdMob advertisings initialize completed.");
                InitializeEvent.Invoke(true);

                Database.Unload(settings);
            }
            else {
                Log.Error("[AdMobAdvertisings] AdMob advertisings initialize failed because AdMob initialize failed");
                InitializeEvent.Invoke(false);
            }
        }

        private class AdMobAppOpenAd : Services.Advertisings.AppOpenAd {
            private AdMobAdvertisings client;
            private ScreenOrientation screenOrientation;
            private string placement;
            private Action onCompleted;
            private int retry;
            private GoogleMobileAds.Api.AppOpenAd ad;
            private DateTime loadTime;
            public override AdService Client => client;
            public override bool IsReady {
                get {
                    if (ad != null) {
                        if ((DateTime.UtcNow - loadTime).TotalHours >= 4) {
                            Log.Warning("[AdMobAppOpenAd] App open ad has expried!");
                            ad.Destroy();
                            ad = null;
                            Reload(true);
                            return false;
                        }
                        else {
                            bool canShow = ad.CanShowAd();

                            if (canShow) {
                                return true;
                            }
                            else {
                                Log.Warning("[AdMobAppOpenAd] App open ad can only be shown once per load!");
                                ad.Destroy();
                                ad = null;
                                Reload(true);
                                return false;
                            }
                        }
                    }
                    return false;
                }
            }

            public AdMobAppOpenAd(AdMobAdvertisings client, AdId id, ScreenOrientation screenOrientation) : base(id) {
                this.client = client;
                this.screenOrientation = screenOrientation;
                this.retry = 0;
                this.placement = "NULL";
                ad = null;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AdMobAppOpenAd] Load failed! App open ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AdMobAppOpenAd] Load failed! App open ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AdMobAppOpenAd] Load failed! App open ad id is null or empty!");
                    return false;
                }

                IsLoading = true;
                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                AdRequest request = new AdRequest();
                GoogleMobileAds.Api.AppOpenAd.Load(Identifier, screenOrientation, request, this.OnAdLoadEvent);
                loadTime = DateTime.UtcNow;
                return true;
            }

            public override bool Show(Action onCompleted, string placement) {
                if (!IsReady) {
                    Log.Warning("[AdMobAppOpenAd] Show failed! App open ad not avaliable!");
                    return false;
                }

                if (IsShowing) {
                    Log.Warning("[AdMobAppOpenAd] Show failed! App open ad is showing!");
                    return false;
                }

                IsShowing = true;
                this.onCompleted = onCompleted;
                this.placement = placement;

                ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
                ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
                ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                ad.OnAdPaid += OnAdPaid;
                ad.OnAdImpressionRecorded += OnAdImpressionRecorded;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, base.Identifier)
                    .Add(AdEventArgs.placement, placement));

                ad.Show();
                return true;
            }

            private void OnAdLoadEvent(GoogleMobileAds.Api.AppOpenAd ad, LoadAdError error) {
                IsLoading = false;

                if (error == null) {
                    this.ad = ad;
                    InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                          .Add(AdEventArgs.id, base.Identifier)
                          .Add(AdEventArgs.placement, placement));

                    retry = 0;
                    loadTime = DateTime.UtcNow;
                }
                else {
                    InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                          .Add(AdEventArgs.id, base.Identifier)
                          .Add(AdEventArgs.placement, placement)
                          .Add(AdEventArgs.error, error.ToString()));

                    retry++;
                    float delay = 2 * Math.Min(6, retry);
                    Invoke(Reload, delay);
                }
            }

            private void OnAdFullScreenContentOpened() {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                      .Add(AdEventArgs.id, base.Identifier)
                      .Add(AdEventArgs.placement, placement));
            }

            private void OnAdPaid(AdValue value) {
                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = client.Network.ToString(),
                    adUnitName = base.Identifier,
                    adFormat = base.Unit.ToString(),
                    value = value.Value,
                    currency = value.CurrencyCode,
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private void OnAdFullScreenContentFailed(AdError error) {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;
                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                      .Add(AdEventArgs.id, base.Identifier)
                      .Add(AdEventArgs.placement, placement)
                      .Add(AdEventArgs.error, error.ToString()));


                Reload(true);
            }

            private void OnAdImpressionRecorded() {
            }

            private void OnAdFullScreenContentClosed() {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                      .Add(AdEventArgs.id, base.Identifier)
                      .Add(AdEventArgs.placement, placement));

                Reload(true);
            }
        }

        private class AdMobInterstitialAd : Services.Advertisings.InterstitialAd {
            private AdMobAdvertisings client;
            private string placement;
            private Action onCompleted;
            private int retry;
            private GoogleMobileAds.Api.InterstitialAd ad;
            public override AdService Client => client;
            public override bool IsReady {
                get {
                    if (ad != null) {
                        bool canShow = ad.CanShowAd();

                        if (canShow) {
                            return true;
                        }
                        else {
                            Log.Warning("[AdMobInterstitialAd] Interstitial ad can only be shown once per load!");
                            ad.Destroy();
                            ad = null;
                            Reload(true);
                            return false;
                        }
                    }
                    return false;
                }
            }

            public AdMobInterstitialAd(AdMobAdvertisings client, AdId id) : base(id) {
                this.client = client;
                this.retry = 0;
                this.placement = "NULL";
                ad = null;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AdMobInterstitialAd] Load failed! Interstitial ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AdMobInterstitialAd] Load failed! Interstitial ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AdMobInterstitialAd] Load failed! Interstitial ad id is null or empty!");
                    return false;
                }


                IsLoading = true;

                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                AdRequest request = new AdRequest();
                GoogleMobileAds.Api.InterstitialAd.Load(Identifier, request, OnAdLoadEvent);
                return true;
            }

            public override bool Show(Action onCompleted, string placement) {
                if (!IsReady) {
                    Log.Warning("[AdMobInterstitialAd] Show failed! Interstitial ad not avaliable!");
                    return false;
                }

                if (IsShowing) {
                    Log.Warning("[AdMobInterstitialAd] Show failed! Interstitial ad is showing!");
                    return false;
                }

                IsShowing = true;
                this.onCompleted = onCompleted;
                this.placement = placement;

                ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
                ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
                ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                ad.OnAdClicked += OnAdClicked;
                ad.OnAdPaid += OnAdPaid;
                ad.OnAdImpressionRecorded += OnAdImpressionRecorded;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                ad.Show();
                return true;
            }

            private void OnAdLoadEvent(GoogleMobileAds.Api.InterstitialAd ad, LoadAdError error) {
                IsLoading = false;

                if (error == null) {
                    this.ad = ad;
                    InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                          .Add(AdEventArgs.id, Identifier)
                          .Add(AdEventArgs.placement, placement));

                    retry = 0;
                }
                else {
                    InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                          .Add(AdEventArgs.id, Identifier)
                          .Add(AdEventArgs.placement, placement)
                          .Add(AdEventArgs.error, error.ToString()));

                    retry++;
                    float delay = 2 * Math.Min(6, retry);
                    Invoke(Reload, delay);
                }
            }

            private void OnAdFullScreenContentOpened() {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdFullScreenContentFailed(AdError error) {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.info, error.ToString())
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnAdClicked() {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdPaid(AdValue value) {
                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = client.Network.ToString(),
                    adUnitName = Identifier,
                    adFormat = Unit.ToString(),
                    value = value.Value,
                    currency = value.CurrencyCode,
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private void OnAdImpressionRecorded() {
            }

            private void OnAdFullScreenContentClosed() {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                Reload(true);
            }
        }

        private class AdMobRewardedAd : Services.Advertisings.RewardedAd {
            private AdMobAdvertisings client;
            private string placement;
            private Action onCompleted;
            private Action onFailed;
            private int retry;
            private bool receivedReward;
            private GoogleMobileAds.Api.RewardedAd ad;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    if (ad != null) {
                        bool canShow = ad.CanShowAd();

                        if (canShow) {
                            return true;
                        }
                        else {
                            Log.Warning("[AdMobRewardedAd] Rewarded ad can only be shown once per load!");
                            ad.Destroy();
                            ad = null;
                            Reload(true);
                            return false;
                        }
                    }
                    return false;
                }
            }

            public AdMobRewardedAd(AdMobAdvertisings client, AdId id) : base(id) {
                this.client = client;
                this.retry = 0;
                this.placement = "NULL";
                this.receivedReward = false;
                ad = null;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AdMobRewardedAd] Load failed! Rewarded ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AdMobRewardedAd] Load failed! Rewarded ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AdMobRewardedAd] Load failed! Rewarded ad id is null or empty!");
                    return false;
                }

                IsLoading = true;

                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                AdRequest request = new AdRequest();
                GoogleMobileAds.Api.RewardedAd.Load(Identifier, request, OnAdLoadEvent);
                return true;
            }

            public override bool Show(Action onCompleted, Action onFailed, string placement) {
                if (!IsReady) {
                    Log.Warning("[AdMobRewardedAd] Show failed! Rewarded ad not avaliable!");
                    return false;
                }

                if (IsShowing) {
                    Log.Warning("[AdMobRewardedAd] Show failed! Rewarded ad is showing!");
                    return false;
                }

                IsShowing = true;
                receivedReward = false;
                this.onCompleted = onCompleted;
                this.placement = placement;

                ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
                ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
                ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                ad.OnAdClicked += OnAdClicked;
                ad.OnAdPaid += OnAdPaid;
                ad.OnAdImpressionRecorded += OnAdImpressionRecorded;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                ad.Show(OnUserRewardEarned);
                return true;
            }

            private void OnAdLoadEvent(GoogleMobileAds.Api.RewardedAd ad, LoadAdError error) {
                IsLoading = false;

                if (error == null) {
                    this.ad = ad;
                    InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                          .Add(AdEventArgs.id, Identifier)
                          .Add(AdEventArgs.placement, placement));

                    retry = 0;
                }
                else {
                    InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                          .Add(AdEventArgs.id, Identifier)
                          .Add(AdEventArgs.placement, placement)
                          .Add(AdEventArgs.error, error.ToString()));

                    retry++;
                    float delay = 2 * Math.Min(6, retry);
                    Invoke(Reload, delay);
                }
            }

            private void OnAdFullScreenContentOpened() {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdFullScreenContentFailed(AdError error) {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                receivedReward = false;
                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnAdClicked() {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdPaid(AdValue value) {
                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = client.Network.ToString(),
                    adUnitName = Identifier,
                    adFormat = Unit.ToString(),
                    value = value.Value,
                    currency = value.CurrencyCode,
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private void OnAdImpressionRecorded() {
            }

            private void OnUserRewardEarned(GoogleMobileAds.Api.Reward reward) {
                receivedReward = true;
                CheckReceiveRewardProcess();
            }
            private void OnAdFullScreenContentClosed() {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;
                CheckReceiveRewardProcess();

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                Reload(true);
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

        private class AdMobBannerAd : BannerAd {
            private AdMobAdvertisings client;
            private string placement;
            private BannerView bannerView;
            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return true;
                }
            }
            public override float Height {
                get {
                    if (IsShowing) {
                        return bannerView.GetHeightInPixels();
                    }
                    return 0;
                }
            }

            public AdMobBannerAd(AdMobAdvertisings client, AdId id) : base(id.Reset()) {
                this.client = client;
                this.placement = "NULL";
                this.bannerView = null;
            }

            public override bool Create() {
                if (Created) {
                    Log.Warning("[AdMobBannerAd] Banner ad has created!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AdMobBannerAd] Create ad failed! Banner ad id is null or empty!");
                    return false;
                }

                Created = true;

                bannerView = new BannerView(Identifier, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), ToAdMobBannerAdPosition(BannerAdPosition.Centered));

                bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
                bannerView.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
                bannerView.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                bannerView.OnAdClicked += OnAdClicked;
                bannerView.OnAdPaid += OnAdPaid;
                bannerView.OnAdImpressionRecorded += OnAdImpressionRecorded;

                AdRequest request = new AdRequest();
                bannerView.LoadAd(request);

                bannerView.Hide();

                return true;
            }

            public override bool Show(BannerAdPosition position, string placement, Vector2Int offset) {
                if (!Created) {
                    Log.Warning("[AdMobBannerAd] Banner ad not been created!");
                    return false;
                }

                if (Position != position || Offset != offset) {
                    if (offset == Vector2Int.zero) {
                        bannerView.SetPosition(ToAdMobBannerAdPosition(position));
                    }
                    else {

                        float screenDensity = Screen.dpi / 160f;

                        float screenWidthDp = Screen.width / screenDensity;
                        float screenHeightDp = Screen.height / screenDensity;

                        float adWidthDp = bannerView.GetHeightInPixels() / screenDensity;
                        float adHeightDp = bannerView.GetHeightInPixels() / screenDensity;

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

                        bannerView.SetPosition(Mathf.FloorToInt(xPositionDp), Mathf.FloorToInt(yPositionDp));
                    }
                }

                IsShowing = true;
                IsLoading = true;
                this.Position = position;
                this.Offset = offset;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                bannerView.Show();

                return true;
            }

            public override bool Hide() {
                if (!IsShowing) {
                    Log.Warning("[AdMobBannerAd] Banner ad not showing!");
                    return false;
                }

                IsShowing = false;
                bannerView.Hide();
                return true;
            }

            public override bool Destroy() {
                if (!Created) {
                    Log.Warning("[AdMobBannerAd] Banner ad not been created!");
                    return false;
                }

                Created = false;
                bannerView.Destroy();
                bannerView = null;
                return true;
            }

            private void OnBannerAdLoaded() {
                IsLoading = false;
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));
            }

            private void OnAdFullScreenContentOpened() {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdClicked() {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdPaid(AdValue value) {
                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = client.Network.ToString(),
                    adUnitName = Identifier,
                    adFormat = Unit.ToString(),
                    value = value.Value,
                    currency = value.CurrencyCode,
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private void OnAdImpressionRecorded() {
            }

            private void OnAdFullScreenContentClosed() {
                IsShowing = false;
                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private AdPosition ToAdMobBannerAdPosition(BannerAdPosition position) {
                switch (position) {
                    case BannerAdPosition.TopCenter: return AdPosition.Top;
                    case BannerAdPosition.TopLeft: return AdPosition.TopLeft;
                    case BannerAdPosition.TopRight: return AdPosition.TopRight;
                    case BannerAdPosition.Centered: return AdPosition.Center;
                    case BannerAdPosition.CenterLeft: return AdPosition.Center;
                    case BannerAdPosition.CenterRight: return AdPosition.Center;
                    case BannerAdPosition.BottomCenter: return AdPosition.Bottom;
                    case BannerAdPosition.BottomLeft: return AdPosition.BottomLeft;
                    case BannerAdPosition.BottomRight: return AdPosition.BottomRight;
                    default: return AdPosition.Center;
                }
            }
        }

        private class AdMobMediumRectangleAd : MediumRectangleAd {
            private AdMobAdvertisings client;
            private string placement;
            private BannerView bannerView;
            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return true;
                }
            }
            public override float Height {
                get {
                    if (IsShowing) {
                        return bannerView.GetHeightInPixels();
                    }
                    return 0;
                }
            }

            public AdMobMediumRectangleAd(AdMobAdvertisings client, AdId id) : base(id.Reset()) {
                this.client = client;
                this.placement = "NULL";
                this.bannerView = null;
            }

            public override bool Create() {
                if (Created) {
                    Log.Warning("[AdMobMediumRectangleAd] Medium rectangle ad has created!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AdMobMediumRectangleAd] Create ad failed! Medium rectangle ad id is null or empty!");
                    return false;
                }

                Created = true;

                bannerView = new BannerView(Identifier, AdSize.MediumRectangle, ToAdMobBannerAdPosition(MediumRectangleAdPosition.Centered));

                bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
                bannerView.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
                bannerView.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                bannerView.OnAdClicked += OnAdClicked;
                bannerView.OnAdPaid += OnAdPaid;
                bannerView.OnAdImpressionRecorded += OnAdImpressionRecorded;

                AdRequest request = new AdRequest();
                bannerView.LoadAd(request);

                bannerView.Hide();

                return true;
            }

            public override bool Show(MediumRectangleAdPosition position, string placement, Vector2Int offset) {
                if (!Created) {
                    Log.Warning("[AdMobMediumRectangleAd] Medium rectangle ad not been created!");
                    return false;
                }

                if (Position != position || Offset != offset) {
                    if (offset == Vector2Int.zero) {
                        bannerView.SetPosition(ToAdMobBannerAdPosition(position));
                    }
                    else {

                        float screenDensity = Screen.dpi / 160f;

                        float screenWidthDp = Screen.width / screenDensity;
                        float screenHeightDp = Screen.height / screenDensity;

                        float adWidthDp = bannerView.GetHeightInPixels() / screenDensity;
                        float adHeightDp = bannerView.GetHeightInPixels() / screenDensity;

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

                        bannerView.SetPosition(Mathf.FloorToInt(xPositionDp), Mathf.FloorToInt(yPositionDp));
                    }
                }

                IsShowing = true;
                IsLoading = true;
                this.Position = position;
                this.Offset = offset;
                this.placement = placement;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                bannerView.Show();

                return true;
            }

            public override bool Hide() {
                if (!IsShowing) {
                    Log.Warning("[AdMobMediumRectangleAd] Medium rectangle ad not showing!");
                    return false;
                }

                IsShowing = false;
                bannerView.Hide();
                return true;
            }

            public override bool Destroy() {
                if (!Created) {
                    Log.Warning("[AdMobMediumRectangleAd] Medium rectangle ad not been created!");
                    return false;
                }

                Created = false;
                bannerView.Destroy();
                bannerView = null;
                return true;
            }

            private void OnBannerAdLoaded() {
                IsLoading = false;
                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));
            }

            private void OnAdFullScreenContentOpened() {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdClicked() {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdPaid(AdValue value) {
                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = client.Network.ToString(),
                    adUnitName = Identifier,
                    adFormat = Unit.ToString(),
                    value = value.Value,
                    currency = value.CurrencyCode,
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private void OnAdImpressionRecorded() {
            }

            private void OnAdFullScreenContentClosed() {
                IsShowing = false;
                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private AdPosition ToAdMobBannerAdPosition(MediumRectangleAdPosition position) {
                switch (position) {
                    case MediumRectangleAdPosition.TopCenter: return AdPosition.Top;
                    case MediumRectangleAdPosition.TopLeft: return AdPosition.TopLeft;
                    case MediumRectangleAdPosition.TopRight: return AdPosition.TopRight;
                    case MediumRectangleAdPosition.Centered: return AdPosition.Center;
                    case MediumRectangleAdPosition.CenterLeft: return AdPosition.Center;
                    case MediumRectangleAdPosition.CenterRight: return AdPosition.Center;
                    case MediumRectangleAdPosition.BottomCenter: return AdPosition.Bottom;
                    case MediumRectangleAdPosition.BottomLeft: return AdPosition.BottomLeft;
                    case MediumRectangleAdPosition.BottomRight: return AdPosition.BottomRight;
                    default: return AdPosition.Center;
                }
            }
        }


#if ADMOB_NATIVE
        private class AdMobNativeAd : Services.Advertisings.NativeAd {
            private AdMobAdvertisings client;
            private NativeAdView view;
            private string placement;
            private int retry;
            private GoogleMobileAds.Api.NativeAd ad;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    return ad != null && !IsShowing;
                }
            }

            public AdMobNativeAd(AdMobAdvertisings client, AdId id) : base(id) {
                this.client = client;
                this.retry = 0;
                this.placement = "NULL";
                ad = null;

                AddElemnet(new StringElement(ElementType.Name, GetHeadlineText, RegisterHeadlineText));
                AddElemnet(new TextureElement(ElementType.Image, GetImageTexture, RegisterImageTexture));
                AddElemnet(new StringElement(ElementType.Description, GetBodyText, RegisterBodyText));
                AddElemnet(new TextureElement(ElementType.Icon, GetIconTexture, RegisterIconTexture));
                AddElemnet(new TextureElement(ElementType.AdChoicesLogo, GetAdChoicesLogoTexture, RegisterAdChoicesLogoTexture));
                AddElemnet(new StringElement(ElementType.CTA, GetCallToActionText, RegisterCallToActionText));
                AddElemnet(new DoubleElement(ElementType.StarRating, GetStarRating, RegisterStarRating));
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AdMobNativeAd] Load failed! Native ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AdMobNativeAd] Load failed! Native ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AdMobNativeAd] Load failed! Native ad id is null or empty!");
                    return false;
                }


                IsLoading = true;

                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                AdLoader adLoader = new AdLoader.Builder(Identifier).ForNativeAd().Build();
                AdRequest request = new AdRequest();

                adLoader.OnNativeAdLoaded += OnNativeAdLoaded;
                adLoader.OnAdFailedToLoad += OnAdFailedToLoad;
                adLoader.OnNativeAdOpening += OnNativeAdOpening;
                adLoader.OnNativeAdClicked += OnNativeAdClicked;
                adLoader.OnNativeAdClosed += OnNativeAdClosed;
                adLoader.OnNativeAdImpression += OnNativeAdImpression;

                adLoader.LoadAd(request);

                return true;
            }

            public override bool Show(NativeAdView view, string placement) {
                if (!IsReady) {
                    Log.Warning("[AdMobNativeAd] Show failed! Native ad not avaliable!");
                    return false;
                }

                if (IsShowing) {
                    Log.Warning("[AdMobNativeAd] Show failed! Native ad is showing!");
                    return false;
                }

                IsShowing = true;
                this.placement = placement;
                this.view = view;

                this.view.onNativeAdDisplayed += OnNativeAdDisplayed;
                this.view.onNativeAdDisplayFailed += OnNativeAdDisplayFailed;
                this.view.onNativeAdClosed += OnNativeAdClosed;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                this.view.Show(this);

                return true;
            }

            public override bool Hide() {
                if (!IsShowing) {
                    Log.Warning("[AdMobNativeAd] Native ad not showing!");
                    return false;
                }

                IsShowing = false;
                if (view != null) {
                    view.Hide();
                    view = null;
                }

                return true;
            }


            #region Elements
            private string GetHeadlineText() {
                if (ad != null) {
                    return ad.GetHeadlineText();
                }
                return null;
            }
            private bool RegisterHeadlineText(GameObject gameObject) {
                if (ad != null) {
                    return ad.RegisterHeadlineTextGameObject(gameObject);
                }
                return false;
            }
            private Texture2D GetImageTexture() {
                if (ad != null) {
                    List<Texture2D> result = ad.GetImageTextures();
                    if (result != null && result.Count > 0) {
                        return result[0];
                    }
                }
                return null;
            }
            private bool RegisterImageTexture(GameObject gameObject) {
                if (ad != null) {
                    return ad.RegisterImageGameObjects(new List<GameObject>() { gameObject }) > 0;
                }
                return false;
            }
            private string GetBodyText() {
                if (ad != null) {
                    return ad.GetBodyText();
                }
                return null;
            }
            private bool RegisterBodyText(GameObject gameObject) {
                if (ad != null) {
                    return ad.RegisterBodyTextGameObject(gameObject);
                }
                return false;
            }
            private Texture2D GetIconTexture() {
                if (ad != null) {
                    return ad.GetIconTexture();
                }
                return null;
            }
            private bool RegisterIconTexture(GameObject gameObject) {
                if (ad != null) {
                    return ad.RegisterIconImageGameObject(gameObject);
                }
                return false;
            }
            private Texture2D GetAdChoicesLogoTexture() {
                if (ad != null) {
                    return ad.GetAdChoicesLogoTexture();
                }
                return null;
            }
            private bool RegisterAdChoicesLogoTexture(GameObject gameObject) {
                if (ad != null) {
                    return ad.RegisterAdChoicesLogoGameObject(gameObject);
                }
                return false;
            }
            private string GetCallToActionText() {
                if (ad != null) {
                    return ad.GetCallToActionText();
                }
                return null;
            }
            private bool RegisterCallToActionText(GameObject gameObject) {
                if (ad != null) {
                    return ad.RegisterCallToActionGameObject(gameObject);
                }
                return false;
            }
            private double GetStarRating() {
                if (ad != null) {
                    return ad.GetStarRating();
                }
                return 4.5d;
            }
            private bool RegisterStarRating(GameObject gameObject) {
                return true;
            }
            #endregion

            private void OnNativeAdLoaded(object sender, NativeAdEventArgs e) {
                IsLoading = false;

                this.ad = e.nativeAd;
                this.ad.OnPaidEvent += OnPaidEvent;

                InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                retry = 0;
            }

            private void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) {
                IsLoading = false;

                InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement)
                      .Add(AdEventArgs.error, e.LoadAdError.ToString()));

                retry++;
                float delay = 2 * Math.Min(6, retry);
                Invoke(Reload, delay);
            }

            private void OnNativeAdDisplayed() {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnNativeAdDisplayFailed(string error) {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;

                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnNativeAdClosed() {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                Reload(true);
            }

            private void OnNativeAdOpening(object sender, System.EventArgs e) {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnNativeAdClicked(object sender, System.EventArgs e) {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnNativeAdClosed(object sender, System.EventArgs e) {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                Reload(true);
            }

            private void OnNativeAdImpression(object sender, System.EventArgs e) {

            }

            private void OnPaidEvent(object sender, AdValueEventArgs e) {
                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = client.Network.ToString(),
                    adUnitName = Identifier,
                    adFormat = Unit.ToString(),
                    value = e.AdValue.Value,
                    currency = e.AdValue.CurrencyCode,
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }
        }
#endif

        private class AdMobRewardedInterstitialAd : Services.Advertisings.RewardedInterstitialAd {
            private AdMobAdvertisings client;
            private string placement;
            private Action onCompleted;
            private Action onFailed;
            private int retry;
            private bool receivedReward;
            private GoogleMobileAds.Api.RewardedInterstitialAd ad;

            public override AdService Client => client;
            public override bool IsReady {
                get {
                    if (ad != null) {
                        bool canShow = ad.CanShowAd();

                        if (canShow) {
                            return true;
                        }
                        else {
                            Log.Warning("[AdMobRewardedAd] Rewarded ad can only be shown once per load!");
                            ad.Destroy();
                            ad = null;
                            Reload(true);
                            return false;
                        }
                    }
                    return false;
                }
            }

            public AdMobRewardedInterstitialAd(AdMobAdvertisings client, AdId id) : base(id) {
                this.client = client;
                this.retry = 0;
                this.placement = "NULL";
                this.receivedReward = false;
                ad = null;
            }

            public override bool Load() {
                if (IsReady) {
                    Log.Warning("[AdMobRewardedInterstitialAd] Load failed! Rewarded interstitial ad is ready!");
                    return false;
                }

                if (IsLoading) {
                    Log.Warning("[AdMobRewardedInterstitialAd] Load failed! Rewarded interstitial ad is loading!");
                    return false;
                }

                if (string.IsNullOrEmpty(Identifier)) {
                    Log.Warning("[AdMobRewardedInterstitialAd] Load failed! Rewarded interstitial ad id is null or empty!");
                    return false;
                }

                IsLoading = true;

                InvokeOnLoadEvent(AdEventArgs.Create(AdEventArgs.adLoad)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                AdRequest request = new AdRequest();
                GoogleMobileAds.Api.RewardedInterstitialAd.Load(Identifier, request, OnAdLoadEvent);
                return true;
            }

            public override bool Show(Action onCompleted, Action onFailed, string placement) {
                if (!IsReady) {
                    Log.Warning("[AdMobRewardedInterstitialAd] Show failed! Rewarded interstitial ad not avaliable!");
                    return false;
                }

                if (IsShowing) {
                    Log.Warning("[AdMobRewardedInterstitialAd] Show failed! Rewarded interstitial ad is showing!");
                    return false;
                }

                IsShowing = true;
                receivedReward = false;
                this.onCompleted = onCompleted;
                this.placement = placement;

                ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
                ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
                ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                ad.OnAdClicked += OnAdClicked;
                ad.OnAdPaid += OnAdPaid;
                ad.OnAdImpressionRecorded += OnAdImpressionRecorded;

                InvokeOnDisplayEvent(AdEventArgs.Create(AdEventArgs.adDisplay)
                      .Add(AdEventArgs.id, Identifier)
                      .Add(AdEventArgs.placement, placement));

                ad.Show(OnUserRewardEarned);
                return true;
            }

            private void OnAdLoadEvent(GoogleMobileAds.Api.RewardedInterstitialAd ad, LoadAdError error) {
                IsLoading = false;

                if (error == null) {
                    this.ad = ad;
                    InvokeOnLoadedEvent(AdEventArgs.Create(AdEventArgs.adLoaded)
                          .Add(AdEventArgs.id, Identifier)
                          .Add(AdEventArgs.placement, placement));

                    retry = 0;
                }
                else {
                    InvokeOnLoadFailedEvent(AdEventArgs.Create(AdEventArgs.adLoadFailed)
                          .Add(AdEventArgs.id, Identifier)
                          .Add(AdEventArgs.placement, placement)
                          .Add(AdEventArgs.error, error.ToString()));

                    retry++;
                    float delay = 2 * Math.Min(6, retry);
                    Invoke(Reload, delay);
                }
            }

            private void OnAdFullScreenContentOpened() {
                InvokeOnDisplayedEvent(AdEventArgs.Create(AdEventArgs.adDisplayed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdFullScreenContentFailed(AdError error) {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                receivedReward = false;
                IsShowing = false;
                Invoke(onCompleted);
                InvokeOnDisplayFailedEvent(AdEventArgs.Create(AdEventArgs.adDisplayFailed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement)
                    .Add(AdEventArgs.error, error.ToString()));

                Reload(true);
            }

            private void OnAdClicked() {
                InvokeOnClickedEvent(AdEventArgs.Create(AdEventArgs.adClicked)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));
            }

            private void OnAdPaid(AdValue value) {
                AdRevenuePaid adRevenuePaid = new AdRevenuePaid() {
                    adPlatform = client.Network.ToString(),
                    adSource = client.Network.ToString(),
                    adUnitName = Identifier,
                    adFormat = Unit.ToString(),
                    value = value.Value,
                    currency = value.CurrencyCode,
                };

                InvokeOnRevenuePaidEvent(adRevenuePaid);
            }

            private void OnAdImpressionRecorded() {
            }

            private void OnUserRewardEarned(GoogleMobileAds.Api.Reward reward) {
                receivedReward = true;
                CheckReceiveRewardProcess();
            }

            private void OnAdFullScreenContentClosed() {
                if (ad != null) {
                    ad.Destroy();
                    ad = null;
                }

                IsShowing = false;
                CheckReceiveRewardProcess();

                InvokeOnClosedEvent(AdEventArgs.Create(AdEventArgs.adClosed)
                    .Add(AdEventArgs.id, Identifier)
                    .Add(AdEventArgs.placement, placement));

                Reload(true);
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

    }
#endif


    [CategoryMenu("AdMob Advertisings")]
    [System.Serializable]
    public class AdMobServiceProvider : AdServiceProvider {
        public override AdService GetService() {
#if ADMOB
            return new AdMobAdvertisings();
#else
            return null;
#endif
        }
    }
}

