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

        public Point16 Target()
        {
            Point16 target = Point16.NegativeOne;
            Item item = Main.LocalPlayer.HeldItem;
            foreach (Point16 point in wiredTerminals)
            {
                Tile tile = Main.tile[point.X, point.Y];
                ushort type = tile.TileType;
                if (Main.tileAxe[type])
                {
                    if (item.axe > 0)
                    {
                        return point;
                    }
                } else if (Main.tileHammer[type])
                {
                    if (item.hammer > 0) { return point; }
                }
                else if (WorldGen.CanKillTile(point.X, point.Y))
                {
                    if(item.pick >  0) { return point; }
                }
                if(WorldGen.CanPoundTile(point.X, point.Y))
                {
                    if(item.axe + item.pick + item.hammer > 0 || item.createTile != -1) { target = point; }
                }
                if (! tile.HasTile)
                {
                    if(item.createTile != -1) { return point; }
                }
            }
            return target;
        }

        private void UpdateTarget()
        {
            workingTE = this;
            wiredTerminals.Clear();
            //Wiring.TripWire(this.Position.X, Position.Y, 2, 2);
            TerminalChecker.TripWire(Position.X, Position.Y, 2, 2);
            workingTE = null;
        }

        public override void Update()
        {
            UpdateTarget();
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

        public static bool IsResting => workingTE is null;


    }
}
