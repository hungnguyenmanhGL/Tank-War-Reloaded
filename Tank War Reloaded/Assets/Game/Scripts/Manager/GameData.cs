using HAVIGAME.SaveLoad;

public static class GameData {
    private static DataHolder<DataInfo> info;
    private static DataHolder<PlayerData> player;
    private static DataHolder<PlayerLevel> level;
    private static DataHolder<PlayerInventory> inventory;
    private static DataHolder<PlayerRemoteConfig> remoteConfig;
    private static DataHolder<PlayerShop> shop;

    public static DataInfo DataInfo => info.Data;
    public static PlayerData PlayerData => player.Data;
    public static PlayerLevel PlayerLevel => level.Data;
    public static PlayerInventory PlayerInventory => inventory.Data;
    public static PlayerRemoteConfig PlayerRemoteConfig => remoteConfig.Data;
    public static PlayerShop PlayerShop => shop.Data;

    public static void Initialize() {

        info = SaveLoadManager.Create<DataInfo>("info");
        player = SaveLoadManager.Create<PlayerData>("player");
        level = SaveLoadManager.Create<PlayerLevel>("level");
        inventory = SaveLoadManager.Create<PlayerInventory>("inventory");
        remoteConfig = SaveLoadManager.Create<PlayerRemoteConfig>("remoteConfig");
        shop = SaveLoadManager.Create<PlayerShop>("shop");

        if (DataInfo.Version != DataInfo.dataVersion) {
            //Update data to new version
        }
    }

    public static void Save() {
        SaveLoadManager.Save();
    }

    public static void Load(bool force = false) {
        SaveLoadManager.Load(force);
    }
}