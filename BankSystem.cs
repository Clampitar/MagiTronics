using MagiTronics.Tiles;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MagiTronics
{
    internal class BankSystem : ModSystem
    {
        public Chest PiggyBank = new Chest(bank: true);

        public Chest Safe = new Chest(bank: true);

        public Chest VoidVault = new Chest(bank: true);

        public Chest DefendersForge = new Chest(bank: true);

        public TEItemUsor currentIU;

        public enum BankType
        {
            PiggyBank = 1,
            Safe = 2,
            VoidVault = 3,
            DefendersForge = 4,
            ItemUsor = 5
        }

        public BankSystem()
        {
            for (int i = 0; i < 40; i++)
            {
                PiggyBank.item[i] = new Item();
                Safe.item[i] = new Item();
                VoidVault.item[i] = new Item();
                DefendersForge.item[i] = new Item();
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("piggyBank"))
            {
                PiggyBank.item = [.. tag.GetList<Item>("piggyBank")];
            }
            if (tag.ContainsKey("safe"))
            {
                Safe.item = [.. tag.GetList<Item>("safe")];
            }
            if (tag.ContainsKey("voidVault"))
            {
                VoidVault.item = [.. tag.GetList<Item>("voidVault")];
            }
            if (tag.ContainsKey("defendersForge"))
            {
                DefendersForge.item = [.. tag.GetList<Item>("defendersForge")];
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("piggyBank", PiggyBank.item.ToList());
            tag.Set("safe", Safe.item.ToList());
            tag.Set("voidVault", VoidVault.item.ToList());
            tag.Set("defendersForge", DefendersForge.item.ToList());
        }

        public override void NetReceive(BinaryReader reader)
        {
            foreach (Item item in PiggyBank.item)
                ItemIO.Receive(item, reader, readStack: true);
            foreach (Item item in Safe.item)
                ItemIO.Receive(item, reader, readStack: true);
            foreach (Item item in VoidVault.item)
                ItemIO.Receive(item, reader, readStack: true);
            foreach (Item item in DefendersForge.item)
                ItemIO.Receive(item, reader, readStack: true);

        }

        override public void NetSend(BinaryWriter writer)
        {
            foreach (Item item in PiggyBank.item)
                ItemIO.Send(item, writer, writeStack: true);
            foreach (Item item in Safe.item)
                ItemIO.Send(item, writer, writeStack: true);
            foreach (Item item in VoidVault.item)
                ItemIO.Send(item, writer, writeStack: true);
            foreach (Item item in DefendersForge.item)
                ItemIO.Send(item, writer, writeStack: true);
        }

        public void SyncedItem(BinaryReader reader)
        {
            BankType msgType = (BankType)reader.ReadByte();
            int index = reader.ReadInt32();
            switch (msgType)
            {
                case BankType.PiggyBank:
                    ItemIO.Receive(PiggyBank.item[index], reader, readStack: true);
                    break;
                case BankType.Safe:
                    ItemIO.Receive(Safe.item[index], reader, readStack: true);
                    break;
                case BankType.VoidVault:
                    ItemIO.Receive(VoidVault.item[index], reader, readStack: true);
                    break;
                case BankType.DefendersForge:
                    ItemIO.Receive(DefendersForge.item[index], reader, readStack: true);
                    break;
                case BankType.ItemUsor:
                    break;
            }
            if (Main.netMode == NetmodeID.Server)
            {
                SyncItem(msgType, index);
            }
        }

        public void SyncItem(BankType type, int index)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return;
            }
            ModPacket modPacket = Mod.GetPacket();
            modPacket.Write((byte)MagiTronics.PacketId.BANKSYNC);
            modPacket.Write((byte)type);
            modPacket.Write(index);
            switch(type)
            {
                case BankType.PiggyBank:
                    ItemIO.Send(PiggyBank.item[index], modPacket, writeStack: true);
                    break;
                case BankType.Safe:
                    ItemIO.Send(Safe.item[index], modPacket, writeStack: true);
                    break;
                case BankType.VoidVault:
                    ItemIO.Send(VoidVault.item[index], modPacket, writeStack: true);
                    break;
                case BankType.DefendersForge:
                    ItemIO.Send(DefendersForge.item[index], modPacket, writeStack: true);
                    break;
            }
            modPacket.Send();
        }

        public void MaybeSyncItem(BankType type)
        {

        }
    }

}
