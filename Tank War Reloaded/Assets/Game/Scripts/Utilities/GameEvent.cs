using HAVIGAME;

public static class GameEvent {
    public const int playerInventoryChanged = 100;

    public class PlayerInventoryChanged : EventArgs {
        public ItemContainer Inventory { get; private set; }
        public ItemStack ItemStackChange { get; private set; }
        public bool IsAdded { get; private set; }

        public static PlayerInventoryChanged Create(object sender, ItemContainer inventory, ItemStack itemStackChange, bool isAdded) {
            PlayerInventoryChanged args = Create<PlayerInventoryChanged>(sender);
            args.Inventory = inventory;
            args.ItemStackChange = itemStackChange;
            args.IsAdded = isAdded;
            return args;
        }

        public override void Clear() {
            base.Clear();

            Inventory = null;
        }
    }
}
