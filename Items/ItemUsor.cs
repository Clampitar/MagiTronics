
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class ItemUsor : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ItemUsor>());
            Item.mech = true;
        }
    }
}
