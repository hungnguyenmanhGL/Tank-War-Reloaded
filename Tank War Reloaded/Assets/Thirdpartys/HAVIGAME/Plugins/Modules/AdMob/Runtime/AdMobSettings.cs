using UnityEngine;
using HAVIGAME.Services.Advertisings;

namespace HAVIGAME.Plugins.AdMob {

    [DefineSymbols(AdMobManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(AdMobSettings), "Plugins/AdMob", "", "https://developers.google.com/admob/unity/quick-start", 201, "Icons/icon_admob")]
    [CreateAssetMenu(fileName = "AdMobSettings", menuName = "HAVIGAME/Settings/Plugins/AdMob")]
    public class AdMobSettings : Database<AdMobSettings> {
        [SerializeReference, Subclass] private AdId[] appOpenAdIds;
        [SerializeReference, Subclass] private AdId[] rewardedAdIds;
        [SerializeReference, Subclass] private AdId[] interstitialAdIds;
        [SerializeReference, Subclass] private AdId[] bannerAdIds;
        [SerializeReference, Subclass] private AdId[] mediumRectangleAdIds;
        [SerializeReference, Subclass] private AdId[] rewardedInterstitialAdIds;
        [SerializeReference, Subclass] private AdId[] nativeAdIds;
        [SerializeField] private ScreenOrientation appOpenAdScreenOrientation;
        [SerializeField] private Result tagForChildDirectedTreatment = Result.Unspecified;
        [SerializeField] private Result tagForUnderAgeOfConsent = Result.Unspecified;

        public AdId[] AppOpenAdIds => appOpenAdIds;
        public AdId[] RewardedAdIds => rewardedAdIds;
        public AdId[] InterstitialAdIds => interstitialAdIds;
        public AdId[] BannerAdIds => bannerAdIds;
        public AdId[] MediumRectangleAdIds => mediumRectangleAdIds;
        public AdId[] RewardedInterstitialAdIds => rewardedInterstitialAdIds;
        public AdId[] NativeAdIds => nativeAdIds;
        public Result TagForChildDirectedTreatment => tagForChildDirectedTreatment;
        public Result TagForUnderAgeOfConsent => tagForUnderAgeOfConsent;
        public ScreenOrientation AppOpenAdScreenOrientation => appOpenAdScreenOrientation;
    }
}
