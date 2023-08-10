using UnityEngine;

namespace HAVIGAME.Services.RemoteConfig {

    [DefineSymbols(RemoteConfigManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(RemoteConfigSettings), "Services/Remote Config", "", null, 101, "Icons/icon_remoteconfig")]
    [CreateAssetMenu(fileName = "RemoteConfigSettings", menuName = "HAVIGAME/Settings/Services/Remote Config")]
    public class RemoteConfigSettings : Database<RemoteConfigSettings> {

        [SerializeReference, Subclass] private RemoteConfigServiceProvider serviceProvider;
        [SerializeField] private bool saveConfigValues = true;
        [SerializeField] private string saveId = "remote_config";

        public RemoteConfigServiceProvider ServiceProvider => serviceProvider;
        public bool SaveConfigValues  => saveConfigValues;
        public string SaveId => saveId;
    }
}
