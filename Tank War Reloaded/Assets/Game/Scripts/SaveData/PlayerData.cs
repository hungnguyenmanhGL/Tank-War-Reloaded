using HAVIGAME.SaveLoad;
using UnityEngine;

[System.Serializable]
public class PlayerData : SaveData {
    [SerializeField] private int level;
    [SerializeField] private int exp;

    public int Level => level;
    public int Exp => exp;

    public PlayerData() : base() {
        level = 1;
        exp = 0;
    }

    public override void OnAfterLoad() {
        base.OnAfterLoad();

        LevelUp();
    }

    public bool AddExp(int expToAdd, out int overflowedExp) {
        int expRequire = GetExpRequire();
        int maxExpToAdd = expRequire - Exp;

        if (expToAdd < maxExpToAdd) {
            this.exp += expToAdd;
            overflowedExp = 0;
            SetChanged();
            return false;
        }
        else {
            this.exp += maxExpToAdd;
            overflowedExp = expToAdd - maxExpToAdd;
            SetChanged();
            return LevelUp();
        }
    }

    public bool LevelUp() {
        int expRequire = GetExpRequire();
        if (Exp >= expRequire) {
            this.exp -= expRequire;
            this.level++;
            SetChanged();
            return true;
        }
        else {
            return false;
        }
    }

    public int GetExpRequire() {
        return GetExpRequire(level + 1);
    }

    public int GetExpRequire(int level) {
        return level * 2;
    }
}
