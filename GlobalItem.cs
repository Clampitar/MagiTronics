using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics
{
    internal class GlobalItem : Terraria.ModLoader.GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            if(item.type == ItemID.WireCutter)
            {
                MagitronicsWorld.RemoveData(Player.tileTargetX, Player.tileTargetY);
                return null;
            }
            return null;
        }
    }
}
