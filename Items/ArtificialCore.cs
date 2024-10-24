using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class ArtificialCore : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.ResearchUnlockCount = 10;
            Item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LifeCrystal, 1);
            recipe.AddIngredient(ItemID.LifeFruit, 1);
            recipe.AddIngredient(ItemID.MythrilBar, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
            recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LifeCrystal, 1);
            recipe.AddIngredient(ItemID.LifeFruit, 1);
            recipe.AddIngredient(ItemID.OrichalcumBar, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
