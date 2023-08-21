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
            if (item.IsAir) return target;
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
                else if (WorldGen.CanKillTile(point.X, point.Y) && item.pick > 0)
                {
                    return point;
                }
                else if(item.hammer > 0 && tile.WallType > 0)
                {
                    return point;
                }
                switch (item.type)
                {
                    case ItemID.LawnMower:
                        if (tile.HasUnactuatedTile && (type == TileID.Grass || type == TileID.HallowedGrass))
                        {
                            return point;
                        }
                        break;
                    case ItemID.StaffofRegrowth:
                    case ItemID.AcornAxe:
                        switch(type)
                        {
                            case TileID.ImmatureHerbs:
                            case TileID.MatureHerbs:
                            case TileID.BloomingHerbs:
                            case TileID.Dirt:
                                return point;
                        }
                        if (tile.HasTile) target = point;
                        break;
                }
                if(item.createWall > -1)
                {
                    if(WallLoader.CanPlace(point.X, point.Y, item.createWall) && Main.tile[point.X, point.Y].WallType != item.createWall)
                    {
                        if (Main.tile[point.X+1, point.Y].HasTile || Main.tile[point.X+1, point.Y].WallType > 0
                            || Main.tile[point.X, point.Y + 1].HasTile || Main.tile[point.X, point.Y + 1].WallType > 0
                            || Main.tile[point.X - 1, point.Y].HasTile || Main.tile[point.X - 1, point.Y].WallType > 0
                            || Main.tile[point.X, point.Y - 1].HasTile || Main.tile[point.X, point.Y - 1].WallType > 0)
                        {
                            if (Main.tile[point.X, point.Y].WallType > 0)
                            {
                                if (Main.LocalPlayer.TileReplacementEnabled
                                    && WorldGen.NearFriendlyWall(point.X, point.Y)
                                    && !(Main.wallDungeon[Main.tile[point.X, point.Y].WallType] && !NPC.downedBoss3)
                                    && !(Main.tile[point.X, point.Y].WallType == WallID.LihzahrdBrickUnsafe && !NPC.downedGolemBoss))
                                {
                                    return point;
                                }
                            }
                            else
                            {
                                return point;
                            }
                        }
                    }
                }
                if (item.createTile != -1 && !tile.HasTile )
                {
                    if (TileID.Sets.Torch[item.createTile])
                    {
                        Tile tile2 = Main.tile[point.X - 1, point.Y];
                        Tile tile3 = Main.tile[point.X + 1, point.Y];
                        Tile tile4 = Main.tile[point.X, point.Y + 1];

                        if ((!TileID.Sets.AllowLightInWater[Main.LocalPlayer.BiomeTorchHoldStyle(type)] || tile.LiquidType <= 0) &&
                        (tile.WallType > 0 ||
                        (tile2.HasTile && (tile2.Slope == SlopeType.Solid || tile2.Slope == SlopeType.SlopeDownRight || tile2.Slope == SlopeType.SlopeUpRight) && ((Main.tileSolid[tile2.TileType] && !Main.tileNoAttach[tile2.TileType] && !Main.tileSolidTop[tile2.TileType] && !TileID.Sets.NotReallySolid[tile2.TileType]) || TileID.Sets.IsBeam[tile2.TileType] || (WorldGen.IsTreeType(tile2.TileType) && WorldGen.IsTreeType(Main.tile[point.X - 1, point.Y - 1].TileType) && WorldGen.IsTreeType(Main.tile[point.X - 1, point.Y + 1].TileType))))
                        || (tile3.HasTile && (tile3.Slope == 0 || tile3.Slope == SlopeType.SlopeDownLeft || tile3.Slope == SlopeType.SlopeUpLeft) && ((Main.tileSolid[tile3.TileType] && !Main.tileNoAttach[tile3.TileType] && !Main.tileSolidTop[tile3.TileType] && !TileID.Sets.NotReallySolid[tile3.TileType]) || TileID.Sets.IsBeam[tile3.TileType] || (WorldGen.IsTreeType(tile3.TileType) && WorldGen.IsTreeType(Main.tile[point.X + 1, point.Y - 1].TileType) && WorldGen.IsTreeType(Main.tile[point.X + 1, point.Y + 1].TileType))))
                        || (tile4.HasTile && Main.tileSolid[tile4.TileType] && !Main.tileNoAttach[tile4.TileType] && (!Main.tileSolidTop[tile4.TileType] || (TileID.Sets.Platforms[tile4.TileType] && tile4.Slope == 0)) && !TileID.Sets.NotReallySolid[tile4.TileType] && !tile4.IsHalfBlock && tile4.Slope == 0))
                        && !TileID.Sets.Torch[tile.TileType])
                        {
                            Main.NewText("torch spot: "+point);
                            return point;
                        }
                    } else
                    {
                        return point;
                    }
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
