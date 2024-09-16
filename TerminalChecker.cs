using MagiTronics.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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
        private static Dictionary<Point16, byte> _toProcess;
        private static bool running;
        private static DoubleStack<byte> _wireDirectionList;
        private static DoubleStack<Point16> _wireList;
        private static int _currentWireColor;

        public static void initialize()
        {
            BuffersToCheck = new Queue<Point16>();
            BuffersCurrent = new Queue<Point16>();
            BuffersDone = new Dictionary<Point16, bool>();
            _toProcess = new Dictionary<Point16, byte>();
            _wireDirectionList = new DoubleStack<byte>();
            _wireList = new DoubleStack<Point16>();
        }
        public static List<Point16> TripWire(int left, int top, int width, int height)
        {
            List<Point16> wiredTerminals = [];
            _wireSkip = new Dictionary<Point16, bool>();
            int netmode = Main.netMode;
            running = true;
            if (_wireList.Count != 0)
            {
                _wireList.Clear(quickClear: true);
            }
            if (_wireDirectionList.Count != 0)
            {
                _wireDirectionList.Clear(quickClear: true);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.RedWire)
                    {
                        _wireList.PushBack(back);
                    }
                }
            }
            if (_wireList.Count > 0)
            {
                HitWire(_wireList, 1,ref wiredTerminals);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.GreenWire)
                    {
                        _wireList.PushBack(back);
                    }
                }
            }
            if (_wireList.Count > 0)
            {
                HitWire(_wireList, 2,ref wiredTerminals);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.BlueWire)
                    {
                        _wireList.PushBack(back);
                    }
                }
            }
            if (_wireList.Count > 0)
            {
                HitWire(_wireList, 3,ref wiredTerminals);
            }
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.YellowWire)
                    {
                        _wireList.PushBack(back);
                    }
                }
            }
            if (_wireList.Count > 0)
            {
                HitWire(_wireList, 4,ref wiredTerminals);
            }
            running = false;
            bufferGatePass(ref wiredTerminals);
            return wiredTerminals;
        }

        private static void HitWire(DoubleStack<Point16> next, int wireType,ref List<Point16> wiredTerminals)
        {
            //List<Point16> wiredTerminals = [];
            _wireDirectionList.Clear(quickClear: true);
            for (int i = 0; i < next.Count; i++)
            {
                Point16 point = next.PopFront();
                _wireSkip[point] = true;
                _toProcess.Add(point, 4);
                next.PushBack(point);
                _wireDirectionList.PushBack(0);
            }
            Wiring._currentWireColor = wireType;
            while (next.Count > 0)
            {
                Point16 key = next.PopFront();
                int num = _wireDirectionList.PopFront();
                int x = key.X;
                int y = key.Y;
                HitWireSingle(key, ref wiredTerminals);
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
                    if (_toProcess.TryGetValue(point2, out var value))
                    {
                        value--;
                        if (value == 0)
                        {
                            _toProcess.Remove(point2);
                        }
                        else
                        {
                            _toProcess[point2] = value;
                        }
                        continue;
                    }
                    next.PushBack(point2);
                    _wireDirectionList.PushBack((byte)j);
                    if (b > 0)
                    {
                        _toProcess.Add(point2, b);
                    }
                }
            }
            _wireSkip.Clear();
            _toProcess.Clear();
        }

        private static void HitWireSingle(Point16 key, ref List<Point16> wiredTerminals)
        {
            if (TerminalSystem.modedActuators.Contains(key))
            {
                wiredTerminals.Add(key);
            }
            Tile tile = Main.tile[key.X, key.Y];
            if (!tile.HasTile) return;
            if(tile.TileType == ModContent.TileType<Tiles.LogicBuffer>())
            {
                BuffersToCheck.Enqueue(key);
            }
        }
        private static void bufferGatePass(ref List<Point16> wiredTerminals)
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
                            wiredTerminals.AddRange(TripWire(point.X, point.Y + 1, 1, 1));
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
