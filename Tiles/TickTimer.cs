using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace MagiTronics.Tiles
{
    internal class TickTimer : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TETickTimer>().Hook_AfterPlacement, 0, 0, false);
            TileObjectData.addTile(Type);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<TETickTimer>().Kill(i, j);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override bool RightClick(int i, int j)
        {
            Switch(i, j);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            Switch(i, j);
            Wiring.SkipWire(i, j);
        }

        private void Switch(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te) && te is TETickTimer)
            {
                TETickTimer tt = (TETickTimer)te;
                if (tile.TileFrameY == 0)
                {
                    tile.TileFrameY = 18;
                    tt.Activate();
                }
                else
                {
                    tile.TileFrameY = 0;
                    tt.Deactivate();
                }
            }
        }
    }
}
