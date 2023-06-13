using Terraria;
using Terraria.ID;
using System.Collections.Generic;


namespace MagiTronics
{
    public class MagicWiring
    {
        internal class ChestLocation
        {
            public int x;
            public int y;
            public int id;

            public ChestLocation(int x, int y, int id) {
                this.x = x; this.y = y; this.id = id;
            }
        }

        public static int[] _chestOutPumpX = new int[400];
        public static int[] _chestOutPumpY = new int[400];
        public static int _numChestOutPump = 0;

        public static int[] _chestInPumpX = new int[400];
        public static int[] _chestInPumpY = new int[400];
        public static int _numChestInPump = 0;

        static List<ChestLocation> wiredChests = new List<ChestLocation>();

        public static void HitwireChest(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int type = (int)tile.TileType;
            switch(type)
            {
                case TileID.Containers:
                case TileID.Containers2:
                case TileID.FakeContainers:
                case TileID.FakeContainers2:
                case TileID.Dressers:
                    wiredChests.Add(new ChestLocation(i, j, type));
                    break;
                    
            }
        }

        public static void XferWater()
        {
            Wiring._numOutPump -= _numChestOutPump;
            for (int i = 0; i < _numChestOutPump && i + _numChestOutPump < 20; i++)
            {
                Wiring._outPumpX[i] = Wiring._outPumpX[i + _numChestOutPump];
                Wiring._outPumpY[i] = Wiring._outPumpY[i + _numChestOutPump];
            }
            Wiring._numInPump -= _numChestInPump;
            for (int i = 0; i < _numChestInPump && i + _numChestInPump < 20; i++)
            {
                Wiring._inPumpX[i] = Wiring._inPumpX[i + _numChestInPump];
                Wiring._inPumpY[i] = Wiring._inPumpY[i + _numChestInPump];
            }


            LiquidToChests();
            LiquidFromChests();

            _numChestInPump = 0;
            _numChestOutPump = 0;
            wiredChests.Clear();
        }

        private static void LiquidToChests()
        {
            for (int i = 0; i < _numChestOutPump; i++)
            {
                Chest chest = FindWiredChest();
                if (chest != null)
                {
                    Item[] items = chest.item;
                    ChestPumpInventory inv = new ChestPumpInventory(items);
                    for (int inpumpindex = 0; inpumpindex < Wiring._numInPump; inpumpindex++)
                    {
                        int inPumpX = Wiring._inPumpX[inpumpindex];
                        int inPumpY = Wiring._inPumpY[inpumpindex];
                        Tile tile = Main.tile[inPumpX, inPumpY];
                        if (tile.LiquidAmount <= 0)
                        {
                            continue;
                        }
                        bool success = inv.WaterIn(tile.LiquidType);
                        if (success)
                        {
                            tile.LiquidAmount = 0;
                            WorldGen.SquareTileFrame(inPumpX, inPumpY);
                        }
                    }
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {

                } 
            }
        }

        private static void LiquidFromChests()
        {
            for(int i = 0; i < _numChestInPump; i++)
            {
                Chest chest = FindWiredChest();
                if (chest != null)
                {
                    Item[] items = chest.item;
                    ChestPumpInventory inv = new ChestPumpInventory(items);
                    for (int outPumpIndex = 0; outPumpIndex < Wiring._numOutPump; outPumpIndex++)
                    {
                        int outPumpX = Wiring._outPumpX[outPumpIndex];
                        int outPumpY = Wiring._outPumpY[outPumpIndex];
                        Tile tile = Main.tile[outPumpX, outPumpY];
                        if (tile.LiquidAmount >= 255)
                        {
                            continue;
                        }
                        bool success = inv.WaterOut(out int liquidType);
                        if (success)
                        {
                            tile.LiquidType = liquidType;
                            tile.LiquidAmount = 255;
                            WorldGen.SquareTileFrame(outPumpX, outPumpY);
                        }
                    }
                }
            }
        }

        private static Chest FindWiredChest()
        {
            foreach(ChestLocation cl in wiredChests)
            {
                int chestIndex = PatchedFindChestByGuessing(cl);
                if (chestIndex != -1)
                    return Main.chest[chestIndex];
            }
            return null;
        }

        private static int PatchedFindChestByGuessing(ChestLocation cl)
        {
            int length = 2;
            int height = 2;
            if (cl.id == TileID.Dressers) length = 3;
            for (int i = 0; i < 8000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x > cl.x - length && Main.chest[i].x <= cl.x && Main.chest[i].y > cl.y - height && Main.chest[i].y <= cl.y)
                {
                    return i;
                }
            }
            
            return -1;
        }



    }
}
