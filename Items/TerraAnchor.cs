using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class TerraAnchor : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.ResearchUnlockCount = 10;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.AddIngredient(ItemID.StoneBlock, 1);
            recipe.AddIngredient(ItemID.MudBlock, 1);
            recipe.AddIngredient(ItemID.SandBlock, 1);
            recipe.AddIngredient(ItemID.AshBlock, 1);
            recipe.AddIngredient(ItemID.Cloud, 1);
            recipe.AddIngredient(ItemID.GlowingMushroom, 1);
            recipe.AddRecipeGroup(RecipeGroupID.Wood, 1);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}
