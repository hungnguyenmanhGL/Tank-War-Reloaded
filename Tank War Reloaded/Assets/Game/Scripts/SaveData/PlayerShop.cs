using HAVIGAME.SaveLoad;
using UnityEngine;

public class PlayerShop : SaveData {
    [SerializeField] private bool isPremium;

    public bool IsPremium {
        get {
#if CHEAT
            return true;
#else
            return isPremium;
#endif
        }
    }

    public PlayerShop() : base() {
        isPremium = false;
    }

    public void SetPremium(bool premium) {
        if (isPremium != premium) {
            isPremium = premium;
            SetChanged();
        }
    }
}
