using MagiTronics.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class AutoUsorUI : UIState
    {
        public UsorInventory inventory;

        public TEItemUsor Usor { get => inventory.Usor; set => inventory.Usor = value; }
        public override void OnInitialize()
        {
            inventory = new UsorInventory();
            Append(inventory);
        }
    }
}
