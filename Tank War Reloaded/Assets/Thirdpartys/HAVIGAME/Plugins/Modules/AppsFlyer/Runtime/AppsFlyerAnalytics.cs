using HAVIGAME.Services.Analytics;

namespace HAVIGAME.Plugins.AppsFlyer {

#if APPSFLYER
    public class AppsFlyerAnalytics : IAnalyticService {

        public InitializeEvent InitializeEvent => AppsFlyerManager.initializeEvent; 

        public bool IsInitialized => AppsFlyerManager.IsInitialized;

        public void Initialize() {
            Log.Debug("[AppsFlyerAnalytics] AppsFlyer analytics will initialize with AppsFlyer.");
        }

        public void LogEvent(AnalyticEvent analyticEvent) {
            if (!IsInitialized) {
                Log.Warning("[AppsFlyerAnalytics] Appsflyer no initialize.");
            }

            AppsFlyerSDK.AppsFlyer.sendEvent(analyticEvent.Name, analyticEvent.BuildAppsflyer());
        }

        public void SetProperty(string propertyName, object propertValue) {

        }
    }
#endif


    [CategoryMenu("AppsFlyer Analytics")]
    [System.Serializable]
    public class AppsflyerAnalyticServiceProvider : AnalyticServiceProvider {
        public override IAnalyticService GetService() {
#if APPSFLYER
            return new AppsFlyerAnalytics();
#else
            return null;
#endif
        }
    }
}