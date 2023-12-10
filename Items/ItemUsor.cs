
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class ItemUsor : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.ItemUsor>();
            Item.mech = true;
        }
    }
}
