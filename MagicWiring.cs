using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Microsoft.Xna.Framework;

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
        }

        public static void DrawMagicWire(){
            foreach (Point16 point in MagitronicsWorld.modedActuators)
                Main.spriteBatch.Draw(
                MagitronicsWorld.texture,                                                                       //a texture2d
                new Vector2(point.X * 16 - (int)Main.screenPosition.X, point.Y * 16 - (int)Main.screenPosition.Y),      //a vector2
                new Rectangle(0, 0, 16, 16),              //a rectangle
     new Color(255, 255, 255),                                                                                  //a color
     0f,                                                                                                //a float
     default,                                                                                  // a vector2
     1f,
     SpriteEffects.None,
     0f);
        }

        public static void XferWater()
        {
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
