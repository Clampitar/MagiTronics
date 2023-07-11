using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Tiles
{
    internal class TERedirector : ModTileEntity
    {

        private static TERedirector workingTE;

        public List<Point16> wiredTerminals = new();

        public Point16 target;

        public bool CanKill()
        {
            //TODO implement
            workingTE = this;
            wiredTerminals.Clear();
            Wiring.TripWire(this.Position.X, Position.Y, 2, 2);
            workingTE = null;
            foreach (Point16 point in wiredTerminals)
            {
                if(WorldGen.CanKillTile(point.X, point.Y))
                {
                    target = point;
                    return true;
                }
            }
            target = Point16.NegativeOne;
            return false;
        }


        public static void registerTerminal(Point16 p)
        {
            workingTE?.wiredTerminals.Add(p);
        }


        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<Redirector>();
        }


        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                int width = 2;
                int height = 2;
                NetMessage.SendTileSquare(Main.myPlayer, x, y, width, height);

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: x, number2: y, number3: Type);
            }

            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            // Set "tileOrigin" to the same value you set TileObjectData.newTile.Origin to in the ModTile
            Point16 tileOrigin = new Point16(0, 1);
            int placedEntity = Place(x - tileOrigin.X, y - tileOrigin.Y);
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        
    }
}
