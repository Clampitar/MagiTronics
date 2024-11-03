using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MagiTronics.Tiles
{
    internal class TEAutoPicker : TEItemUsor
    {
        private static readonly int pickTime = new Item(ItemID.NightmarePickaxe).useTime;

        private int useTime = 0;

        static readonly int power = new Item(ItemID.DeathbringerPickaxe).pick;

        private delegate bool MapCondition(Point16 p);

        MapCondition InMap;

        private delegate Point16 SearchDirection(int i = 1);

        SearchDirection Go;
        public enum Direction
        {
            STOP = 0,
            UP = 1,
            RIGHT = 2,
            DOWN = 3,
            LEFT = 4
        }

        private Direction dir = Direction.STOP;

        public TEAutoPicker()
        {
            dir = Direction.STOP;
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == ModContent.TileType<AutoPicker>());
        }

        public override void Update()
        {
            useTime--;
            if (useTime <= 0)
            {
                Pick();
                useTime = pickTime;
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(usorPlayer.position.X);
            writer.Write(usorPlayer.position.Y);
        }

        public override void NetReceive(BinaryReader reader)
        {
            usorPlayer.position.X = reader.ReadSingle();
            usorPlayer.position.Y = reader.ReadSingle();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Set("dir", (short)dir);
            Vector2 pos = usorPlayer.position;
            tag.Set("pos", pos);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("dir"))
            {
                ChangeDir((Direction)tag.GetAsShort("dir"));
            }
            if (tag.ContainsKey("pos"))
            {
                usorPlayer.position = tag.Get<Vector2>("pos");
            }
        }

        public void ChangeDir(Direction direction)
        {
            if ((Main.netMode == NetmodeID.MultiplayerClient))
            {
                ModPacket modPacket = Mod.GetPacket();
                modPacket.Write((byte)MagiTronics.PacketId.PICKERDIR);
                modPacket.Write(Position.X);
                modPacket.Write(Position.Y);
                modPacket.Write((byte)direction);
                modPacket.Send();
                return;
            }
            dir = direction;
            switch (direction)
            {
                case Direction.STOP:
                    InMap = (Point16 p) => false;
                    break;
                case Direction.UP:
                    InMap = (Point16 p) => p.Y > 0;
                    Go = (int i = 1) => new Point16(0, -i);
                    break;
                case Direction.RIGHT:
                    InMap = (Point16 p) => p.X < Main.maxTilesX;
                    Go = (int i = 1) => new Point16(i, 0);
                    break;
                case Direction.DOWN:
                    InMap = (Point16 p) => p.Y < Main.maxTilesY;
                    Go = (int i = 1) => new Point16(0, i);
                    break;
                case Direction.LEFT:
                    InMap = (Point16 p) => p.X > 0;
                    Go = (int i = 1) => new Point16(-i, 0);
                    break;
            }
        }

        public void PickTile(int x, int y)
        {
            usorPlayer.PickTile(x, y, power);
        }

        private void Pick()
        {
            if (dir == Direction.STOP) { return; }
            List<Point16> terminals = TerminalChecker.TripWire(Position.X, Position.Y, 3, 3);
            int mindepth = -1;
            Point16 p = Point16.NegativeOne;
            foreach (Point16 terminal in terminals)
            {
                int depth = Pickterminal(terminal);
                if (depth >= 0 && (depth < mindepth || mindepth == -1))
                {
                    mindepth = depth;
                    p = terminal;
                }
            }
            if(p!= Point16.NegativeOne && mindepth != -1)
            {
                p += Go(mindepth);
                if (Main.netMode == NetmodeID.Server)
                {
                    useTime = pickTime;
                    ModPacket modPacket = Mod.GetPacket();
                    modPacket.Write((byte)MagiTronics.PacketId.PICKERDIR);
                    modPacket.Write(Position.X);
                    modPacket.Write(Position.Y);
                    modPacket.Write(p.X);
                    modPacket.Write(p.Y);
                    /*for (int playerIndex = 0; playerIndex< 256; playerIndex++)
                    {
                        if (Netplay.Clients[playerIndex].IsConnected())
                        {
                            modPacket.Send(toClient: playerIndex);
                            break;
                        }
                    }*/
                    modPacket.Send();
                }
                else
                {
                    PickTile(p.X, p.Y);
                }
            }
        }

        private int Pickterminal(Point16 terminal)
        {
            int depth = 0;
            do
            {
                Tile tile = Main.tile[terminal.X, terminal.Y];
                int type = tile.TileType;
                if (tile.HasTile && WorldGen.CanKillTile(terminal.X, terminal.Y) && !Main.tileHammer[type] && !Main.tileAxe[type])
                {
                    return depth;
                }
                depth++;
                terminal += Go();
            } while (InMap(terminal));
            return -1;
        }

    }
}
