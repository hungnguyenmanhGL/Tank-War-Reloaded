using HAVIGAME;
using HAVIGAME.SaveLoad;

[System.Serializable]
public class PlayerInventory : ItemContainer, ISaveData {
    [System.NonSerialized] private bool isChanged;

    public bool IsChanged => isChanged;

    public PlayerInventory() : base() {
        Add(ConfigDatabase.Instance.DefaultInventory);

        isChanged = false;
    }

    public void Dispose() {

    }

    public void OnAfterLoad() {

    }

    public void OnBeforeSave() {
        isChanged = false;
    }

    protected override void OnAdded(ItemStack itemStack) {
        base.OnAdded(itemStack);

        isChanged = true;

        EventDispatcher.Dispatch(GameEvent.playerInventoryChanged, GameEvent.PlayerInventoryChanged.Create(this, this, itemStack, true));
    }

    protected override void OnRemoved(ItemStack itemStack) {
        base.OnRemoved(itemStack);

        isChanged = true;

        EventDispatcher.Dispatch(GameEvent.playerInventoryChanged, GameEvent.PlayerInventoryChanged.Create(this, this, itemStack, false));
    }

    protected override void OnCleared() {
        base.OnCleared();

        isChanged = true;
    }
}
