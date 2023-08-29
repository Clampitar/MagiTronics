﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Items
{
    internal class UsageTerminal : ModItem
    {

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.ammo = Item.type;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAmmo = Item.type;
            Item.useTime = 5;
            Item.mech = true;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.value = 1000;
            Item.ResearchUnlockCount = 50;
        }



        public override bool CanUseItem(Player player)
        {
            if (player.noBuilding)
                return false;
            return true;
        }

        public override bool? UseItem(Player player)
        {
            int ammoIndex = -1;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].stack > 0 && player.inventory[i].type == ModContent.ItemType<UsageTerminal>())
                {
                    ammoIndex = i;
                    break;
                }

            }
            if (ammoIndex > 0 && (Main.mouseLeft || !Main.mouseLeftRelease))
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
