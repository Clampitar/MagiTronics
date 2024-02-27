using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;

namespace MagiTronics.Tiles
{
    internal class AutoPicker : ModTile
    {
        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TEAutoPicker>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TEAutoPicker>().Kill(i, j);
        }

        public override void HitWire(int i, int j)
        {
            TerminalChecker.TripWire(i, j, 1, 1);
        }
    }
}
