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
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TETickTimer>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
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
