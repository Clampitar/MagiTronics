using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MagiTronics.Tiles
{
    internal class ItemUsor : ModTile
    {
        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new[] {16, 16, 16};
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TEItemUsor>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TEItemUsor>().Kill(i, j);
        }

        public override void HitWire(int x, int y)
        {
            
            TEItemUsor redirector = FindByGuessing(x, y);
            if (redirector is null)
            {
                Wiring.SkipWire(x, y);
                return;
            }
            redirector.Player.controlUseItem = !redirector.Player.controlUseItem;
            Wiring.SkipWire(x, y);
        }

        public static Point16 Redirect(int x, int y)
        {
            TERedirector mined = FindByGuessing(x, y);
            if (mined is null)
                return Point16.Zero;
            Point16 target = mined.Target(mined.Position.X != x, mined.Position.Y != y);
            if (target != Point16.NegativeOne)
            {
                x = target.X - x;
                y = target.Y - y;
                return new Point16(x, y);
            }
            return Point16.Zero;
        }

        public static TEItemUsor FindByGuessing(int x, int y)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(x - i, y - j), out var value) && value is TEItemUsor)
                    {
                        return (TEItemUsor)value;
                    }
                }
            }
            return null;
        }

        public override bool RightClick(int x, int y)
        {
            ModContent.GetInstance<MagitronicsWorld>().toggleUI();
            return true;
        }
    }
}
