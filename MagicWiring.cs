    using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MagiTronics.Tiles;
using System;
using MagiTronics.ChestManagement;

namespace MagiTronics
{
    public class MagicWiring
    {

        public static List<Point16> _chestOutPump = new List<Point16>();
        //public static int _numChestOutPump;

        public static List<Point16> _chestInPump = new List<Point16>();
        //public static int _numChestInPump = 0;

        public static bool addedPump;

        private static ChestManager chestManager = new ChestManager();

        public static Queue<Point16> SignalsToCount = new Queue<Point16>();

        private static List<Point16> SignalsDone = new List<Point16>();
        
        public static Queue<Point16> BuffersHit = new Queue<Point16>();

        private static List<Point16> BuffersDone = new List<Point16>();

        public static Random rand = new Random();


        public static void LogicPass()
        {
            CountSignals();
            foreach (Point16 p in Wiring._LampsToCheck)
            {
                LogicBuffer.UpdatedLamp(p.X, p.Y);
            }
            TripBuffers();
            if(BuffersHit.Count == 0)
            {
                XferWater();    
            }

        }

        private static void CountSignals()
        {
            Queue<Point16> lampsToTrip = new Queue<Point16>();
            foreach (Point16 point in SignalsToCount)
            {
                if (SignalsDone.Contains(point))
                {
                    continue;
                }
                SignalsDone.Add(point);
                for (int lampY = point.Y - 1; lampY > Main.mapMinY; lampY--)
                {
                    Tile tile = Main.tile[point.X, lampY];
                    if (!tile.HasTile || tile.TileType != TileID.LogicGateLamp)
                    {
                        break;
                    }
                    SignalsDone.Add(new Point16(point.X, lampY));
                    lampsToTrip.Enqueue(new Point16(point.X, lampY));
                    if (tile.TileFrameX == 0)
                    {
                        tile.TileFrameX = 18;
                        break;
                    }
                    if (tile.TileFrameX == 18)
                    {
                        tile.TileFrameX = 0;
                    }
                    if (tile.TileFrameX == 36)
                    {
                        if (MagicWiring.rand.Next(0, 2) == 0)
                            break;
                    }
                }
            }
            foreach (Point16 point in lampsToTrip)
            {
                Wiring.TripWire(point.X, point.Y, 1, 1);
            }
            SignalsToCount.Clear();
            SignalsDone.Clear();
        }

        private static void TripBuffers()
        {
            Queue<Point16> GatesToTrip = new();
            foreach(Point16 point in BuffersHit)
            {
                if (BuffersDone.Contains(point))
                {
                    continue;
                }
                BuffersDone.Add(point);
                for(int gateY = point.Y + 1;  gateY < Main.mapMaxY; gateY++)
                {
                    Tile tile = Main.tile[point.X, gateY];
                    if (!tile.HasTile || tile.TileType != TileID.LogicGate)
                    {
                        break;
                    }
                    if (tile.TileFrameX == 18)
                    {
                        GatesToTrip.Enqueue(new Point16(point.X, gateY));
                    }
                }
            }
            foreach (Point16 point in GatesToTrip)
            {
                Wiring.TripWire(point.X, point.Y, 1, 1);
            }
            BuffersHit.Clear();
            BuffersDone.Clear();
        }

        public static bool shouldKillLamp(int x, int y)
        {
            Tile tile = Main.tile[x, y+1];
            if (tile.TileFrameX % 18 != 0)
            {
                return true;
            }
            if (tile.TileFrameY % 18 != 0)
            {
                return true;
            }
            int type = tile.TileType;
            return type != ModContent.TileType<LogicBuffer>()
                && type != ModContent.TileType<TickTimer>()
                && type != ModContent.TileType<SignalCounter>();
        }

        public static bool SatisfiesGate(int x, int y, int gateType)
        {
            Tile tile = Main.tile[x, y];
            if(!tile.HasTile ||  tile.TileType != TileID.LogicGateLamp)
                return false;
            int numLamps = 0;
            int numActiveLamps = 0;
            for (int i = y; i > Main.miniMapY; i--)
            {
                tile = Main.tile[x, i];
                if (!tile.HasTile || tile.TileType != 419)
                {
                    break;
                }
                numLamps++;
                if (tile.TileFrameX == 18)
                {
                    numActiveLamps++;
                }
                if (tile.TileFrameX == 36)
                {
                    numActiveLamps += rand.Next(0, 2);
                }
            }
            switch (gateType)
            {
                case 0:
                    return numLamps == numActiveLamps;
                case 1:
                    return numActiveLamps > 0;
                case 2:
                    return numActiveLamps != numLamps;
                case 3:
                    return numActiveLamps == 0;
                case 4:
                    return numActiveLamps == 1;
                case 5:
                    return numActiveLamps != 1;
            }
            return false;
        }

        public static int CountLamps(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != TileID.LogicGateLamp)
                return -1;
            int num = 0;
            int multiplier = 1;
            for (int i = y; i > Main.miniMapY; i--)
            {
                tile = Main.tile[x, i];
                if (!tile.HasTile || tile.TileType != 419)
                {
                    break;
                }
                if (tile.TileFrameX == 18)
                {
                    num += multiplier;
                }
                if (tile.TileFrameX == 36)
                {
                    num += rand.Next(0, 2) * multiplier;
                }
                multiplier *= 2;
            }
            return num;
        }
        public static void HitwireChest(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int type = (int)tile.TileType;
            switch(type)
            {
                case TileID.Containers:
                case TileID.Containers2:
                case TileID.Dressers:
                case TileID.ItemFrame:
                case TileID.PiggyBank:
                case TileID.Safes:
                case TileID.VoidVault:
                case TileID.DefendersForge:
                    chestManager.AddChest(i, j, type);
                    break;
                    
            }
            if (MagiTronics.magicStorageLoaded)
                chestManager.checkMagicStorage(i, j, type);
        }

        public static void XferWater()
        {

            if (BuffersHit.Count > 0)
            {
                return;
            }
            if (addedPump)
            {
                Wiring._numOutPump--;
                for (int i = 0; i < Wiring._numOutPump; i++)
                {
                    Wiring._outPumpX[i] = Wiring._outPumpX[i + 1];
                    Wiring._outPumpY[i] = Wiring._outPumpY[i + 1];
                }
                Wiring._numInPump--;
                for (int i = 0; i < Wiring._numInPump; i++)
                {
                    Wiring._inPumpX[i] = Wiring._inPumpX[i + 1];
                    Wiring._inPumpY[i] = Wiring._inPumpY[i + 1];
                }
                addedPump = false;
            }
            if(_chestOutPump.Count > 0)
                LiquidToChests();
            if(_chestInPump.Count > 0)
                LiquidFromChests();
            chestManager = new ChestManager();
            _chestOutPump.Clear();
            _chestInPump.Clear();
        }

        public static void newInPump(int x, int y)
        {
            if(Wiring._numInPump < 1)
            {
                Wiring._numInPump = 1;
                addedPump=true;
            }
            if (Wiring._numOutPump < 1)
            {
                Wiring._numOutPump = 1;
                addedPump = true;
            }
            _chestInPump.Add(new Point16(x, y));
        }

        public static void newOutPump(int x, int y)
        {
            if (Wiring._numOutPump < 1)
            {
                Wiring._numOutPump = 1;
                addedPump = true;
            }
            if (Wiring._numInPump < 1)
            {
                Wiring._numInPump = 1;
                addedPump = true;
            }

            _chestOutPump.Add(new Point16(x, y));
        }
        /**
         * Makes liquids move from inlet pumps and chest outlet pumps to chests
         */
        private static void LiquidToChests()
        {

            for (int inpumpindex = 0; inpumpindex < Wiring._numInPump; inpumpindex++)
            {
                int inPumpX = Wiring._inPumpX[inpumpindex];
                int inPumpY = Wiring._inPumpY[inpumpindex];
                LiquidToChestsFrom(inPumpX, inPumpY);
            }
            foreach (Point16 pump in _chestOutPump)
            {
                LiquidToChestsFrom(pump.X, pump.Y);
                LiquidToChestsFrom(pump.X + 1, pump.Y);
                LiquidToChestsFrom(pump.X, pump.Y + 1);
                LiquidToChestsFrom(pump.X + 1, pump.Y + 1);
            }
        }

        private static void LiquidToChestsFrom(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile.LiquidAmount <= 0)
            {
                return;
            }
            bool success = chestManager.WaterIn(tile.LiquidType);
            if (success)
            {
                tile.LiquidAmount = 0;
                WorldGen.SquareTileFrame(x, y);
            }
        }

        private static void LiquidFromChests()
        {
            for (int outPumpIndex = 0; outPumpIndex < Wiring._numOutPump; outPumpIndex++)
            {
                int outPumpX = Wiring._outPumpX[outPumpIndex];
                int outPumpY = Wiring._outPumpY[outPumpIndex];
                LiquidFromChestTo(outPumpX, outPumpY);
            }
            foreach(Point16 pump in _chestInPump) 
            {
                LiquidFromChestTo(pump.X, pump.Y);
                LiquidFromChestTo(pump.X + 1, pump.Y);
                LiquidFromChestTo(pump.X, pump.Y + 1);
                LiquidFromChestTo(pump.X + 1, pump.Y + 1);
            }
        }

        private static void LiquidFromChestTo(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile.LiquidAmount >= 255)
            {
                return;
            }
            bool success = chestManager.WaterOut(out int liquidType);
            if (success)
            {
                tile.LiquidType = liquidType;
                tile.LiquidAmount = 255;
                WorldGen.SquareTileFrame(x, y);
            }
        }



    }
}
