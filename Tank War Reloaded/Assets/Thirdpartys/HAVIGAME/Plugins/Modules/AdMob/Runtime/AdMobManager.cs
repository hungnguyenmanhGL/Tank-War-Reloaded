#if ADMOB
using GoogleMobileAds.Api;
using UnityEngine;
#endif

namespace HAVIGAME.Plugins.AdMob {
    public static class AdMobManager {
        public const string DEFINE_SYMBOL = "ADMOB";

        public static readonly InitializeEvent initializeEvent = new InitializeEvent();

        public static bool IsInitialized => initializeEvent.IsInitialized;


        public static void Initialize() {
#if ADMOB
            if (initializeEvent.IsRunning) {
                Log.Warning("[AdMob] AdMob is running with initialize state {0}.", IsInitialized);
                return;
            }

            AdMobSettings settings = AdMobSettings.Instance;
            RequestConfiguration requestConfiguration = new RequestConfiguration();
            if (settings.TagForChildDirectedTreatment != Result.Unspecified) {

            }

            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize(OnSdkInitializedEvent);
#endif
        }

#if ADMOB
        private static void OnSdkInitializedEvent(InitializationStatus initializationStatus) {
            foreach (var item in initializationStatus.getAdapterStatusMap()) {
                switch (item.Value.InitializationState) {
                    case AdapterState.NotReady:
                        Log.Warning("[AdMob] Adapter {0} initialize failed, description = {1}, latency = {2}", item.Key, item.Value.Description, item.Value.Latency);
                        break;
                    case AdapterState.Ready:
                        Log.Debug("[AdMob] Adapter {0} initialized, description = {1}, latency = {2}", item.Key, item.Value.Description, item.Value.Latency);
                        break;
                    default:
                        break;
                }
            }

            Log.Info("[AdMob] AdMob initialize completed.");
            initializeEvent.Invoke(true);
        }
#endif

#if ADMOB
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModule() {
            GameManager.RegisterModule<Initializer>();
        }

        public class Initializer : ModuleInitializer {

            public override int Order => PLUGIN;

            public override InitializeEvent InitializeEvent => AdMobManager.initializeEvent;

            public override void Initialize() {
                AdMobManager.Initialize();
            }
        }
#endif
    }
}
