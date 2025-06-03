using MagiTronics.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MagiTronics
{
    internal class BankSystem: ModSystem
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
    }
}
