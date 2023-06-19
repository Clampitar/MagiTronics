using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class ChestInletPump : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.ChestInletPump>();
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("IronBar", 10);
            recipe.AddIngredient(ItemID.Wire, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Description", "Extracts liquids from chests");
            tooltips.Add(line);
            line = new TooltipLine(Mod, "Description", "Also sends water to outlet pumps");
            tooltips.Add(line);
            line = new TooltipLine(Mod, "Description", "Requires buckets");
            tooltips.Add(line);
        }
    }
}
