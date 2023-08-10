using HAVIGAME.Services.Analytics;

namespace HAVIGAME.Plugins.Firebases {
#if FIREBASE && FIREBASE_ANALYTICS
    public class FirebaseAnalytics : IAnalyticService {

        public InitializeEvent InitializeEvent => FirebaseManager.initializeEvent;

        public bool IsInitialized => FirebaseManager.IsInitialized;

        public void Initialize() {
            Log.Debug("[FirebaseAnalytics] Firebase analytics will initialize with Firebase.");
        }

        public void LogEvent(AnalyticEvent analyticEvent) {
            if (!IsInitialized) {
                Log.Warning("[FirebaseAnalytics] Firebase no initialize.");
                return;
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(analyticEvent.Name, analyticEvent.BuildFirebase());
        }

        public void SetProperty(string propertyName, object propertValue) {
            if (!IsInitialized) {
                Log.Warning("[FirebaseAnalytics] Firebase no initialize.");
                return;
            }

            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(propertyName, propertValue.ToString());
        }
    }
#endif

    [CategoryMenu("Firebase Analytics")]
    [System.Serializable]
    public class FirebaseAnalyticServiceProvider : AnalyticServiceProvider {
        public override IAnalyticService GetService() {
#if FIREBASE && FIREBASE_ANALYTICS
            return new FirebaseAnalytics();
#else
            return null;
#endif
        }
    }
}