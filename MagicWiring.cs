using Terraria;
using Terraria.ID;
using System.Collections.Generic;


namespace MagiTronics
{
    public class MagicWiring
    {
        

        public static int[] _chestOutPumpX = new int[400];
        public static int[] _chestOutPumpY = new int[400];
        public static int _numChestOutPump = 0;

        public static int[] _chestInPumpX = new int[400];
        public static int[] _chestInPumpY = new int[400];
        public static int _numChestInPump = 0;

        private static ChestManager chestManager = new ChestManager();

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
                    chestManager.AddChest(i, j, type);
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
            chestManager = new ChestManager();
        }

        private static void LiquidToChests()
        {
            for (int i = 0; i < _numChestOutPump; i++)
            {
                if (!chestManager.Empty())
                {
                    for (int inpumpindex = 0; inpumpindex < Wiring._numInPump; inpumpindex++)
                    {
                        int inPumpX = Wiring._inPumpX[inpumpindex];
                        int inPumpY = Wiring._inPumpY[inpumpindex];
                        Tile tile = Main.tile[inPumpX, inPumpY];
                        if (tile.LiquidAmount <= 0)
                        {
                            continue;
                        }
                        bool success = chestManager.WaterIn(tile.LiquidType);
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
                if (!chestManager.Empty())
                {
                    for (int outPumpIndex = 0; outPumpIndex < Wiring._numOutPump; outPumpIndex++)
                    {
                        int outPumpX = Wiring._outPumpX[outPumpIndex];
                        int outPumpY = Wiring._outPumpY[outPumpIndex];
                        Tile tile = Main.tile[outPumpX, outPumpY];
                        if (tile.LiquidAmount >= 255)
                        {
                            continue;
                        }
                        bool success = chestManager.WaterOut(out int liquidType);
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

        



    }
}
