using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MagiTronics.Tiles
{
    internal class BossSensor : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            RegisterItemDrop(ModContent.ItemType<Items.BossSensor>());

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TEBossSensor>().Hook_AfterPlacement, 0, 0, false);
            TileObjectData.addTile(Type);
        }


        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<TEBossSensor>().Kill(i, j);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        
    }
}
