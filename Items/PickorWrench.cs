using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI;
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
            Item.rare = ItemRarityID.Blue;
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
            int ammoIndex = -1;
            for(int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].stack > 0 && player.inventory[i].type == ModContent.ItemType<UsageTerminal>())
                {
                    ammoIndex = i;
                    break;
                }

            }
            if (ammoIndex > 0)
            {
                int x = Player.tileTargetX;
                int y = Player.tileTargetY;

                if (MagitronicsWorld.AddData(new Terraria.DataStructures.Point16(x, y)))
                    player.inventory[ammoIndex].stack--;
            }

            return true;
        }
    }
}
