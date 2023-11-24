using System.Linq;
using Terraria;
using Terraria.DataStructures;
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
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(419, 0));
            TileObjectData.newTile.AnchorAlternateTiles = new int[5] { 420, 419, Type, ModContent.TileType<TickTimer>(), ModContent.TileType<SignalCounter>() };
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[4] { 419, Type, ModContent.TileType<TickTimer>(), ModContent.TileType<SignalCounter>() };
            TileObjectData.addTile(419);
        }

        public override void HitWire(int i, int j)
        {
            Wiring.SkipWire(i, j);
            MagicWiring.BuffersHit.Enqueue(new Point16(i, j));
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

        public static void UpdatedLamp(int x, int y)
        {
            for(int j= y; j< Main.maxTilesY; j++)
            {
                Tile tile = Main.tile[x, j];
                if (!tile.HasTile) return;
                if (tile.TileType == ModContent.TileType<LogicBuffer>())
                {
                    Tile tileDown = Main.tile[x, j + 1];
                    if (tileDown.HasTile && tileDown.TileType == TileID.LogicGate)
                    {
                        int gateType = tileDown.TileFrameY / 18;
                        if (MagicWiring.SatisfiesGate(x, j - 1, gateType))
                        {
                            tileDown.TileFrameX = 18;
                        }
                        else
                        {
                            tileDown.TileFrameX = 0;
                        }
                        WorldGen.SquareTileFrame(x, j + 1);
                        if(Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(Main.myPlayer, x, j + 1);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, x, j + 1);
                        }
                    }
                }
                else if (tile.TileType != TileID.LogicGateLamp) return;
            }
        }

    }
}
