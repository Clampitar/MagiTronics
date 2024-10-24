
using Terraria;
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
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.TargetDummy, 1);
            recipe.AddIngredient(ItemID.HallowedBar, 30);
            recipe.AddIngredient(ModContent.ItemType<Redirector>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ArtificialCore>(), 1);
            recipe.AddIngredient(ModContent.ItemType<TerraAnchor>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
