namespace HAVIGAME.Services.Analytics {

    public interface IAnalyticService {
        public InitializeEvent InitializeEvent { get; }
        public bool IsInitialized { get; }
        public void Initialize();
        public void SetProperty(string propertyName, object propertValue);
        public void LogEvent(AnalyticEvent analyticEvent);
    }

    [System.Serializable]
    public abstract class AnalyticServiceProvider : ServiceProvider<IAnalyticService> {

    }
}
