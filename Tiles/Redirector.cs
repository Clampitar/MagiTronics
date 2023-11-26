using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace MagiTronics.Tiles
{
    internal class Redirector : ModTile
    {

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            //Terraria.ID.TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TERedirector>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TERedirector>().Kill(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            if(Main.netMode == NetmodeID.Server)
            {
                base.MouseOver(i, j);
                return;
            }
            Point16 shift = Redirect(i, j);
            if (shift != Point16.Zero)
            {
                Player.tileTargetX += shift.X;
                Player.tileTargetY += shift.Y;
                Main.LocalPlayer.TileInteractionsCheck(Player.tileTargetX, Player.tileTargetY);
                Player.tileTargetX -= shift.X;
                Player.tileTargetY -= shift.Y;
            }
        }

        public static Point16 Redirect(int x,int y)
        {
            TERedirector mined = FindByGuessing(x, y);
            if (mined is null)
                return Point16.Zero;
            Point16 target = mined.Target(mined.Position.X != x, mined.Position.Y != y);
            if(target != Point16.NegativeOne)
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
                for (int j = 0; j < 2; j++)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(x-i, y-j), out var value) && value is TERedirector)
                    {
                        return (TERedirector)value;
                    }
                }
            }
            return null;
        }
    }
}
