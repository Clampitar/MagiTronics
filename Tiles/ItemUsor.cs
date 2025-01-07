using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
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
            RegisterItemDrop(ModContent.ItemType<Items.ItemUsor>());

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = new[] {16, 16, 16};
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TEItemUsor>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<UISystem>().KilledUsor(new Point16(i, j));
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
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket modPacket = Mod.GetPacket();
                modPacket.Write((byte)MagiTronics.PacketId.USORITEMUSE);
                modPacket.Write(redirector.Position.X);
                modPacket.Write(redirector.Position.Y);
                for (int playerIndex = 0; playerIndex < 256; playerIndex++)
                {
                    if (Netplay.Clients[playerIndex].IsConnected())
                    {
                        modPacket.Send(toClient: playerIndex);
                        break;
                    }
                }
            }
            else
            {
                redirector.Player.controlUseItem = true;
            }
            Wiring.SkipWire(x, y);
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
            TEItemUsor iu = FindByGuessing(x, y);
            if (iu != null)
            {
                Main.LocalPlayer.tileEntityAnchor.Set(iu.ID, x, y);
                ModContent.GetInstance<UISystem>().ToggleUI(iu);
                return true;
            }

            return false;
        }
    }
}
