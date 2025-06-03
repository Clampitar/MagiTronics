using Terraria;
using Terraria.DataStructures;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class AutoUsorUI : UIState
    {
        private InventoryUI inventory;
        public Point16 position;
        public BankSystem.BankType bankType { get => inventory.bankType; set => inventory.bankType = value; }
        public Item[] inv { get => inventory.Inv; set => inventory.Inv = value; }
        public override void OnInitialize()
        {
            inventory = new InventoryUI();
            Append(inventory);
        }
    }
}
