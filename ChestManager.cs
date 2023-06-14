using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace MagiTronics
{
    internal class ChestManager
    {

        internal class ChestLocation
        {
            public int x;
            public int y;
            public int id;

            public ChestLocation(int x, int y, int id)
            {
                this.x = x; this.y = y; this.id = id;
            }
        }

        private bool piggyBank;
        private bool safe;
        private bool voidVault;
        private bool defendersForge;

        private Queue<ChestLocation> unknownChests = new Queue<ChestLocation>();
        private List<ManagableChest> chests = new List<ManagableChest>();

        public void AddChest(int x, int y, int type)
        {
            switch(type)
            {
                case TileID.PiggyBank:
                    piggyBank = true; break;
                case TileID.Safes:
                    safe = true; break;
                case TileID.VoidVault:
                    voidVault = true; break;
                case TileID.DefendersForge:
                    defendersForge = true; break;
                default:
                    unknownChests.Enqueue(new ChestLocation(x, y, type));
                    break;
            }
        }

        public bool Empty() => unknownChests.Count == 0 && chests.Count == 0;


        public bool WaterOut(out int liquidType)
        {
            foreach (var chest in chests)
            {
                if (chest.WaterOut(out liquidType))
                    return true;
            }
            while (DiscoverChest())
            {
                if (chests.Last().WaterOut(out liquidType))
                    return true;
            }
            liquidType = -1;
            return false;
        }
        public bool WaterIn(int liquidType)
        {
            foreach (var chest in chests)
            {
                if (chest.WaterIn(liquidType))
                    return true;
            }
            while (DiscoverChest())
            {
                if (chests.Last().WaterIn(liquidType))
                    return true;
            }
            return false;
        }


        private bool DiscoverChest()
        {
            if(!unknownChests.TryDequeue(out ChestLocation location))
                return false;
            switch (location.id)
            {
                case TileID.Containers:
                case TileID.Containers2:
                case TileID.Dressers:
                    int chestIndex = PatchedFindChestByGuessing(location);
                    ChestPumpInventory chest = new ChestPumpInventory(Main.chest[chestIndex].item);
                    chests.Add(chest);
                    return true;
                    case TileID.ItemFrame:
                    chestIndex = PatchedFindChestByGuessing(location);
                    Main.NewText("item rack id: "+ Main.itemFrameCounter[0]);
                    break;
             }
            return false;
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
