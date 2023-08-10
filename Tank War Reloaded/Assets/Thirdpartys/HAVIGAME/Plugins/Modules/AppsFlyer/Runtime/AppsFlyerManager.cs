using UnityEngine;

namespace HAVIGAME.Plugins.AppsFlyer {
    public static class AppsFlyerManager {
        public const string DEFINE_SYMBOL = "APPSFLYER";

        public static readonly InitializeEvent initializeEvent = new InitializeEvent();

        public static bool IsInitialized => initializeEvent.IsInitialized;

        public static void Initialize() {
#if APPSFLYER
            if (initializeEvent.IsRunning) {
                Log.Warning("[AppsFlyer] AppsFlyer is running with initialize state {0}.", IsInitialized);
                return;
            }

            AppsFlyerSettings settings = AppsFlyerSettings.Instance;

            AppsFlyerSDK.AppsFlyer.initSDK(settings.DevKey, settings.AppID);

            AppsFlyerSDK.AppsFlyer.startSDK();

            Database.Unload(settings);

            Log.Info("[AppsFlyer] Initialize completed.");
            initializeEvent.Invoke(true);
#endif
        }

#if APPSFLYER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModule() {
            GameManager.RegisterModule<Initializer>();
        }

        public class Initializer : ModuleInitializer {

            public override int Order => PLUGIN;
            public override InitializeEvent InitializeEvent => AppsFlyerManager.initializeEvent;

            public override void Initialize() {
                AppsFlyerManager.Initialize();
            }
        }
#endif
    }
}
