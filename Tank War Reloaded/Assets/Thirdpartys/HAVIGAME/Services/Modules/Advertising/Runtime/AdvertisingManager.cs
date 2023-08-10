using System;
using UnityEngine;

#if PRIVACY
using HAVIGAME.Services.Privacy;
#endif

namespace HAVIGAME.Services.Advertisings {

    public static class AdvertisingManager {

        public const string DEFINE_SYMBOL = "ADVERTISING";

        public static readonly InitializeEvent initializeEvent = new InitializeEvent();

        private static AdService[] adServices = null;

        public static bool IsInitialized => initializeEvent.IsInitialized;

        public static void Initialize() {
#if ADVERTISING
            if (initializeEvent.IsRunning) {
                Log.Warning("[Advertising] Cancel initialize! Advertising initialized with result {0}.", initializeEvent.IsInitialized);
                return;
            }

#if PRIVACY
            PrivacyManager.initializeEvent.AddListener(OnPrivacyInitialized);
#else
            InitializeAdvertisings();
#endif

#endif
        }

#if PRIVACY
        private static void OnPrivacyInitialized(bool isInitialized) {
            if (isInitialized) {
                PrivacyManager.RequestAuthorization(OnRequestAuthorizationCallback);
            }
            else {
                InitializeAdvertisings();
            }
        }

        private static void OnRequestAuthorizationCallback(AuthorizationStatus authorizationStatus) {
            InitializeAdvertisings();
        }
#endif

        private static void InitializeAdvertisings() {
            AdvertisingSettings settings = AdvertisingSettings.Instance;

            AdServiceProvider[] providers = settings.GetServiceProviders();

            if (providers.Length <= 0) {
                Log.Error("[Advertising] Initialize failed! Advertising services is empty.");
                initializeEvent.Invoke(false);
                return;
            }

            adServices = new AdService[providers.Length];

            for (int i = 0; i < providers.Length; i++) {
                adServices[i] = providers[i].GetService();
                RegisterClientEvents(adServices[i]);
                adServices[i].Initialize();
            }

            Database.Unload(settings);

            Log.Info("[Advertising] Initialize completed.");
            initializeEvent.Invoke(true);
        }


        public static AdService GetAdClient(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize.");
                return null;
            }

            foreach (var item in adServices) {
                if (item.Network == network) return item;
            }
            return null;
        }

        #region Events
        public static event AdClientInitializeDelegate onAdInitialized;
        public static event AdClientRevenuePaidDelegate onAdRevenuePaid;
        public static event AdClientDelegate onAdLoad;
        public static event AdClientDelegate onAdLoaded;
        public static event AdClientDelegate onAdLoadFailed;
        public static event AdClientDelegate onAdDisplay;
        public static event AdClientDelegate onAdDisplayed;
        public static event AdClientDelegate onAdDisplayFailed;
        public static event AdClientDelegate onAdClicked;
        public static event AdClientDelegate onAdClosed;

        private static void RegisterClientEvents(AdService client) {
            client.initializeEvent.AddListener(isInitialized => InvokeOnAdInitializedEvent(client, isInitialized));
            client.onAdRevenuePaid += InvokeOnAdRevenuePaidEvent;
            client.onAdLoad += InvokeOnAdLoadEvent;
            client.onAdLoaded += InvokeOnAdLoadedEvent;
            client.onAdLoadFailed += InvokeOnAdLoadFailedEvent;
            client.onAdDisplay += InvokeOnAdDisplayEvent;
            client.onAdDisplayed += InvokeOnAdDisplayedEvent;
            client.onAdDisplayFailed += InvokeOnAdDisplayFailedEvent;
            client.onAdClicked += InvokeOnAdClickedEvent;
            client.onAdClosed += InvokeOnAdClosedEvent;
        }
        private static void UnregisterClientEvents(AdService client) {
            client.onAdRevenuePaid -= InvokeOnAdRevenuePaidEvent;
            client.onAdLoad -= InvokeOnAdLoadEvent;
            client.onAdLoaded -= InvokeOnAdLoadedEvent;
            client.onAdLoadFailed -= InvokeOnAdLoadFailedEvent;
            client.onAdDisplay -= InvokeOnAdDisplayEvent;
            client.onAdDisplayed -= InvokeOnAdDisplayedEvent;
            client.onAdDisplayFailed -= InvokeOnAdDisplayFailedEvent;
            client.onAdClicked -= InvokeOnAdClickedEvent;
            client.onAdClosed -= InvokeOnAdClosedEvent;
        }

        private static void InvokeOnAdInitializedEvent(AdService client, bool isInitialized) {
            Invoke(() => onAdInitialized?.Invoke(client, isInitialized));
        }
        private static void InvokeOnAdRevenuePaidEvent(AdService client, AdRevenuePaid adImpression) {
            Invoke(() => onAdRevenuePaid?.Invoke(client, adImpression));
        }
        private static void InvokeOnAdLoadEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdLoad?.Invoke(client, unit, args));
        }
        private static void InvokeOnAdLoadedEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdLoaded?.Invoke(client, unit, args));
        }
        private static void InvokeOnAdLoadFailedEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdLoadFailed?.Invoke(client, unit, args));
        }
        private static void InvokeOnAdDisplayEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdDisplay?.Invoke(client, unit, args));
        }
        private static void InvokeOnAdDisplayedEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdDisplayed?.Invoke(client, unit, args));
        }
        private static void InvokeOnAdDisplayFailedEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdDisplayFailed?.Invoke(client, unit, args));
        }
        private static void InvokeOnAdClickedEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdClicked?.Invoke(client, unit, args));
        }
        private static void InvokeOnAdClosedEvent(AdService client, AdUnit unit, AdEventArgs args) {
            Invoke(() => onAdClosed?.Invoke(client, unit, args));
        }
        private static void Invoke(Action action) {
            Executor.Instance.RunOnMainTheard(action);
        }
        #endregion

        #region App Open Ad
        public static bool IsAppOpenAdReady() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsAppOpenAdReady) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsAppOpenAdReady(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsAppOpenAdReady;
            }
            else {
                return false;
            }
        }

        public static bool IsAppOpenAdShowing() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsAppOpenAdShowing) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsAppOpenAdShowing(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsAppOpenAdShowing;
            }
            else {
                return false;
            }
        }

        public static bool ShowAppOpenAd(Action onCompleted, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsAppOpenAdReady) {
                    return adService.ShowAppOpenAd(onCompleted, placement);
                }
            }

            return false;
        }

        public static bool ShowAppOpenAd(AdNetwork network, Action onCompleted, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.ShowAppOpenAd(onCompleted, placement);
            }
            else {
                return false;
            }
        }
        #endregion

        #region Interstitial Ad
        public static bool IsInterstitialAdReady() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsInterstitialAdReady) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInterstitialAdReady(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsInterstitialAdReady;
            }
            else {
                return false;
            }
        }

        public static bool IsInterstitialAdShowing() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsInterstitialAdShowing) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInterstitialAdShowing(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsInterstitialAdShowing;
            }
            else {
                return false;
            }
        }

        public static bool ShowInterstitialAd(Action onCompleted, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsInterstitialAdReady) {
                    return adService.ShowInterstitialAd(onCompleted, placement);
                }
            }

            return false;
        }

        public static bool ShowInterstitialAd(AdNetwork network, Action onCompleted, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.ShowInterstitialAd(onCompleted, placement);
            }
            else {
                return false;
            }
        }
        #endregion

        #region Rewarded Ad
        public static bool IsRewardedAdReady() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsRewardedAdReady) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsRewardedAdReady(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsRewardedAdReady;
            }
            else {
                return false;
            }
        }

        public static bool IsRewardedAdShowing() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsRewardedAdShowing) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsRewardedAdShowing(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsRewardedAdShowing;
            }
            else {
                return false;
            }
        }

        public static bool ShowRewardedAd(Action onCompleted, Action onFailed, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsRewardedAdReady) {
                    return adService.ShowRewardedAd(onCompleted, onFailed, placement);
                }
            }

            return false;
        }

        public static bool ShowRewardedAd(AdNetwork network, Action onCompleted, Action onFailed, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.ShowRewardedAd(onCompleted, onFailed, placement);
            }
            else {
                return false;
            }
        }
        #endregion

        #region Banner Ad
        public static bool IsBannerAdReady() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsBannerAdReady) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsBannerAdReady(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsBannerAdReady;
            }
            else {
                return false;
            }
        }

        public static bool IsBannerAdShowing() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsBannerAdShowing) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsBannerAdShowing(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsBannerAdShowing;
            }
            else {
                return false;
            }
        }

        public static BannerAdPosition BannerAdPosition() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return HAVIGAME.Services.Advertisings.BannerAdPosition.Null;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsBannerAdShowing) {
                    return adService.BannerAdPosition;
                }
            }

            return HAVIGAME.Services.Advertisings.BannerAdPosition.Null;
        }

        public static BannerAdPosition BannerAdPosition(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return HAVIGAME.Services.Advertisings.BannerAdPosition.Null;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.BannerAdPosition;
            }
            else {
                return HAVIGAME.Services.Advertisings.BannerAdPosition.Null;
            }
        }

        public static float BannerHeight() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return 0;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsBannerAdShowing) {
                    return adService.BannerHeight;
                }
            }

            return 0;
        }

        public static float BannerHeight(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return 0;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.BannerHeight;
            }
            else {
                return 0;
            }
        }

        public static bool ShowBannerAd(BannerAdPosition position, string placement, Vector2Int offset) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsBannerAdReady) {
                    return adService.ShowBannerAd(position, placement, offset);
                }
            }

            return false;
        }

        public static bool ShowBannerAd(AdNetwork network, BannerAdPosition position, string placement, Vector2Int offset) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.ShowBannerAd(position, placement, offset);
            }
            else {
                return false;
            }
        }

        public static bool HideBannerAd() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsBannerAdShowing) {
                    return adService.HideBannerAd();
                }
            }

            return false;
        }

        public static bool HideBannerAd(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.HideBannerAd();
            }
            else {
                return false;
            }
        }

        #endregion

        #region Native Ad
        public static bool IsNativeAdReady() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsNativeAdReady) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNativeAdReady(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsNativeAdReady;
            }
            else {
                return false;
            }
        }

        public static bool IsNativeAdShowing() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsNativeAdShowing) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNativeAdShowing(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adClient = GetAdClient(network);

            if (adClient != null) {
                return adClient.IsNativeAdShowing;
            }
            else {
                return false;
            }
        }

        public static bool ShowNativeAd(NativeAdView view, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsNativeAdReady) {
                    return adService.ShowNativeAd(view, placement);
                }
            }

            return false;
        }
        #endregion

        #region Medium Rectangle Ad
        public static bool IsMediumRectangleAdReady() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsMediumRectangleAdReady) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMediumRectangleAdReady(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.IsMediumRectangleAdReady;
            }
            else {
                return false;
            }
        }

        public static bool IsMediumRectangleAdShowing() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsMediumRectangleAdShowing) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMediumRectangleAdShowing(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.IsMediumRectangleAdShowing;
            }
            else {
                return false;
            }
        }

        public static MediumRectangleAdPosition MediumRectangleAdPosition() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return HAVIGAME.Services.Advertisings.MediumRectangleAdPosition.Null;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsMediumRectangleAdShowing) {
                    return adService.MediumRectangleAdPosition;
                }
            }

            return HAVIGAME.Services.Advertisings.MediumRectangleAdPosition.Null;
        }

        public static MediumRectangleAdPosition MediumRectangleAdPosition(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return HAVIGAME.Services.Advertisings.MediumRectangleAdPosition.Null;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.MediumRectangleAdPosition;
            }
            else {
                return HAVIGAME.Services.Advertisings.MediumRectangleAdPosition.Null;
            }
        }

        public static float MediumRectangleHeight() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return 0;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsMediumRectangleAdShowing) {
                    return adService.MediumRectangleHeight;
                }
            }

            return 0;
        }

        public static float MediumRectanglerHeight(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return 0;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.MediumRectangleHeight;
            }
            else {
                return 0;
            }
        }

        public static bool ShowMediumRectangleAd(MediumRectangleAdPosition position, string placement, Vector2Int offset) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsMediumRectangleAdReady) {
                    return adService.ShowMediumRectangleAd(position, placement, offset);
                }
            }

            return false;
        }

        public static bool ShowMediumRectangleAd(AdNetwork network, MediumRectangleAdPosition position, string placement, Vector2Int offset) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.ShowMediumRectangleAd(position, placement, offset);
            }
            else {
                return false;
            }
        }

        public static bool HideMediumRectangleAd() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsMediumRectangleAdShowing) {
                    return adService.HideMediumRectangleAd();
                }
            }

            return false;
        }

        public static bool HideMediumRectangleAd(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.HideMediumRectangleAd();
            }
            else {
                return false;
            }
        }

        #endregion

        #region Rewarded Interstitial Ad
        public static bool IsRewardedInterstitialAdReady() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsRewardedInterstitialAdReady) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsRewardedInterstitialAdReady(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.IsRewardedInterstitialAdReady;
            }
            else {
                return false;
            }
        }

        public static bool IsRewardedInterstitialAdShowing() {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsRewardedInterstitialAdShowing) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsRewardedInterstitialAdShowing(AdNetwork network) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.IsRewardedInterstitialAdShowing;
            }
            else {
                return false;
            }
        }

        public static bool ShowRewardedInterstitialAd(Action onCompleted, Action onFailed, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            foreach (AdService adService in adServices) {
                if (adService.IsRewardedInterstitialAdReady) {
                    return adService.ShowRewardedInterstitialAd(onCompleted, onFailed, placement);
                }
            }

            return false;
        }

        public static bool ShowRewardedInterstitialAd(AdNetwork network, Action onCompleted, Action onFailed, string placement) {
            if (!IsInitialized) {
                Log.Warning("[Advertising] Advertising no initialize!");
                return false;
            }

            AdService adService = GetAdClient(network);

            if (adService != null) {
                return adService.ShowRewardedInterstitialAd(onCompleted, onFailed, placement);
            }
            else {
                return false;
            }
        }
        #endregion


#if ADVERTISING
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModule() {
            GameManager.RegisterModule<Initializer>();
        }

        public class Initializer : ModuleInitializer {

            public override int Order => SERVICE;
            public override InitializeEvent InitializeEvent => AdvertisingManager.initializeEvent;
            public override void Initialize() {
                AdvertisingManager.Initialize();
            }
        }
#endif
    }
}
