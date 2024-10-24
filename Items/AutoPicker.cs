using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class AutoPicker : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AutoPicker>());
            Item.mech = true;
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DeathbringerPickaxe, 1);
            recipe.AddIngredient(ModContent.ItemType<Redirector>(), 1);
            recipe.AddIngredient(ItemID.TimerOneFourthSecond, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.NightmarePickaxe, 1);
            recipe.AddIngredient(ModContent.ItemType<Redirector>(), 1);
            recipe.AddIngredient(ItemID.TimerOneFourthSecond, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }


    }
}
