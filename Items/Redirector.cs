
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MagiTronics.Items
{
    internal class Redirector : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Redirector>();
        }
    }
}
