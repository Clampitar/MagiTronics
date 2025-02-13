using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items.Worldly
{
    internal class VoidVault : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.createTile = ModContent.TileType<Tiles.Worldly.VoidVault>();
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 30000;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.VoidVault, 1);
            recipe.AddIngredient(ModContent.ItemType<TerraAnchor>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
