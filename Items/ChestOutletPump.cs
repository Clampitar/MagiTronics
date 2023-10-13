using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace MagiTronics.Items
{
    public class ChestOutletPump : ModItem
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
            Item.createTile = ModContent.TileType<Tiles.ChestOutletPump>();
            Item.mech = true;
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
            var line = new TooltipLine(Mod, "Description", "Transfers liquids to chests");
            tooltips.Add(line);
            line = new TooltipLine(Mod, "Description", "Can also recieve water from inlet pumps");
            tooltips.Add(line);
            line = new TooltipLine(Mod, "Description", "Requires buckets or sponge");
            tooltips.Add(line);
        }
    }
}
