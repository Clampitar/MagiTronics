using Terraria;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class UsageTerminal : ModItem
    {

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.ammo = Item.type;
        }

        
    }
}
