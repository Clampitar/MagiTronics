using Terraria.ID;
using Terraria;

namespace MagiTronics
{
    internal class GlobalTile : Terraria.ModLoader.GlobalTile
    {
        public override void HitWire(int i, int j, int type)
        {
            MagicWiring.HitwireChest(i, j);
        }
    }
}
