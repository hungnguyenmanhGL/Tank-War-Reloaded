using System.Collections.Generic;

namespace HAVIGAME.Services.Advertisings {
    public delegate void AdDelegate(AdUnit type, AdEventArgs args);
    public delegate void AdRevenuePaidDelegate(AdUnit unit, AdRevenuePaid adRevenuePaid);

    public delegate void AdClientRevenuePaidDelegate(AdService client, AdRevenuePaid adRevenuePaid);
    public delegate void AdClientDelegate(AdService client, AdUnit unit, AdEventArgs args);
    public delegate void AdClientInitializeDelegate(AdService client, bool isInitialized);

    public sealed class AdEventArgs {
        public const string network = "network";
        public const string adUnit = "adUnit";
        public const string id = "id";
        public const string placement = "placement";
        public const string info = "info";
        public const string error = "error";

        public const string adLoad = "ad_load";
        public const string adLoaded = "ad_loaded";
        public const string adLoadFailed = "ad_load_failed";
        public const string adDisplay = "ad_display";
        public const string adDisplayed = "ad_displayed";
        public const string adDisplayFailed = "ad_failed";
        public const string adClicked = "ad_clicked";
        public const string adClosed = "ad_closed";


        private string name;
        private Dictionary<string, string> paramaters;

        public string Name => name;

        public Dictionary<string, string> Paramaters => paramaters;

        private AdEventArgs(string name) {
            this.name = name;
            this.paramaters = new Dictionary<string, string>(4);
        }

        private AdEventArgs(string name, Dictionary<string, string> paramaters) {
            this.name = name;
            this.paramaters = paramaters;
        }

        public string Get(string parameterName, string defaultParameterValue = "NULL") {
            if (paramaters.ContainsKey(parameterName)) {
                return paramaters[parameterName];
            }
            else {
                return defaultParameterValue;
            }
        }

        public static AdEventArgs Create(string name) {
            return new AdEventArgs(name);
        }

        public static AdEventArgs Create(string name, Dictionary<string, string> paramaters) {
            return new AdEventArgs(name, paramaters);
        }

        public AdEventArgs Add(string parameterName, string parameterValue) {
            paramaters[parameterName] = parameterValue;
            return this;
        }

        public override string ToString() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(Name);
            foreach (var item in paramaters) {
                sb.Append($"\n{item.Key}: {item.Value}");
            }
            return sb.ToString();
        }
    }
}
