using MagiTronics.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class AutoUsorUI : UIState
    {
        public UsorInventory inventory;

        public Player Player { get => inventory.Player; set => inventory.Player = value; }
        public override void OnInitialize()
        {
            inventory = new UsorInventory();
            Append(inventory);
        }
    }
}
