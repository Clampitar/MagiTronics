
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
            Item.value = 50000;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Teleporter, 1);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ModContent.ItemType<UsageTerminal>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
