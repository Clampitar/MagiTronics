

using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class LogicBuffer : ModItem
    {

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.LogicBuffer>();
            Item.mech = true;
            Item.value = 20000;
            Item.ResearchUnlockCount = 5;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
