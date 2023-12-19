using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class MenuBar : UIState
    {
        public PlayButton icon;
        public override void OnInitialize()
        {
            icon = new PlayButton(new Item(ItemID.Abeemination));

            Append(icon);
        }
    }
}
