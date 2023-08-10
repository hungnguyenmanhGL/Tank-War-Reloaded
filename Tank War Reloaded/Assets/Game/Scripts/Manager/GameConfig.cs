using System;

public static class GameConfig {
    public static int LevelShowRate {
        get {
            return GameData.PlayerRemoteConfig.GetInt(PlayerRemoteConfig.levelShowRateKey, 1);
        }
    }

    public static bool AppOpenAdEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.appOpenAdEnableKey, true);
        }
    }

    public static bool InterstitialAdEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.interstitialAdEnableKey, true);
        }
    }

    public static bool InterstitialAdUIEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.interstitialAdEnableKey, true);
        }
    }

    public static bool InterstitialAdStartLevelEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.interstitialAdStartLevelEnableKey, true);
        }
    }

    public static bool InterstitialAdFinishLevelEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.interstitialAdFinishLevelEnableKey, true);
        }
    }

    public static int InterstitialAdCooldownTime {
        get {
            return GameData.PlayerRemoteConfig.GetInt(PlayerRemoteConfig.interstitialAdCooldownTimeKey, 30);
        }
    }

    public static bool RewardedAdEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.rewardedAdEnableKey, true);
        }
    }

    public static bool BannerAdEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.bannerAdEnableKey, true);
        }
    }

    public static bool MRectAdEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.mrectAdEnableKey, true);
        }
    }

    public static bool NativeAdEnable {
        get {
            return GameData.PlayerRemoteConfig.GetBool(PlayerRemoteConfig.nativeAdEnableKey, true);
        }
    }

    public static float AppOpenAdCooldownTime {
        get {
            return GameData.PlayerRemoteConfig.GetInt(PlayerRemoteConfig.appOpenAdCooldownKey, 30);
        }
    }
}
