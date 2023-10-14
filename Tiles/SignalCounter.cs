using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace MagiTronics.Tiles
{
    internal class SignalCounter : ModTile
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
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
        }

        public override void HitWire(int x, int y)
        {
            for (int lampY = y-1; lampY > Main.miniMapY; lampY--)
            {
                Tile tile = Main.tile[x, lampY];
                if (!tile.HasTile || tile.TileType != TileID.LogicGateLamp)
                {
                    break;
                }
                //Wiring.TripWire(x, lampY, 1, 1);
                Wiring.SkipWire(x, lampY);
                Wiring.SkipWire(x, y);
                if (tile.TileFrameX == 0)
                {
                    tile.TileFrameX = 18;
                    break;
                }
                if(tile.TileFrameX == 18){
                    tile.TileFrameX = 0;
                }
                if(tile.TileFrameX == 36) {
                    if (MagicWiring.rand.Next(0, 2) == 0)
                        break;
                }
            }
            Wiring.SkipWire(x, y);
        }
    }
}
