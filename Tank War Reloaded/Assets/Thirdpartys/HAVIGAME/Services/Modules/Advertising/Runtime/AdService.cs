using System;
using UnityEngine;

namespace HAVIGAME.Services.Advertisings {
    public abstract class AdService {
        public readonly InitializeEvent initializeEvent = new InitializeEvent();

        public InitializeEvent InitializeEvent => initializeEvent;
        public bool IsInitialized => InitializeEvent.IsInitialized;
        public abstract AdNetwork Network { get; }
        public abstract AdUnit AdUnitSupported { get; }
        public abstract void Initialize();

        #region Events
        internal event AdClientRevenuePaidDelegate onAdRevenuePaid;

        internal AdClientDelegate onAdLoad;
        internal AdClientDelegate onAdLoaded;
        internal AdClientDelegate onAdLoadFailed;
        internal AdClientDelegate onAdDisplay;
        internal AdClientDelegate onAdDisplayed;
        internal AdClientDelegate onAdDisplayFailed;
        internal AdClientDelegate onAdClicked;
        internal AdClientDelegate onAdClosed;

        protected void InvokeAdRevenuePaidEvent(AdUnit unit, AdRevenuePaid adImpression) {
            onAdRevenuePaid?.Invoke(this, adImpression);
        }

        protected void RegisterAdEvents(Ad ad) {
            ad.onRevenuePaid += InvokeAdRevenuePaidEvent;
            ad.onLoad += InvokeOnAdLoadEvent;
            ad.onLoaded += InvokeOnAdLoadedEvent;
            ad.onLoadFailed += InvokeOnAdLoadFailedEvent;
            ad.onDisplay += InvokeOnAdDisplayEvent;
            ad.onDisplayed += InvokeOnAdDisplayedEvent;
            ad.onDisplayFailed += InvokeOnAdDisplayFailedEvent;
            ad.onClicked += InvokeOnAdClickedEvent;
            ad.onClosed += InvokeOnAdClosedEvent;
        }
        protected void UnregisterAdEvents(Ad ad) {
            ad.onRevenuePaid -= InvokeAdRevenuePaidEvent;
            ad.onLoad -= InvokeOnAdLoadEvent;
            ad.onLoaded -= InvokeOnAdLoadedEvent;
            ad.onLoadFailed -= InvokeOnAdLoadFailedEvent;
            ad.onDisplay -= InvokeOnAdDisplayEvent;
            ad.onDisplayed -= InvokeOnAdDisplayedEvent;
            ad.onDisplayFailed -= InvokeOnAdDisplayFailedEvent;
            ad.onClicked -= InvokeOnAdClickedEvent;
            ad.onClosed -= InvokeOnAdClosedEvent;
        }

        private void InvokeOnAdLoadEvent(AdUnit unit, AdEventArgs args) {
            onAdLoad?.Invoke(this, unit, args);
        }
        private void InvokeOnAdLoadedEvent(AdUnit unit, AdEventArgs args) {
            onAdLoaded?.Invoke(this, unit, args);
        }
        private void InvokeOnAdLoadFailedEvent(AdUnit unit, AdEventArgs args) {
            onAdLoadFailed?.Invoke(this, unit, args);
        }
        private void InvokeOnAdDisplayEvent(AdUnit unit, AdEventArgs args) {
            onAdDisplay?.Invoke(this, unit, args);
        }
        private void InvokeOnAdDisplayedEvent(AdUnit unit, AdEventArgs args) {
            onAdDisplayed?.Invoke(this, unit, args);
        }
        private void InvokeOnAdDisplayFailedEvent(AdUnit unit, AdEventArgs args) {
            onAdDisplayFailed?.Invoke(this, unit, args);
        }
        private void InvokeOnAdClickedEvent(AdUnit unit, AdEventArgs args) {
            onAdClicked?.Invoke(this, unit, args);
        }
        private void InvokeOnAdClosedEvent(AdUnit unit, AdEventArgs args) {
            onAdClosed?.Invoke(this, unit, args);
        }
        #endregion

        #region App Open Ads
        protected abstract AppOpenAd[] AppOpenAds { get; }
        public bool IsAppOpenAdSupported => AdUnitSupported.HasFlag(AdUnit.AppOpenAd);
        public bool IsAppOpenAdInitialized => AppOpenAds != null;
        public bool IsAppOpenAdReady {
            get {
                if (IsAppOpenAdInitialized) {
                    foreach (AppOpenAd ad in AppOpenAds) {
                        if (ad != null && ad.IsReady) return true;
                    }
                }

                return false;
            }
        }
        public bool IsAppOpenAdShowing {
            get {
                if (IsAppOpenAdInitialized) {
                    foreach (AppOpenAd ad in AppOpenAds) {
                        if (ad != null && ad.IsShowing) return true;
                    }
                }

                return false;
            }
        }

        public bool ShowAppOpenAd(Action onCompleted, string placement) {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsAppOpenAdSupported) {
                Log.Warning("[{0}] App open ad not supported!", Network);
                return false;
            }

            if (!IsAppOpenAdInitialized) {
                Log.Warning("[{0}] App open ad not initialized!", Network);
                return false;
            }

            if (IsAppOpenAdShowing) {
                Log.Warning("[{0}] App open ad is showing!", Network);
                return false;
            }

            foreach (AppOpenAd ad in AppOpenAds) {
                if (ad != null && ad.IsReady) {
                    return ad.Show(onCompleted, placement);
                }
            }

            Log.Warning("[{0}] App open ad not available!", Network);
            return false;
        }

        #endregion

        #region Interstitial Ads
        protected abstract InterstitialAd[] InterstitialAds { get; }
        public bool IsInterstitialAdSupported => AdUnitSupported.HasFlag(AdUnit.InterstitialAd);
        public bool IsInterstitialAdInitialized => InterstitialAds != null;
        public bool IsInterstitialAdReady {
            get {
                if (IsInterstitialAdInitialized) {
                    foreach (InterstitialAd ad in InterstitialAds) {
                        if (ad != null && ad.IsReady) return true;
                    }
                }

                return false;
            }
        }
        public bool IsInterstitialAdShowing {
            get {
                if (IsInterstitialAdInitialized) {
                    foreach (InterstitialAd ad in InterstitialAds) {
                        if (ad != null && ad.IsShowing) return true;
                    }
                }

                return false;
            }
        }
        public bool ShowInterstitialAd(Action onCompleted, string placement) {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsInterstitialAdSupported) {
                Log.Warning("[{0}] Interstitial ad not supported!", Network);
                return false;
            }

            if (!IsInterstitialAdInitialized) {
                Log.Warning("[{0}] Interstitial ad not initialized!", Network);
                return false;
            }

            if (IsInterstitialAdShowing) {
                Log.Warning("[{0}] Interstitial ad is showing!", Network);
                return false;
            }

            foreach (InterstitialAd ad in InterstitialAds) {
                if (ad != null && ad.IsReady) {
                    return ad.Show(onCompleted, placement);
                }
            }

            Log.Warning("[{0}] Interstitial ad not available!", Network);
            return false;
        }
        #endregion

        #region Rewarded Ads
        protected abstract RewardedAd[] RewardedAds { get; }
        public bool IsRewardedAdSupported => AdUnitSupported.HasFlag(AdUnit.RewardedAd);
        public bool IsRewardedAdInitialized => RewardedAds != null;
        public bool IsRewardedAdReady {
            get {
                if (IsRewardedAdInitialized) {
                    foreach (RewardedAd ad in RewardedAds) {
                        if (ad != null && ad.IsReady) return true;
                    }
                }

                return false;
            }
        }
        public bool IsRewardedAdShowing {
            get {
                if (IsRewardedAdInitialized) {
                    foreach (RewardedAd ad in RewardedAds) {
                        if (ad != null && ad.IsShowing) return true;
                    }
                }

                return false;
            }
        }
        public bool ShowRewardedAd(Action onCompleted, Action onFailed, string placement) {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsRewardedAdSupported) {
                Log.Warning("[{0}] Rewarded ad not supported!", Network);
                return false;
            }

            if (!IsRewardedAdInitialized) {
                Log.Warning("[{0}] Rewarded ad not initialized!", Network);
                return false;
            }

            if (IsRewardedAdShowing) {
                Log.Warning("[{0}] Rewarded ad is showing!", Network);
                return false;
            }

            foreach (RewardedAd ad in RewardedAds) {
                if (ad != null && ad.IsReady) {
                    return ad.Show(onCompleted, onFailed, placement);
                }
            }

            Log.Warning("[{0}] Rewarded ad not available!", Network);
            return false;
        }
        #endregion

        #region Banner Ads
        protected abstract BannerAd[] BannerAds { get; }
        public bool IsBannerAdSupported => AdUnitSupported.HasFlag(AdUnit.BannerAd);
        public bool IsBannerAdInitialized => BannerAds != null;
        public bool IsBannerAdReady {
            get {
                if (IsBannerAdInitialized) {
                    foreach (BannerAd ad in BannerAds) {
                        if (ad != null && ad.IsReady) return true;
                    }
                }

                return false;
            }
        }
        public bool IsBannerAdShowing {
            get {
                if (IsBannerAdInitialized) {
                    foreach (BannerAd ad in BannerAds) {
                        if (ad != null && ad.IsShowing) return true;
                    }
                }

                return false;
            }
        }
        public BannerAdPosition BannerAdPosition {
            get {
                if (IsBannerAdInitialized) {
                    foreach (BannerAd ad in BannerAds) {
                        if (ad != null && ad.IsShowing) return ad.Position;
                    }
                }

                return BannerAdPosition.Null;
            }
        }
        public float BannerHeight {
            get {
                if (IsBannerAdInitialized) {
                    foreach (BannerAd ad in BannerAds) {
                        if (ad != null && ad.IsShowing) return ad.Height;
                    }
                }

                return 0;
            }
        }
        public bool ShowBannerAd(BannerAdPosition position, string placement, Vector2Int offset) {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsBannerAdSupported) {
                Log.Warning("[{0}] Banner ad not supported!", Network);
                return false;
            }

            if (!IsBannerAdInitialized) {
                Log.Warning("[{0}] Banner ad not initialized!", Network);
                return false;
            }

            foreach (BannerAd ad in BannerAds) {
                if (ad != null && ad.IsReady) {
                    return ad.Show(position, placement, offset);
                }
            }

            Log.Warning("[{0}] Banner ad not available!", Network);
            return false;
        }
        public bool HideBannerAd() {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsBannerAdSupported) {
                Log.Warning("[{0}] Banner ad not supported!", Network);
                return false;
            }

            foreach (BannerAd ad in BannerAds) {
                if (ad != null && ad.IsShowing) {
                    return ad.Hide();
                }
            }

            Log.Warning("[{0}] Banner ad not showing!", Network);
            return false;
        }
        #endregion

        #region Rewarded Interstitial Ads
        protected abstract RewardedInterstitialAd[] RewardedInterstitialAds { get; }
        public bool IsRewardedInterstitialAdSupported => AdUnitSupported.HasFlag(AdUnit.RewardedInterstitialAd);
        public bool IsRewardedInterstitialAdInitialized => RewardedAds != null;
        public bool IsRewardedInterstitialAdReady {
            get {
                if (IsRewardedInterstitialAdInitialized) {
                    foreach (RewardedInterstitialAd ad in RewardedInterstitialAds) {
                        if (ad != null && ad.IsReady) return true;
                    }
                }

                return false;
            }
        }
        public bool IsRewardedInterstitialAdShowing {
            get {
                if (IsRewardedInterstitialAdInitialized) {
                    foreach (RewardedInterstitialAd ad in RewardedInterstitialAds) {
                        if (ad != null && ad.IsShowing) return true;
                    }
                }

                return false;
            }
        }
        public bool ShowRewardedInterstitialAd(Action onCompleted, Action onFailed, string placement) {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsRewardedInterstitialAdSupported) {
                Log.Warning("[{0}] Rewarded interstitial ad not supported!", Network);
                return false;
            }

            if (!IsRewardedInterstitialAdInitialized) {
                Log.Warning("[{0}] Rewarded interstitial ad not initialized!", Network);
                return false;
            }

            if (IsRewardedInterstitialAdShowing) {
                Log.Warning("[{0}] Rewarded interstitial ad is showing!", Network);
                return false;
            }

            foreach (RewardedInterstitialAd ad in RewardedInterstitialAds) {
                if (ad != null && ad.IsReady) {
                    return ad.Show(onCompleted, onFailed, placement);
                }
            }

            Log.Warning("[{0}] Rewarded interstitial ad not available!", Network);
            return false;
        }
        #endregion

        #region Medium Rectangle Ads
        protected abstract MediumRectangleAd[] MediumRectangleAds { get; }
        public bool IsMediumRectangleAdSupported => AdUnitSupported.HasFlag(AdUnit.MediumRectangle);
        public bool IsMediumRectangleAdInitialized => MediumRectangleAds != null;
        public bool IsMediumRectangleAdReady {
            get {
                if (IsMediumRectangleAdInitialized) {
                    foreach (MediumRectangleAd ad in MediumRectangleAds) {
                        if (ad != null && ad.IsReady) return true;
                    }
                }

                return false;
            }
        }
        public bool IsMediumRectangleAdShowing {
            get {
                if (IsMediumRectangleAdInitialized) {
                    foreach (MediumRectangleAd ad in MediumRectangleAds) {
                        if (ad != null && ad.IsShowing) return true;
                    }
                }

                return false;
            }
        }
        public MediumRectangleAdPosition MediumRectangleAdPosition {
            get {
                if (IsBannerAdInitialized) {
                    foreach (MediumRectangleAd ad in MediumRectangleAds) {
                        if (ad != null && ad.IsShowing) return ad.Position;
                    }
                }

                return MediumRectangleAdPosition.Null;
            }
        }
        public float MediumRectangleHeight {
            get {
                if (IsMediumRectangleAdInitialized) {
                    foreach (MediumRectangleAd ad in MediumRectangleAds) {
                        if (ad != null && ad.IsShowing) return ad.Height;
                    }
                }

                return 0;
            }
        }
        public bool ShowMediumRectangleAd(MediumRectangleAdPosition position, string placement, Vector2Int offset) {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsMediumRectangleAdSupported) {
                Log.Warning("[{0}] Medium rectangle ad not supported!", Network);
                return false;
            }

            if (!IsMediumRectangleAdInitialized) {
                Log.Warning("[{0}] Medium rectangle ad not initialized!", Network);
                return false;
            }

            foreach (MediumRectangleAd ad in MediumRectangleAds) {
                if (ad != null && ad.IsReady) {
                    return ad.Show(position, placement, offset);
                }
            }

            Log.Warning("[{0}] Medium rectangle ad not available!", Network);
            return false;
        }
        public bool HideMediumRectangleAd() {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsMediumRectangleAdSupported) {
                Log.Warning("[{0}] Medium rectangle ad not supported!", Network);
                return false;
            }

            foreach (MediumRectangleAd ad in MediumRectangleAds) {
                if (ad != null && ad.IsShowing) {
                    return ad.Hide();
                }
            }

            Log.Warning("[{0}] Medium rectangle ad not showing!", Network);
            return false;
        }
        #endregion

        #region Native Ads
        protected abstract NativeAd[] NativeAds { get; }
        public bool IsNativeAdSupported => AdUnitSupported.HasFlag(AdUnit.NativeAd);
        public bool IsNativeAdInitialized => NativeAds != null;
        public bool IsNativeAdReady {
            get {
                if (IsNativeAdInitialized) {
                    foreach (NativeAd ad in NativeAds) {
                        if (ad != null && ad.IsReady) return true;
                    }
                }

                return false;
            }
        }
        public bool IsNativeAdShowing {
            get {
                if (IsNativeAdInitialized) {
                    foreach (NativeAd ad in NativeAds) {
                        if (ad != null && ad.IsShowing) return true;
                    }
                }

                return false;
            }
        }
        public bool ShowNativeAd(NativeAdView view, string placement) {
            if (!IsInitialized) {
                Log.Warning("[{0}] Client not initialized!", Network);
                return false;
            }

            if (!IsNativeAdSupported) {
                Log.Warning("[{0}] Native ad not supported!", Network);
                return false;
            }

            if (!IsNativeAdInitialized) {
                Log.Warning("[{0}] Native ad not initialized!", Network);
                return false;
            }

            foreach (NativeAd ad in NativeAds) {
                if (ad != null && ad.IsReady && !ad.IsShowing) {
                    return ad.Show(view, placement);
                }
            }

            Log.Warning("[{0}] Native ad not available!", Network);
            return false;
        }
        #endregion

    }

    [System.Serializable]
    public abstract class AdServiceProvider : ServiceProvider<AdService> {

    }

    public enum AdNetwork {
        AdMob,
        AppLovin,
        IronSource,
    }

    public struct AdRevenuePaid {
        public string adPlatform;
        public string adSource;
        public string adUnitName;
        public string adFormat;
        public string currency;
        public double value;

        public override string ToString() {
            return Utility.Text.Format("{0} {1}",
                   Utility.Text.Format("value={0}, currency={1}, platform={2}", value, currency, adPlatform),
                   Utility.Text.Format("source={0}, unit name={1}, format={2}", adSource, adUnitName, adFormat));
        }
    }
}
