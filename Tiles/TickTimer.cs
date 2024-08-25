using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using System.IO;
using Terraria.ModLoader.IO;

namespace MagiTronics.Tiles
{
    internal class TickTimer : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            RegisterItemDrop(ModContent.ItemType<Items.TickTimer>());

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TETickTimer>().Hook_AfterPlacement, 0, 0, false);
            TileObjectData.addTile(Type);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<TETickTimer>().Kill(i, j);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket modPacket = Mod.GetPacket();
                modPacket.Write((byte)0);
                modPacket.Write(i);
                modPacket.Write(j);
                modPacket.Send();
            }
            else Switch(i, j);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            Switch(i, j);
            Wiring.SkipWire(i, j);
        }

        public static void Switch(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te) && te is TETickTimer timer)
            {
                if (tile.TileFrameY == 0)
                {
                    tile.TileFrameY = 18;
                    timer.Activate();
                }
                else
                {
                    tile.TileFrameY = 0;
                    timer.Deactivate();
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j);
                }
            }
        }
    }
}
