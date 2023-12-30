using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class MenuBar : UIState
    {
        public UsorInventory icon;

        public Player Player { get => icon.Player; set => icon.Player = value; }
        public override void OnInitialize()
        {
            icon = new UsorInventory();

            Append(icon);
        }
    }
}
