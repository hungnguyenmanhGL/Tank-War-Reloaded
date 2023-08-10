using UnityEngine;

namespace HAVIGAME.Plugins.Firebases {

    [DefineSymbols(FirebaseManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(FirebaseSettings), "Plugins/Firebase", "", "https://firebase.google.com/docs/unity/setup", 201, "Icons/icon_firebase")]
    [CreateAssetMenu(fileName = "FirebaseSettings", menuName = "HAVIGAME/Settings/Plugins/Firebase")]
    public class FirebaseSettings : Database<FirebaseSettings> {
        [SerializeField] private bool fetchAndActiveOnInitialize = true;

        public bool FetchAndActiveOnInitialize => fetchAndActiveOnInitialize;
    }
}
