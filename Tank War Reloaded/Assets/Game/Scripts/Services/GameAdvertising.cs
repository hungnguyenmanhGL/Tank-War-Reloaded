using System;
using HAVIGAME;
using HAVIGAME.Services.Advertisings;
using UnityEngine;
using HAVIGAME.UI;
using System.Collections.Generic;

public class GameAdvertising : Singleton<GameAdvertising> {
    public static bool IsInitialized() => AdvertisingManager.IsInitialized;


    public static bool IsAppOpenAdEnable => GameConfig.AppOpenAdEnable;
    public static bool IsIntertitialAdEnable => GameConfig.InterstitialAdEnable;
    public static bool IsRewaredAdEnable => GameConfig.RewardedAdEnable;
    public static bool IsBannerAdEnable => GameConfig.BannerAdEnable;
    public static bool IsMRectAdEnable => GameConfig.MRectAdEnable;
    public static bool IsNativeAdEnable => GameConfig.NativeAdEnable;


    public static bool IsAppOpenAdReady => AdvertisingManager.IsAppOpenAdReady();
    public static bool IsInterstitialAdReady => AdvertisingManager.IsInterstitialAdReady();
    public static bool IsRewardedAdReady => AdvertisingManager.IsRewardedAdReady();
    public static bool IsBannerAdReady => AdvertisingManager.IsBannerAdReady();
    public static bool IsMRectAdReady => AdvertisingManager.IsMediumRectangleAdReady();
    public static bool IsNativeAdReady => AdvertisingManager.IsNativeAdReady();


    public static bool IsShowingAppOpenAd => AdvertisingManager.IsAppOpenAdShowing();
    public static bool IsShowingInterstitialAd => AdvertisingManager.IsInterstitialAdShowing();
    public static bool IsShowingRewardedAd => AdvertisingManager.IsRewardedAdShowing();
    public static bool IsShowingBannerAd => AdvertisingManager.IsBannerAdShowing();
    public static bool IsShowingMRectAd => AdvertisingManager.IsMediumRectangleAdShowing();
    public static bool IsShowingNativeAd => AdvertisingManager.IsNativeAdShowing();


    private static float lastTimeInterstitialAdShowed = float.MinValue;

    private static float lastTimeAppOpenAdShowed = float.MinValue;

    private static Dictionary<string, NativeAdView> nativeAdViews = new Dictionary<string, NativeAdView>();

    public static bool IsAppOpenAdCooldown => Time.time < lastTimeAppOpenAdShowed + GameConfig.AppOpenAdCooldownTime;
    public static bool IsInterstitialAdCooldown => Time.time < lastTimeInterstitialAdShowed + GameConfig.InterstitialAdCooldownTime;

    public static BannerPosition GetBannerAdPosition() {
        return ToBannerPosition(AdvertisingManager.BannerAdPosition());
    }

    public static float GetBannerHeight() {
        if (IsShowingBannerAd) {
#if UNITY_EDITOR
            switch (GetBannerAdPosition()) {
                case BannerPosition.TopCenter:
                    return 100;
                case BannerPosition.TopLeft:
                    return 100;
                case BannerPosition.TopRight:
                    return 100;
                case BannerPosition.BottomCenter:
                    return 168;
                case BannerPosition.BottomLeft:
                    return 168;
                case BannerPosition.BottomRight:
                    return 168;
                default:
                    return 0;
            }
#else
            return AdvertisingManager.BannerHeight();
#endif
        }

        return 0;
    }

    public static MRectPosition GetMRectAdPosition() {
        return ToMRectPosition(AdvertisingManager.MediumRectangleAdPosition());
    }

    public static float GetMRectHeight() {
        if (IsShowingMRectAd) {
#if UNITY_EDITOR
            switch (GetMRectAdPosition()) {
                case MRectPosition.TopCenter:
                    return 250;
                case MRectPosition.TopLeft:
                    return 250;
                case MRectPosition.TopRight:
                    return 250;
                case MRectPosition.BottomCenter:
                    return 250;
                case MRectPosition.BottomLeft:
                    return 250;
                case MRectPosition.BottomRight:
                    return 250;
                default:
                    return 0;
            }
#else
            return AdvertisingManager.MediumRectangleHeight();
#endif
        }

        return 0;
    }

    protected override void OnAwake() {
        RegisterAdEvents();
    }

    protected override void OnDestroy() {
        base.OnDestroy();

        UnregisterAdEvents();
    }

#if !ADMOB
    private void OnApplicationPause(bool pause) {
        if (!pause) {
            if (IsShowingInterstitialAd) {
                Log.Debug("[GameAds] Cancel show app open ad! Interstitial ad is showing.");
                return;
            }

            if (IsShowingRewardedAd) {
                Log.Debug("[GameAds] Cancel show app open ad! Rewarded ad is showing");
                return;
            }
            TryShowAppOpenAd();
        }
    }
#endif

#region Ad Events
    private void UnregisterAdEvents() {
        AdvertisingManager.onAdRevenuePaid -= OnAdRevenuePaid;

        AdvertisingManager.onAdLoad -= OnAdLoad;
        AdvertisingManager.onAdLoaded -= OnAdLoaded;
        AdvertisingManager.onAdLoadFailed -= OnAdLoadFailed;
        AdvertisingManager.onAdDisplay -= OnAdDisplay;
        AdvertisingManager.onAdDisplayed -= OnAdDisplayed;
        AdvertisingManager.onAdDisplayFailed -= OnAdDisplayFailed;
        AdvertisingManager.onAdClicked -= OnAdClicked;
        AdvertisingManager.onAdClosed -= OnAdClosed;
    }

    private void RegisterAdEvents() {
        AdvertisingManager.onAdRevenuePaid += OnAdRevenuePaid;

        AdvertisingManager.onAdLoad += OnAdLoad;
        AdvertisingManager.onAdLoaded += OnAdLoaded;
        AdvertisingManager.onAdLoadFailed += OnAdLoadFailed;
        AdvertisingManager.onAdDisplay += OnAdDisplay;
        AdvertisingManager.onAdDisplayed += OnAdDisplayed;
        AdvertisingManager.onAdDisplayFailed += OnAdDisplayFailed;
        AdvertisingManager.onAdClicked += OnAdClicked;
        AdvertisingManager.onAdClosed += OnAdClosed;

#if ADMOB
        GoogleMobileAds.Api.AppStateEventNotifier.AppStateChanged += AppStateEventNotifier_AppStateChanged;
#endif
    }

#if ADMOB
    private void AppStateEventNotifier_AppStateChanged(GoogleMobileAds.Common.AppState obj) {
        switch (obj) {
            case GoogleMobileAds.Common.AppState.Background:
                break;
            case GoogleMobileAds.Common.AppState.Foreground:
                if (IsShowingInterstitialAd) {
                    Log.Debug("[GameAds] Cancel show app open ad! Interstitial ad is showing.");
                    return;
                }

                if (IsShowingRewardedAd) {
                    Log.Debug("[GameAds] Cancel show app open ad! Rewarded ad is showing");
                    return;
                }

                TryShowAppOpenAd();
                break;
            default:
                break;
        }
    }
#endif

    private void OnAdRevenuePaid(AdService client, AdRevenuePaid adRevenuePaid) {
        if (Log.DebugEnabled) {
            Log.Debug($"[GameAdvertising][{client.Network}] ad revenue paid: {adRevenuePaid}");
        }

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("ad_impression")
            .Add("ad_platform", adRevenuePaid.adPlatform)
            .Add("ad_source", adRevenuePaid.adSource)
            .Add("ad_unit_name", adRevenuePaid.adUnitName)
            .Add("ad_format", adRevenuePaid.adFormat)
            .Add("value", adRevenuePaid.value)
            .Add("currency", "USD"));
    }

    private void OnAdLoad(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdLoaded(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdLoadFailed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Warning);
    }
    private void OnAdDisplay(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdDisplayed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdDisplayFailed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Warning);
    }
    private void OnAdClicked(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }
    private void OnAdClosed(AdService client, AdUnit unit, AdEventArgs args) {
        LogAdEvent(client, unit, args, LogLevel.Debug);
    }

    private void LogAdEvent(AdService client, AdUnit unit, AdEventArgs args, LogLevel logLevel) {
        if (Log.DebugEnabled) {
            Log.Debug($"[GameAdvertising][{client.Network}-{unit}] {args.Name}, id = {args.Get(AdEventArgs.id)}, placement = {args.Get(AdEventArgs.placement)}, info = {args.Get(AdEventArgs.info)}, error = {args.Get(AdEventArgs.error)}", logLevel);
        }

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create(args.Name)
            .Add("network", client.Network)
            .Add("unit", unit)
            .Add("id", args.Get(AdEventArgs.id))
            .Add("placement", args.Get(AdEventArgs.placement))
            .Add("info", args.Get(AdEventArgs.info))
            .Add("error", args.Get(AdEventArgs.error)));
    }
#endregion

    public static bool TryShowAppOpenAd(Action onCompleted = null, bool ignoreCooldownTime = false) {
        if (GameData.PlayerShop.IsPremium) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("app_open_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        onCompleted?.Invoke();
        return true;
#endif

        if (!IsAppOpenAdEnable) {
            Log.Warning("[GameAds] App open ad is disabled");
            return false;
        }

        if (IsShowingAppOpenAd) {
            Log.Warning("[GameAds] App open ad is showing");
            return false;
        }

        if (!ignoreCooldownTime && IsAppOpenAdCooldown) {
            Log.Warning("[GameAds] App open ad is cooldown");
            return false;
        }

        if (AdvertisingManager.ShowAppOpenAd(onCompleted, GetCurrentPlacement())) {
            lastTimeAppOpenAdShowed = Time.time;
            return true;
        }
        else {
            Log.Warning("[GameAds] App open ad is not ready.");
            return false;
        }
    }

    public static bool TryShowInterstitialAd(Action onCompleted = null, bool ignoreCooldownTime = false) {
        if (GameData.PlayerShop.IsPremium) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("interstitial_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        onCompleted?.Invoke();
        return true;
#endif


        if (!IsIntertitialAdEnable) {
            Log.Warning("[GameAds] Interstitial ad is disabled");
            return false;
        }

        if (IsShowingInterstitialAd) {
            Log.Warning("[GameAds] Interstitial ad is showing");
            return false;
        }

        if (!ignoreCooldownTime && IsInterstitialAdCooldown) {
            Log.Warning("[GameAds] Interstitial ad is showing");
            return false;
        }


        if (AdvertisingManager.ShowInterstitialAd(onCompleted, GetCurrentPlacement())) {
            lastTimeInterstitialAdShowed = Time.time;
            return true;
        }
        else {
            Log.Warning("[GameAds] Intertitial ad is not ready.");
            return false;
        }
    }

    public static bool TryShowRewardedAd(Action onCompleted = null, Action onSkipped = null, bool showWarningPopup = true) {
        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("rewarded_ad_request"));


#if AD_DISABLE
        return false;
#endif

#if CHEAT
        onCompleted?.Invoke();
        return true;
#endif

        if (!IsRewaredAdEnable) {
            Log.Warning("[GameAds] Reward ad is disabled");
            return false;
        }

        if (IsShowingRewardedAd) {
            Log.Warning("[GameAds] Reward Ad is showing");
            return false;
        }


        if (AdvertisingManager.ShowRewardedAd(onCompleted, onSkipped, GetCurrentPlacement())) {
            return true;
        }
        else {
            if (showWarningPopup) ShowPopupAdFailed("Reward ad no ready. Please try again later.");
            Log.Warning("[GameAds] Reward ad is not ready.");
            return false;
        }
    }

    public static bool TryShowBannerAd(BannerPosition position) {
        return TryShowBannerAd(position, Vector2Int.zero);
    }

    public static bool TryShowBannerAd(BannerPosition position, Vector2Int offset) {
        if (GameData.PlayerShop.IsPremium) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("banner_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif


        if (!IsBannerAdEnable) {
            Log.Warning("[GameAds] Banner ad is disabled");
            return false;
        }

        if (IsShowingBannerAd) {
            Log.Warning("[GameAds] Banner Ad is showing");
            return false;
        }

        if (AdvertisingManager.ShowBannerAd(FromBannerPosition(position), GetCurrentPlacement(), offset)) {
            return true;
        }
        else {
            Log.Warning("[GameAds] Banner ad is not ready.");
            return false;
        }
    }

    public static bool TryHideBannerAd() {
        return AdvertisingManager.HideBannerAd();
    }

    public static bool TryShowMediumRectangleAd(MRectPosition position) {
        return TryShowMediumRectangleAd(position, Vector2Int.zero);
    }

    public static bool TryShowMediumRectangleAd(MRectPosition position, Vector2Int offset) {
        if (GameData.PlayerShop.IsPremium) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("mrect_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif


        if (!IsMRectAdEnable) {
            Log.Warning("[GameAds] MRect ad is disabled");
            return false;
        }

        if (IsShowingMRectAd) {
            Log.Warning("[GameAds] MRect Ad is showing");
            return false;
        }

        if( AdvertisingManager.ShowMediumRectangleAd(FromMRectPosition(position), GetCurrentPlacement(), offset)) {
            return true;
        }
        else {
            Log.Warning("[GameAds] MRect ad is not ready.");
            return false;
        }
    }

    public static bool TryHideMediumRectangleAd() {
        return AdvertisingManager.HideMediumRectangleAd();
    }

    public static bool TryShowNativeAd(NativeAdView view) {
        if (GameData.PlayerShop.IsPremium) return false;

        GameAnalytics.LogEvent(GameAnalytics.GameEvent.Create("native_ad_request"));

#if AD_DISABLE
        return false;
#endif

#if CHEAT
        return true;
#endif



        if (!IsNativeAdEnable) {
            Log.Warning("[GameAds] Native ad is disabled");
            return false;
        }

        if (AdvertisingManager.ShowNativeAd(view, GetCurrentPlacement())) {
            nativeAdViews[view.ViewId] = view;
            return true;
        }
        else {
            Log.Warning("[GameAds] Native ad is not ready.");
            return false;
        }
    }


    public static bool TryHideNativeAd() {
        foreach (var item in nativeAdViews) {
            item.Value.Hide();
        }
        return true;
    }

    public static bool TryHideNativeAd(string viewId) {
        if (nativeAdViews.ContainsKey(viewId)) {
            nativeAdViews[viewId].Hide();
            return true;
        }
        else {
            return false;
        }
    }

    public static void ResetInterstitialAdCooldown() {
        lastTimeInterstitialAdShowed = float.MinValue;
    }

    public static void IncreaseInterstitialAdCooldown(float increaseSeconds = 15) {
        lastTimeInterstitialAdShowed += increaseSeconds;
    }

    private static BannerPosition ToBannerPosition(BannerAdPosition position) {
        switch (position) {
            case BannerAdPosition.TopCenter: return BannerPosition.TopCenter;
            case BannerAdPosition.TopLeft: return BannerPosition.TopLeft;
            case BannerAdPosition.TopRight: return BannerPosition.TopRight;
            case BannerAdPosition.Centered: return BannerPosition.Centered;
            case BannerAdPosition.CenterLeft: return BannerPosition.CenterLeft;
            case BannerAdPosition.CenterRight: return BannerPosition.CenterRight;
            case BannerAdPosition.BottomCenter: return BannerPosition.BottomCenter;
            case BannerAdPosition.BottomLeft: return BannerPosition.BottomLeft;
            case BannerAdPosition.BottomRight: return BannerPosition.BottomRight;
            default: return BannerPosition.Centered;
        }
    }

    private static BannerAdPosition FromBannerPosition(BannerPosition position) {
        switch (position) {
            case BannerPosition.TopCenter: return BannerAdPosition.TopCenter;
            case BannerPosition.TopLeft: return BannerAdPosition.TopLeft;
            case BannerPosition.TopRight: return BannerAdPosition.TopRight;
            case BannerPosition.Centered: return BannerAdPosition.Centered;
            case BannerPosition.CenterLeft: return BannerAdPosition.CenterLeft;
            case BannerPosition.CenterRight: return BannerAdPosition.CenterRight;
            case BannerPosition.BottomCenter: return BannerAdPosition.BottomCenter;
            case BannerPosition.BottomLeft: return BannerAdPosition.BottomLeft;
            case BannerPosition.BottomRight: return BannerAdPosition.BottomRight;
            default: return BannerAdPosition.Centered;
        }
    }


    private static MRectPosition ToMRectPosition(MediumRectangleAdPosition position) {
        switch (position) {
            case MediumRectangleAdPosition.TopCenter: return MRectPosition.TopCenter;
            case MediumRectangleAdPosition.TopLeft: return MRectPosition.TopLeft;
            case MediumRectangleAdPosition.TopRight: return MRectPosition.TopRight;
            case MediumRectangleAdPosition.Centered: return MRectPosition.Centered;
            case MediumRectangleAdPosition.CenterLeft: return MRectPosition.CenterLeft;
            case MediumRectangleAdPosition.CenterRight: return MRectPosition.CenterRight;
            case MediumRectangleAdPosition.BottomCenter: return MRectPosition.BottomCenter;
            case MediumRectangleAdPosition.BottomLeft: return MRectPosition.BottomLeft;
            case MediumRectangleAdPosition.BottomRight: return MRectPosition.BottomRight;
            default: return MRectPosition.Centered;
        }
    }

    private static MediumRectangleAdPosition FromMRectPosition(MRectPosition position) {
        switch (position) {
            case MRectPosition.TopCenter: return MediumRectangleAdPosition.TopCenter;
            case MRectPosition.TopLeft: return MediumRectangleAdPosition.TopLeft;
            case MRectPosition.TopRight: return MediumRectangleAdPosition.TopRight;
            case MRectPosition.Centered: return MediumRectangleAdPosition.Centered;
            case MRectPosition.CenterLeft: return MediumRectangleAdPosition.CenterLeft;
            case MRectPosition.CenterRight: return MediumRectangleAdPosition.CenterRight;
            case MRectPosition.BottomCenter: return MediumRectangleAdPosition.BottomCenter;
            case MRectPosition.BottomLeft: return MediumRectangleAdPosition.BottomLeft;
            case MRectPosition.BottomRight: return MediumRectangleAdPosition.BottomRight;
            default: return MediumRectangleAdPosition.Centered;
        }
    }


    public enum BannerPosition {
        TopCenter,
        TopLeft,
        TopRight,
        Centered,
        CenterLeft,
        CenterRight,
        BottomCenter,
        BottomLeft,
        BottomRight,
    }


    public enum MRectPosition {
        TopCenter,
        TopLeft,
        TopRight,
        Centered,
        CenterLeft,
        CenterRight,
        BottomCenter,
        BottomLeft,
        BottomRight
    }

    private static string GetCurrentPlacement() {
        if (UIManager.HasInstance) {
            UIFrame frame = UIManager.Instance.Peek();

            if (frame) {
                return frame.GetType().Name;
            }
        }

        return "NULL";
    }

    private static void ShowPopupAdFailed(string message) {
        if (UIManager.HasInstance) {
            UIManager.Instance.Push<DialogPanel>(dialog => {
                dialog.Dialog("Failed!", message);
            });
        }
    }
}

