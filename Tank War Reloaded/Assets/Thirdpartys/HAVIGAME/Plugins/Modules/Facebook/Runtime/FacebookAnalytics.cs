using HAVIGAME.Services.Analytics;

#if FACEBOOK
using Facebook.Unity;
#endif

namespace HAVIGAME.Plugins.Facebook {

#if FACEBOOK
    public class FacebookAnalytics : IAnalyticService {

        public InitializeEvent InitializeEvent => FacebookManager.initializeEvent;
        public bool IsInitialized => FacebookManager.IsInitialized;

        public void Initialize() {
            Log.Debug("[FacebookAnalytics] Facebook analytics will initialize with Facebook.");
        }

        public void SetProperty(string propertyName, object propertValue) {
            if (!IsInitialized) {
                Log.Warning("[FacebookAnalytics] Facebook no initialize.");
                return;
            }
        }

        public void LogEvent(AnalyticEvent analyticEvent) {
            if (!IsInitialized) {
                Log.Warning("[FacebookAnalytics] Facebook no initialize.");
                return;
            }

            FB.LogAppEvent(analyticEvent.Name, null, analyticEvent.BuildFacebook());
        }
    }
#endif

    
    [CategoryMenu("Facebook Analytics")]
    [System.Serializable]
    public class FacebookAnalyticServiceProvider : AnalyticServiceProvider {
        public override IAnalyticService GetService() {
#if FACEBOOK
            return new FacebookAnalytics();
#else
            return null;
#endif
        }
    }
}
