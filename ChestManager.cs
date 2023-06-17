using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
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

        private bool hasPiggyBank;
        private bool hasSafe;
        private bool hasVoidVault;
        private bool hasDefendersForge;
        private ChestPumpInventory piggyBank;
        private ChestPumpInventory safe;
        private ChestPumpInventory voidVault;
        private ChestPumpInventory defendersForge;

        private Queue<ChestLocation> unknownChests = new Queue<ChestLocation>();
        private List<ManagableChest> chests = new List<ManagableChest>();

        public void AddChest(int x, int y, int type)
        {
            switch(type)
            {
                case TileID.PiggyBank:
                    hasPiggyBank = true;
                    break;
                case TileID.Safes:
                    hasSafe = true; break;
                case TileID.VoidVault:
                    hasVoidVault = true; break;
                case TileID.DefendersForge:
                    hasDefendersForge = true; break;
                default:
                    unknownChests.Enqueue(new ChestLocation(x, y, type));
                    break;
            }
        }

        public bool Empty() => unknownChests.Count == 0 && chests.Count == 0;


        public bool WaterOut(out int liquidType)
        {
            ChestPumpInventory bank = playerChest();
            if (bank != null && bank.WaterOut(out liquidType))
                return true;
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
            ChestPumpInventory bank = playerChest();
            if (bank != null && bank.WaterIn(liquidType))
                return true;
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

        private ChestPumpInventory playerChest()
        {
            if(Main.netMode == NetmodeID.SinglePlayer)
            {
                if (hasPiggyBank)
                {
                    if(piggyBank == null)
                        piggyBank = new ChestPumpInventory(Main.player[0].bank.item);
                    return piggyBank;
                }
                if (hasSafe)
                {
                    if (safe == null)
                        safe = new ChestPumpInventory(Main.player[0].bank2.item);
                    return safe;
                }
                if (hasVoidVault)
                {
                    if (voidVault == null)
                        voidVault = new ChestPumpInventory(Main.player[0].bank4.item);
                    return voidVault;
                }
                if (hasDefendersForge)
                {
                    if (defendersForge == null)
                        defendersForge = new ChestPumpInventory(Main.player[0].bank3.item);
                    return defendersForge;
                }
            }
            return null;
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
                    break;
                case TileID.ItemFrame:
                    chestIndex = ItemFrame.FindByGuessing(location.x, location.y);
                    Main.NewText("item rack id: "+ chestIndex);
                    if (chestIndex != -1)
                    {
                        TEItemFrame t = (TEItemFrame)TileEntity.ByID[chestIndex];
                        ItemFrame ir = new ItemFrame(t.item);
                        chests.Add(ir);
                    }
                    break;
             }
            return true;
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
