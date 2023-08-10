using UnityEngine;

namespace HAVIGAME.Plugins.IronSources {

    [DefineSymbols(IronSourceManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(IronSourceSettings), "Plugins/IronSource", "", "https://developers.is.com/ironsource-mobile/unity/levelplay-starter-kit/", 201, "Icons/icon_ironsource")]
    [CreateAssetMenu(fileName = "IronSourceSettings", menuName = "HAVIGAME/Settings/Plugins/IronSource")]
    public class IronSourceSettings : Database<IronSourceSettings> {
        [SerializeField] private string appId;
        [SerializeField] private BoolPropertyReadonly useRewardedAd = BoolPropertyReadonly.Create();
        [SerializeField] private BoolPropertyReadonly useInterstitialAd = BoolPropertyReadonly.Create();
        [SerializeField] private BoolPropertyReadonly useBannerAd = BoolPropertyReadonly.Create();
        [SerializeField] private bool pauseOnBackground;
        [SerializeField] private bool manualLoadRewardedAd;

        public string AppID => appId;
        public bool UseRewardedAd => useRewardedAd.Get();
        public bool UseInterstitialAd => useInterstitialAd.Get();
        public bool UseBannerAd => useBannerAd.Get();
        public bool PauseOnBackground => pauseOnBackground;
        public bool ManualLoadRewardedAd  => manualLoadRewardedAd;
    }
}
