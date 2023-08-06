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

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            TERedirector ut = FindByGuessing(i, j);
            return ut?.CanKill() ?? true;
        }

        public override bool CanPlace(int i, int j)
        {
            return base.CanPlace(i, j);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            fail = false;
            TERedirector ut = FindByGuessing(i, j);
            if (ut is null || ut.target == Point16.NegativeOne)
            {
                return;
            }
            Player player = Main.player[Main.myPlayer];
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                player.PickTile(ut.target.X, ut.target.Y, player.HeldItem.pick);
            }
            fail = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int number = Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i + 16, j + 16), new Vector2(i + 16, j + 16), new Vector2(0, 0), ModContent.GetInstance<Items.Redirector>().Type);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
            }
            ModContent.GetInstance<TERedirector>().Kill(i, j);
        }

        public static void RedirectMiningTools(int x,int y)
        {
            TERedirector mined = FindByGuessing(x, y);
            if (mined is null)
                return;
            Point16 target = mined.target;
            if(target != Point16.NegativeOne)
            {
                x = target.X;
                y = target.Y;
            }
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
