using UnityEngine;
using HAVIGAME;

public class ItemListener : MonoBehaviour {
    [SerializeField, ConstantField(typeof(ItemID))] private int itemID;
    [SerializeField] private ItemView itemView;

    private void Start() {
        OnPlayerInventoryChanged(GameEvent.PlayerInventoryChanged.Create(this, GameData.PlayerInventory, new ItemStack(itemID, 0), true));

        EventDispatcher.AddListener(GameEvent.playerInventoryChanged, OnPlayerInventoryChanged);
    }

    private void OnDestroy() {
        EventDispatcher.RemoveListener(GameEvent.playerInventoryChanged, OnPlayerInventoryChanged);
    }

    private void OnPlayerInventoryChanged(EventArgs eventArgs) {
        if (eventArgs is GameEvent.PlayerInventoryChanged e) {
            if (e.ItemStackChange.Id == itemID) {
                ItemStack current = e.Inventory.Get(itemID);

                itemView.SetModel(current);
                itemView.Show();
            }
        }
    }
}
