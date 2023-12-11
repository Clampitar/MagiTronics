using Terraria;
using Terraria.DataStructures;
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TERedirector>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TERedirector>().Kill(i, j);
        }

        public override void HitWire(int x, int y)
        {
            
            TERedirector redirector = FindByGuessing(x, y);
            if (redirector is null)
                return;
            Point16 target = redirector.Target(redirector.Position.X != x, redirector.Position.Y != y);
            if (target != Point16.NegativeOne)
            {
                UseItem(target, redirector.Player);
            }
            Wiring.SkipWire(x, y);
        }

        private void UseItem(Point16 point, Player player)
        {
            Player.tileTargetX = point.X;
            Player.tileTargetY = point.Y;
            player.ItemCheck();
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

        public static TERedirector FindByGuessing(int x, int y)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(x - i, y - j), out var value) && value is TERedirector)
                    {
                        return (TERedirector)value;
                    }
                }
            }
            return null;
        }
    }
}
