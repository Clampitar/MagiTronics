using Terraria.DataStructures;
using Terraria;
using System.Collections.Generic;
using Terraria.ID;
using MagiTronics.Tiles;
using Terraria.ModLoader;

namespace MagiTronics
{
    internal static class TerminalChecker
    {
        //slightly modified from Vanilla Wiring
        private static Dictionary<Point16, bool> _wireSkip;
        private static Queue<Point16> BuffersToCheck;
        private static Queue<Point16> BuffersCurrent;
        private static Dictionary<Point16, bool> BuffersDone;

        public static void initialize()
        {
            BuffersToCheck = new Queue<Point16>();
            BuffersCurrent = new Queue<Point16>();
            BuffersDone = new Dictionary<Point16, bool>();
        }
        public static void TripWire(int left, int top, int width, int height)
        {
            _wireSkip = new Dictionary<Point16, bool>();
            Wiring.running = true;
            if (Wiring._wireList.Count != 0)
            {
                Wiring._wireList.Clear(quickClear: true);
            }
            if (Wiring._wireDirectionList.Count != 0)
            {
                Wiring._wireDirectionList.Clear(quickClear: true);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.RedWire)
                    {
                        Wiring._wireList.PushBack(back);
                    }
                }
            }
            if (Wiring._wireList.Count > 0)
            {
                HitWire(Wiring._wireList, 1);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.GreenWire)
                    {
                        Wiring._wireList.PushBack(back);
                    }
                }
            }
            if (Wiring._wireList.Count > 0)
            {
                HitWire(Wiring._wireList, 2);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.BlueWire)
                    {
                        Wiring._wireList.PushBack(back);
                    }
                }
            }
            if (Wiring._wireList.Count > 0)
            {
                HitWire(Wiring._wireList, 3);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.YellowWire)
                    {
                        Wiring._wireList.PushBack(back);
                    }
                }
            }
            if (Wiring._wireList.Count > 0)
            {
                HitWire(Wiring._wireList, 4);
            }
            Wiring.running = false;
            bufferGatePass();
        }

        private static void HitWire(DoubleStack<Point16> next, int wireType)
        {
            Wiring._wireDirectionList.Clear(quickClear: true);
            for (int i = 0; i < next.Count; i++)
            {
                Point16 point = next.PopFront();
                _wireSkip[point] = true;
                Wiring._toProcess.Add(point, 4);
                next.PushBack(point);
                Wiring._wireDirectionList.PushBack(0);
            }
            Wiring._currentWireColor = wireType;
            while (next.Count > 0)
            {
                Point16 key = next.PopFront();
                int num = Wiring._wireDirectionList.PopFront();
                int x = key.X;
                int y = key.Y;
                HitWireSingle(key);
                for (int j = 0; j < 4; j++)
                {
                    int num2;
                    int num3;
                    switch (j)
                    {
                        case 0:
                            num2 = x;
                            num3 = y + 1;
                            break;
                        case 1:
                            num2 = x;
                            num3 = y - 1;
                            break;
                        case 2:
                            num2 = x + 1;
                            num3 = y;
                            break;
                        case 3:
                            num2 = x - 1;
                            num3 = y;
                            break;
                        default:
                            num2 = x;
                            num3 = y + 1;
                            break;
                    }
                    if (num2 < 2 || num2 >= Main.maxTilesX - 2 || num3 < 2 || num3 >= Main.maxTilesY - 2)
                    {
                        continue;
                    }
                    Tile tile = Main.tile[num2, num3];
                    if (tile == null)
                    {
                        continue;
                    }
                    Tile tile2 = Main.tile[x, y];
                    if (tile2 == null)
                    {
                        continue;
                    }
                    byte b = 3;
                    if (tile.TileType == TileID.WirePipe)
                    {
                        b = 0;
                    }
                    if (tile2.TileType == TileID.WirePipe)
                    {
                        switch (tile2.TileFrameX / 18)
                        {
                            case 0:
                                if (j != num)
                                {
                                    continue;
                                }
                                break;
                            case 1:
                                if ((num != 0 || j != 3) && (num != 3 || j != 0) && (num != 1 || j != 2) && (num != 2 || j != 1))
                                {
                                    continue;
                                }
                                break;
                            case 2:
                                if ((num != 0 || j != 2) && (num != 2 || j != 0) && (num != 1 || j != 3) && (num != 3 || j != 1))
                                {
                                    continue;
                                }
                                break;
                        }
                    }
                    if (wireType switch
                    {
                        1 => tile.RedWire ? 1 : 0,
                        2 => tile.GreenWire ? 1 : 0,
                        3 => tile.BlueWire ? 1 : 0,
                        4 => tile.YellowWire ? 1 : 0,
                        _ => 0,
                    } == 0)
                    {
                        continue;
                    }
                    Point16 point2 = new Point16(num2, num3);
                    if (Wiring._toProcess.TryGetValue(point2, out var value))
                    {
                        value--;
                        if (value == 0)
                        {
                            Wiring._toProcess.Remove(point2);
                        }
                        else
                        {
                            Wiring._toProcess[point2] = value;
                        }
                        continue;
                    }
                    next.PushBack(point2);
                    Wiring._wireDirectionList.PushBack((byte)j);
                    if (b > 0)
                    {
                        Wiring._toProcess.Add(point2, b);
                    }
                }
            }
            _wireSkip.Clear();
            Wiring._toProcess.Clear();

        }

        private static void HitWireSingle(Point16 key)
        {
            if (MagitronicsWorld.modedActuators.Contains(key))
            {
                TERedirector.registerTerminal(key);
            }
            Tile tile = Main.tile[key.X, key.Y];
            if (!tile.HasTile) return;
            if(tile.TileType == ModContent.TileType<Tiles.LogicBuffer>())
            {
                BuffersToCheck.Enqueue(key);
            }
        }
        private static void bufferGatePass()
        {
            if(BuffersCurrent.Count > 0)
            {
                return;
            }
            BuffersDone.Clear();
            while (BuffersToCheck.Count > 0)
            {
                Utils.Swap(ref BuffersCurrent, ref BuffersToCheck);
                while (BuffersCurrent.Count > 0)
                {
                    Point16 point = BuffersCurrent.Peek();
                    if(BuffersDone.TryGetValue(point, out bool b) && b)
                    {
                        BuffersCurrent.Dequeue();
                        continue;
                    }
                    BuffersDone.Add(point, true);
                    Tile tileDown = Main.tile[point.X, point.Y + 1];
                    if (tileDown.HasTile && tileDown.TileType == TileID.LogicGate)
                    {
                        int gateType = tileDown.TileFrameY / 18;
                        if (MagicWiring.SatisfiesGate(point.X, point.Y - 1, gateType))
                        {
                            tileDown.TileFrameX = 18;
                            TripWire(point.X, point.Y + 1, 1, 1);
                        }
                        else
                        {
                            tileDown.TileFrameX = 0;
                        }
                    }
                    BuffersCurrent.Dequeue();
                    
                }
            }
        }
    }

    


}
