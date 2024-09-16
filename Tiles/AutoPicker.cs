using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;
using Terraria.DataStructures;

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

        public override bool RightClick(int x, int y)
        {
            TEAutoPicker ap = FindByGuessing(x, y);
            if (ap != null)
            {
                Main.LocalPlayer.tileEntityAnchor.Set(ap.ID, x, y);
                ModContent.GetInstance<UISystem>().ToggleUI(ap);
                return true;
            }
            return false;
        }

        public static TEAutoPicker FindByGuessing(int x, int y)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(x - i, y - j), out var value) && value is TEAutoPicker)
                    {
                        return (TEAutoPicker)value;
                    }
                }
            }
            return null;
        }
    }
}
