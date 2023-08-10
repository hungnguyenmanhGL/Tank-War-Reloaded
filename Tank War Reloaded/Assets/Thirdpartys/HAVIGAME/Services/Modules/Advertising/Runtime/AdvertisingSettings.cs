using UnityEngine;

namespace HAVIGAME.Services.Advertisings {

    [DefineSymbols(AdvertisingManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(AdvertisingSettings), "Services/Advertisings", "", null, 101, "Icons/icon_ad")]
    [CreateAssetMenu(fileName = "AdvertisingSettings", menuName = "HAVIGAME/Settings/Services/Advertisings")]
    public class AdvertisingSettings : Database<AdvertisingSettings> {

        [SerializeReference, Subclass] private AdServiceProvider[] serviceProviders;

        public AdServiceProvider[] GetServiceProviders() {
            return serviceProviders;
        }
    }
}
