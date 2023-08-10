using HAVIGAME.Services.RemoteConfig;

public static class GameRemoteConfig {
    public static string GetStringValue(string key, string defaultValue = "") {
        return RemoteConfigManager.GetStringValue(key, defaultValue);
    }

    public static long GetIntValue(string key, int defaultValue = 0) {
        return RemoteConfigManager.GetIntValue(key, defaultValue);
    }

    public static bool GetBooleanValue(string key, bool defaultValue = false) {
        return RemoteConfigManager.GetBooleanValue(key, defaultValue);
    }
}
