using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MagiTronics.Tiles
{
    public class LogicBuffer : ModTile
    {
        public int[] belowTiles = new int[] {
            TileID.LogicGate
        };
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.addTile(Type);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorAlternateTiles = new int[4] { 420, 419, Type, ModContent.TileType<TickTimer>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[2] { 419, Type };
            TileObjectData.addTile(419);
        }

        public override void HitWire(int i, int j)
        {
            //Wiring.SkipWire(i, j);
            Tile tileDown = Main.tile[i, j + 1];
            if(tileDown.HasTile && tileDown.TileType == TileID.LogicGate)
            {
                int gateType = tileDown.TileFrameY / 18;
                if(MagicWiring.SatisfiesGate(i, j-1, gateType))
                {
                    tileDown.TileFrameX = 18;
                    Wiring.SkipWire(i, j + 1);
                    Wiring.TripWire(i, j + 1, 1, 1);
                }
                else
                {
                    tileDown.TileFrameX = 0;
                }
            }
            Wiring.SkipWire(i, j);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileDown = Main.tile[i, j + 1];
            if(tileDown.HasTile && belowTiles.Contains(tileDown.TileType))
            {
                return true;
            }
            //WorldGen.KillTile(i, j);
            return false;
        }

    }
}
