using UnityEngine;

namespace HAVIGAME.Plugins.AppsFlyer {

    [DefineSymbols(AppsFlyerManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(AppsFlyerSettings), "Plugins/AppsFlyer", "", "https://support.appsflyer.com/hc/vi/articles/360007314277", 201, "Icons/icon_appsflyer")]
    [CreateAssetMenu(fileName = "AppsFlyerSettings", menuName = "HAVIGAME/Settings/Plugins/AppsFlyer")]
    public class AppsFlyerSettings : Database<AppsFlyerSettings> {
        [SerializeField] private string devKey;
        [SerializeField] private string appID;
        [SerializeField] private bool isDebug;
        [SerializeField] private bool getConversionData;

        public string DevKey => devKey;
        public string AppID => appID;
        public bool IsDebug => isDebug;
        public bool GetConversionData => getConversionData;
    }
}
