using UnityEngine;

namespace HAVIGAME.Services.Crashlytics {

    [DefineSymbols(CrashlyticManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(CrashlyticsSettings), "Services/Crashlytics", "", null, 101, "Icons/icon_crashlytics")]
    [CreateAssetMenu(fileName = "CrashlyticsSettings", menuName = "HAVIGAME/Settings/Services/Crashlytics")]
    public class CrashlyticsSettings : Database<CrashlyticsSettings> {

        [SerializeReference, Subclass] private CrashlyticServiceProvider[] serviceProviders;

        public CrashlyticServiceProvider[] GetServiceProviders() {
            return serviceProviders;
        }
    }
}
