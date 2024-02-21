using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class AutoPicker : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AutoPicker>());
            Item.mech = true;
        }
    }
}
