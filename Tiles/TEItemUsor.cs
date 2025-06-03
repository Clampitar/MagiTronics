
using MagiTronics.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace MagiTronics.Tiles
{
    internal class TEItemUsor : TERedirector
    {
        protected Player usorPlayer;

        public TEItemUsor()
        {
            usorPlayer = new Player();
            usorPlayer.selectedItem = 0;
            usorPlayer.releaseUseItem = true;
        }

        public Point16 Target()
        {
            return Target(false, false);
        }

        public override void SaveData(TagCompound tag)
        {
            List<Item> inv = usorPlayer.inventory.ToList();
            tag.Set("inv", inv);
            Vector2 pos = usorPlayer.position;
            tag.Set("pos", pos);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("inv"))
            {
                IList<Item> list = tag.GetList<Item>("inv");
                usorPlayer.inventory = [.. list];
            }
            if (tag.ContainsKey("pos"))
            {
                usorPlayer.position = tag.Get<Vector2>("pos");
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            Item[] items = usorPlayer.inventory;
            foreach (Item item in items)
            {
                ItemIO.Send(item, writer, writeStack: true);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            Item[] items = usorPlayer.inventory;
            foreach (Item item in items)
            {
                ItemIO.Receive(item, reader, readStack: true);
            }
        }

        private void SyncItem(int index)
        {
            if(Main.netMode == NetmodeID.SinglePlayer)
            {
                return;
            }
            ModPacket modPacket = Mod.GetPacket();
            modPacket.Write((byte)MagiTronics.PacketId.USORITEM);
            modPacket.Write(Position.X);
            modPacket.Write(Position.Y);
            modPacket.Write(index);
            Item item = usorPlayer.inventory[index];
            ItemIO.Send(item, modPacket, writeStack: true);
            modPacket.Send();
        }

        public void SyncedItem(BinaryReader reader)
        {
            int index = reader.ReadInt32();
            Item item = usorPlayer.inventory[index];
            ItemIO.Receive(item, reader, readStack: true);
            if(Main.netMode == NetmodeID.Server)
            {
                SyncItem(index);
            }
        }
        public override bool OverrideItemSlotLeftClick(Item[] inv, int context = 0, int slot = 0)
        {
            if (Main.cursorOverride == CursorOverrideID.InventoryToChest)
            {
                TryPlacingInPlayer(inv[slot], usorPlayer.inventory);
                return true;
            }
            if (context != ItemSlot.Context.ChestItem) return false;
            if (Main.cursorOverride == CursorOverrideID.FavoriteStar) return false;
            if (OverrideSellOrTrash(inv, context, slot)) return true;

            if(Main.cursorOverride == CursorOverrideID.ChestToInventory)
            {
                TryPlacingInPlayer(inv[slot], Main.LocalPlayer.inventory);
                SyncItem(slot);
                return true;
            }
            if (Main.mouseItem.maxStack <= 1 || inv[slot].type != Main.mouseItem.type || inv[slot].stack == inv[slot].maxStack || Main.mouseItem.stack == Main.mouseItem.maxStack)
            {
                Utils.Swap(ref inv[slot], ref Main.mouseItem);
            }
            if (inv[slot].stack > 0)
            {
                ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(inv[slot], 21, context, inv[slot].stack));
            }
            else
            {
                ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(Main.mouseItem, context, 21, Main.mouseItem.stack));
            }
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

            if (Main.mouseItem.netID == inv[slot].netID && Main.mouseItem.type == inv[slot].type && inv[slot].stack != inv[slot].maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack && ItemLoader.TryStackItems(inv[slot], Main.mouseItem, out var numTransfered))
            {
                ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(inv[slot], 21, context, numTransfered));
            }
            if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
            {
                Main.mouseItem = new Item();
            }
            if (Main.mouseItem.type > 0 || inv[slot].type > 0)
            {
                Recipe.FindRecipes();
                SoundEngine.PlaySound(SoundID.Grab);
            }
            SyncItem(slot);
            return true;
        }

        public void OverrideRightClick(int slot)
        {
            Item[] inv = usorPlayer.inventory;
            ItemSlot.RightClick(inv, ItemSlot.Context.InventoryItem, slot);
            if (Main.mouseRightRelease || Main.mouseRight)
            {
                SyncItem(slot);
            }
        }

        private bool OverrideSellOrTrash(Item[] inv, int context, int slot)
        {
            bool result = false;
            if (ItemSlot.NotUsingGamepad && ItemSlot.Options.DisableLeftShiftTrashCan)
            {
                if (!ItemSlot.Options.DisableQuickTrash)
                {
                    if (ItemSlot.ControlInUse)
                    {
                        ItemSlot.SellOrTrash(inv, 0, slot);
                        SyncItem(slot);
                        result = true;
                    }
                }
            }
            else
            {
                if (ItemSlot.ShiftInUse && (!ItemSlot.NotUsingGamepad || !ItemSlot.Options.DisableQuickTrash))
                {
                    ItemSlot.SellOrTrash(inv, context, slot);
                    result = true;
                }
            }
            return result;
        }

        private void TryPlacingInPlayer(Item item, Item[] inv)
        {
            if (item.maxStack > 1)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (inv[i].stack >= inv[i].maxStack || item.netID != inv[i].netID || item.type != inv[i].type || !ItemLoader.CanStack(inv[i], item))
                    {
                        continue;
                    }
                    ItemLoader.StackItems(inv[i], item, out var _);
                    SoundEngine.PlaySound(SoundID.Grab);
                    SyncItem(i);
                    if (item.stack <= 0)
                    {
                        item.SetDefaults();
                        break;
                    }
                    if (inv[i].type == 0)
                    {
                        inv[i] = item.Clone();
                        item.SetDefaults();
                    }
                }
            }
            if (item.stack > 0)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (inv[j].stack != 0)
                    {
                        continue;
                    }
                    SoundEngine.PlaySound(SoundID.Grab);
                    inv[j] = item.Clone();
                    item.SetDefaults();
                    ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(inv[j], 0, 3));
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        SyncItem(j);
                    }
                    break;
                }
            }
        }


        public override bool OverrideItemSlotHover(Item[] inv, int context = 0, int slot = 0)
        {
            if (context == 0)
            {
                if (ItemSlot.ShiftInUse && !inv[slot].favorited)
                {
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;
                    return true;
                }
            }
            else if(context == 3)
            {
                if (ItemSlot.ShiftInUse && !inv[slot].favorited)
                {
                    Main.cursorOverride = CursorOverrideID.ChestToInventory;
                    return true;
                }
                if (ItemSlot.ControlInUse)
                {
                    Main.cursorOverride = CursorOverrideID.TrashCan;
                    return true;
                }
                if (Main.cursorOverride == CursorOverrideID.FavoriteStar)
                {
                    Main.cursorOverride = CursorOverrideID.DefaultCursor;
                    return true;
                }
            }
            
            return false;
        }
        public override void Update()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Use();
            }
        }

        public void Use()
        {
            Point16 target = Target();

            if (target != Point16.NegativeOne)
            {
                int prevX = Player.tileTargetX;
                int prevY = Player.tileTargetY;
                Player.tileTargetX = target.X;
                Player.tileTargetY = target.Y;
                Player.tileRangeX += Main.maxTilesX;
                Player.tileRangeY += Main.maxTilesY;
                Player.ItemCheck();
                Player.tileRangeX -= Main.maxTilesX;
                Player.tileRangeY -= Main.maxTilesY;
                Player.tileTargetX = prevX;
                Player.tileTargetY = prevY;
            }

            Player.controlUseItem = false;
        }
        override protected void UpdateTarget(bool right, bool down)
        {
            wiredTerminals = TerminalChecker.TripWire(Position.X, Position.Y+1, 2, 3);
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

        public override void OnKill()
        {
            foreach(Item item in usorPlayer.inventory)
            {
                if (!item.IsAir)
                {
                    usorPlayer.QuickSpawnItem(new EntitySource_Death(usorPlayer), item, item.stack);
                }
            }
        }

        public void RightClick()
        {
            Main.LocalPlayer.tileEntityAnchor.Set(ID, Position.X, Position.Y);
            ModContent.GetInstance<BankSystem>().currentIU = this;
            ModContent.GetInstance<UISystem>().ToggleUI(usorPlayer.inventory, Position, BankSystem.BankType.ItemUsor);
        }
        public override Player Player => usorPlayer;
    }
}
