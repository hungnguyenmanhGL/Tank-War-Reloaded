using HAVIGAME.Services.IAP;
using System;

public static class GameIAP {
    public static bool IsInitialized => IAPManager.IsInitialized;

    public static bool IsOwned(string productId) {
        return IAPManager.IsOwned(productId);
    }

    public static bool IsSubscribed(string productId) {
        return IAPManager.IsSubscribed(productId);
    }

    public static bool Purchase(string productId, Action onCompleted = null, Action<string> onFailed = null) {
#if CHEAT
        onCompleted?.Invoke();
        return true;
#else
        return IAPManager.Purchase(productId, onCompleted, onFailed);
#endif
    }

    public static void RestorePurchases(Action<string[]> onCompleted = null, Action<string> onFailed = null) {
        IAPManager.RestorePurchases(onCompleted, onFailed);
    }
}
