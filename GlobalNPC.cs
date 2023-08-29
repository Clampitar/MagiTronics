using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics {
    public class GlobalNPC : Terraria.ModLoader.GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Mechanic)
            {
                shop.Add(ModContent.ItemType<Items.UsageTerminal>());
            }
        }
    }
}