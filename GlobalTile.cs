using Terraria.ID;
using Terraria;
using Terraria.DataStructures;
using MagiTronics.Tiles;

namespace MagiTronics
{
    internal class GlobalTile : Terraria.ModLoader.GlobalTile
    {
        public override void HitWire(int i, int j, int type)
        {
            MagicWiring.HitwireChest(i, j);
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if(type == TileID.LogicGateLamp)
            {
                return MagicWiring.shouldKillLamp(i, j);
            }
            return true;
        }
    }
}
