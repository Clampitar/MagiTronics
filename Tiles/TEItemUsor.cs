
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace MagiTronics.Tiles
{
    internal class TEItemUsor : TERedirector
    {
        private Player usorPlayer;

        public TEItemUsor()
        {
            usorPlayer = new Player();
            usorPlayer.inventory[0] = new Item(ItemID.GoldBrick, 5);
            usorPlayer.selectedItem = 0;
            usorPlayer.controlUseItem = true;
            usorPlayer.releaseUseItem = true;
        }

        public Point16 Target()
        {
            return Target(false, false);
        }

        public override bool OverrideItemSlotLeftClick(Item[] inv, int context = 0, int slot = 0)
        {
            if (Main.mouseItem.maxStack <= 1 || inv[slot].type != Main.mouseItem.type || inv[slot].stack == inv[slot].maxStack || Main.mouseItem.stack == Main.mouseItem.maxStack)
            {
                Utils.Swap(ref inv[slot], ref Main.mouseItem);
            }
            /*if (inv[slot].stack > 0)
            {
                ItemSlot.AnnounceTransfer(new ItemTransferInfo(inv[slot], 21, context, inv[slot].stack));
            }
            else
            {
                ItemSlot.AnnounceTransfer(new ItemTransferInfo(Main.mouseItem, context, 21, Main.mouseItem.stack));
            }*/
            if (inv[slot].stack > 0)
            {
                switch (Math.Abs(context))
                {
                    case 0:
                        AchievementsHelper.NotifyItemPickup(Main.LocalPlayer, inv[slot]);
                        break;
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 16:
                    case 17:
                    case 25:
                    case 27:
                    case 33:
                        AchievementsHelper.HandleOnEquip(Main.LocalPlayer, inv[slot], context);
                        break;
                }
            }
            if (inv[slot].type == 0 || inv[slot].stack < 1)
            {
                inv[slot] = new Item();
            }

            /*if (Main.mouseItem.IsTheSameAs(inv[slot]) && inv[slot].stack != inv[slot].maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack && ItemLoader.TryStackItems(inv[slot], Main.mouseItem, out var numTransfered))
            {
                ItemSlot.AnnounceTransfer(new ItemTransferInfo(inv[slot], 21, context, numTransfered));
            }*/
            if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
            {
                Main.mouseItem = new Item();
            }
            if (Main.mouseItem.type > 0 || inv[slot].type > 0)
            {
                Recipe.FindRecipes();
                SoundEngine.PlaySound(SoundID.Grab);
            }

            return true;
        }

        public override void Update()
        {
            Point16 target = Target();
            int prevX = Player.tileTargetX;
            int prevY = Player.tileTargetY;
            if (target != Point16.NegativeOne)
            {
                Player.tileTargetX = target.X;
                Player.tileTargetY = target.Y;
            } else
            {
                Player.controlUseItem = false;
            }
            Player.ItemCheck();

            Player.tileTargetX = prevX;
            Player.tileTargetY = prevY;
            Player.controlUseItem = false;
        }
        override protected void UpdateTarget(bool right, bool down)
        {
            workingTE = this;
            wiredTerminals.Clear();
            _ = ID;
            TerminalChecker.TripWire(Position.X, Position.Y+1, 2, 3);
            _ = usorPlayer.position;
            workingTE = null;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == ModContent.TileType<ItemUsor>());
        }

        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            int id = base.Hook_AfterPlacement(x, y, type, style, direction, alternate);
            TEItemUsor iu = (TEItemUsor)TileEntity.ByID[id];
            iu.usorPlayer.position = new Microsoft.Xna.Framework.Vector2(iu.Position.X, iu.Position.Y)*16;
            return id;
        }

        public override Player Player => usorPlayer;
    }
}
