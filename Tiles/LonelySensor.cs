using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using MagiTronics.Items;
using Terraria.Audio;

namespace MagiTronics.Tiles
{
    internal class LonelySensor : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            RegisterItemDrop(ModContent.ItemType<Items.LonelySensor>());

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        private static Point16[] AdjecentTiles => new Point16[] {
            new Point16(-1, -1), 
            new Point16(-1, 0),
            new Point16(-1, 1),
            new Point16(0, -1),
            new Point16(0, 1),
            new Point16(1, -1),
            new Point16(1, 0),
            new Point16(1, 1)
        };


        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            UpdateLoneliness(i, j, false);
            return true;
        }

        public static void WallChanged(int x, int y, bool killWall)
        {
            foreach(Point16 p in AdjecentTiles)
            {
                int px = p.X + x;
                int py = p.Y + y;
                if (px > 5 && py > 5 && px < Main.maxTilesX - 5 && py < Main.maxTilesY - 5 && Main.tile[px, py] != null)
                {
                    Tile tile = Main.tile[px, py]; ;
                    if (tile.HasTile && tile.TileType == ModContent.TileType<LonelySensor>())
                    {
                        UpdateLoneliness(p.X + x, p.Y + y, killWall);
                    }
                }
            }
            Tile here = Main.tile[x, y];
            if (here.HasTile && here.TileType == ModContent.TileType<LonelySensor>())
            {
                UpdateLoneliness(x, y, killWall);
            }
        }

        private static void UpdateLoneliness(int x, int y, bool killWall)
        {
            bool lonely = true;
            foreach (Point16 p in AdjecentTiles) {
                int px = p.X + x;
                int py = p.Y + y;
                if (px > 5 && py > 5 && px < Main.maxTilesX - 5 && py < Main.maxTilesY - 5 && Main.tile[px, py] != null)
                {
                    Tile adjecent = Main.tile[px, py];
                    if (adjecent.HasTile || adjecent.WallType != WallID.None)
                    {
                        if (killWall && !adjecent.HasTile)
                        {
                            killWall = false;
                            continue;
                        }
                        else
                        {
                            lonely = false;
                            break;
                        }
                    }
                }
            }
            Tile tile = Main.tile[x, y];
            if (tile.WallType != WallID.None && !killWall) lonely = false;
            bool active = tile.TileFrameX == 18;
            if (active != lonely)
            {
                Wiring.TripWire(x, y, 1, 1);
                tile.TileFrameX = (short)(lonely ? 18 : 0);
                SoundEngine.PlaySound(SoundID.Mech, new Microsoft.Xna.Framework.Vector2(x, y));
                if(Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(Main.netMode == NetmodeID.MultiplayerClient ? Main.myPlayer : -1, x, y, 1, 1);
                }
            }
        }
    }
}
