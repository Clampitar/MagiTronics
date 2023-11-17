using MagiTronics.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace MagiTronics
{
    internal class Wall : GlobalWall
    {

        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            LonelySensor.WallChanged(i, j, false);
            base.PlaceInWorld(i, j, type, item);
        }

        public override void KillWall(int i, int j, int type, ref bool fail)
        {
            LonelySensor.WallChanged(i, j, true);
            base.KillWall(i, j, type, ref fail);
        }
    }
}
