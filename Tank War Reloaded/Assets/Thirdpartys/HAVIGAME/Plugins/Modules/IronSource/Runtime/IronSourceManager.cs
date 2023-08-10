using UnityEngine;

namespace HAVIGAME.Plugins.IronSources {
    public static class IronSourceManager {
        public const string DEFINE_SYMBOL = "IRON_SOURCE";

        public static readonly InitializeEvent initializeEvent = new InitializeEvent();

        public static bool IsInitialized => initializeEvent.IsInitialized;

        public static void Initialize() {
#if IRON_SOURCE
            if (initializeEvent.IsRunning) {
                Log.Warning("[IronSource] IronSource is running with initialize state {0}.", IsInitialized);
                return;
            }

            IronSourceSettings setting = IronSourceSettings.Instance;

            GameManager.onApplicationPause += OnApplicationPause;
            IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializedEvent;

            IronSource.Agent.SetPauseGame(setting.PauseOnBackground);
            IronSource.Agent.setManualLoadRewardedVideo(setting.ManualLoadRewardedAd);
            IronSource.Agent.init(setting.AppID);

            setting.Dispose();
#endif
        }


#if IRON_SOURCE
        private static void OnApplicationPause(bool isPaused) {
            IronSource.Agent.onApplicationPause(isPaused);
        }

        private static void OnSdkInitializedEvent() {
            Log.Info("[IronSource] initialize completed.");

            initializeEvent.Invoke(true);
        }

#endif

#if IRON_SOURCE
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModule() {
            GameManager.RegisterModule<Initializer>();
        }

        public class Initializer : ModuleInitializer {
            public override int Order => PLUGIN;
            public override InitializeEvent InitializeEvent => IronSourceManager.initializeEvent;

            public override void Initialize() {
                IronSourceManager.Initialize();
            }
        }
#endif
    }
}
