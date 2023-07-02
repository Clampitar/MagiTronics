using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class PickorWrench : ModItem
    {

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = false;
        }
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAmmo = ModContent.ItemType<UsageTerminal>();
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.width = 24;
            Item.height = 28;
            Item.rare = 1;
            Item.value = 20000;
            Item.mech = true;
            Item.tileBoost = 20;
        }

        public override void OnConsumeAmmo(Item ammo, Player player)
        {
            base.OnConsumeAmmo(ammo, player);
        }

        public override bool CanUseItem(Player player)
        {
            if(player.noBuilding)
                return false;
            return true;
        }

        public override bool? UseItem(Player player)
        {
            int x = Player.tileTargetX;
            int y = Player.tileTargetY;


            MagitronicsWorld.AddData(new Terraria.DataStructures.Point16(x, y));


            return true;
        }

        public void DrawWire(Player player)
        {
            //Main.cursor
            CombatText.NewText(player.getRect(), new Microsoft.Xna.Framework.Color(250, 40, 80), 45);
            
        }
    }
}
