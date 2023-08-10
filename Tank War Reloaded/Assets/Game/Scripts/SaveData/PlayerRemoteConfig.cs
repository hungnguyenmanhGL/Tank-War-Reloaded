using HAVIGAME;
using HAVIGAME.SaveLoad;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerRemoteConfig : SaveData {
    public const string appOpenAdEnableKey = "open_ad_enable";
    public const string appOpenAdCooldownKey = "app_open_ad_cooldown_time";

    public const string interstitialAdEnableKey = "interstitial_ad_enable";
    public const string interstitialAdUIEnableKey = "interstitial_ad_ui_enable";
    public const string interstitialAdCooldownTimeKey = "interstitial_ad_cooldown_time";
    public const string interstitialAdStartLevelEnableKey = "interstitial_ad_start_level_enable";
    public const string interstitialAdFinishLevelEnableKey = "interstitial_ad_finish_level_enable";

    public const string rewardedAdEnableKey = "rewarded_ad_enable";

    public const string bannerAdEnableKey = "banner_ad_enable";

    public const string mrectAdEnableKey = "mrect_ad_enable";

    public const string nativeAdEnableKey = "native_ad_enable";

    public const string levelShowRateKey = "level_show_rate";

    [SerializeField] private List<RemoteData> remoteDatas;

    public PlayerRemoteConfig() : base() {
        remoteDatas = new List<RemoteData>();

        remoteDatas.Add(new RemoteData(appOpenAdEnableKey, true));
        remoteDatas.Add(new RemoteData(appOpenAdCooldownKey, 30));

        remoteDatas.Add(new RemoteData(interstitialAdEnableKey, true));
        remoteDatas.Add(new RemoteData(interstitialAdUIEnableKey, 30));
        remoteDatas.Add(new RemoteData(interstitialAdCooldownTimeKey, 30));
        remoteDatas.Add(new RemoteData(interstitialAdStartLevelEnableKey, true));
        remoteDatas.Add(new RemoteData(interstitialAdFinishLevelEnableKey, true));

        remoteDatas.Add(new RemoteData(rewardedAdEnableKey, true));

        remoteDatas.Add(new RemoteData(bannerAdEnableKey, true));

        remoteDatas.Add(new RemoteData(mrectAdEnableKey, true));

        remoteDatas.Add(new RemoteData(nativeAdEnableKey, true));

        remoteDatas.Add(new RemoteData(levelShowRateKey, 1));
    }

    public int GetInt(string key, int defaultValue = 0) {
        RemoteData data = GetData(key);

        if (data != null) {
            return data.GetInt(defaultValue);
        }

        return defaultValue;
    }

    public bool GetBool(string key, bool defaultValue = false) {
        RemoteData data = GetData(key);

        if (data != null) {
            return data.GetBool(defaultValue);
        }

        return defaultValue;
    }

    public string GetString(string key, string defaultValue = "") {
        RemoteData data = GetData(key);

        if (data != null) {
            return data.GetString(defaultValue);
        }

        return defaultValue;
    }

    public RemoteData GetData(string key) {
        foreach (var item in remoteDatas) {
            if (item.Key.Equals(key)) {
                return item;
            }
        }

        return null;
    }

    public IEnumerable<RemoteData> GetDatas() {
        return remoteDatas;
    }

    public void UpdateDatas() {
        foreach (var item in remoteDatas) {
            item.Update();
        }
    }

    public void Dispose() {

    }

    [System.Serializable]
    public class RemoteData {
        [SerializeField] private string key;
        [SerializeField] private string value;

        public string Key => key;
        public string Value => value;

        public RemoteData(string key, object value) {
            this.key = key;
            this.value = value.ToString();
        }

        public void Update() {
            value = GameRemoteConfig.GetStringValue(key, Value);

            Log.Debug($"[PlayerRemoteConfig] Update data, key = {key}, value = {value}");
        }


        public int GetInt(int defaultValue = 0) {
            if (int.TryParse(value, out int result)) {
                return result;
            }

            return defaultValue;
        }

        public bool GetBool(bool defaultValue = false) {
            if (bool.TryParse(value, out bool result)) {
                return result;
            }

            return defaultValue;
        }

        public string GetString(string defaultValue = "") {
            if (!string.IsNullOrEmpty(value)) {
                return value;
            }

            return defaultValue;
        }
    }
}