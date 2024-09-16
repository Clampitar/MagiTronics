
using MagiTronics.Items;
using MagiTronics.Tiles;
using MagiTronics.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace MagiTronics
{
    [Autoload(Side = ModSide.Client)]
    internal class UISystem : ModSystem
    {
        internal AutoUsorUI MenuBar;
        private UserInterface _menuBar;

        private PickerUI PickerUI;
        private UserInterface _pickerInterface;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                MenuBar = new AutoUsorUI();
                MenuBar.Activate();
                _menuBar = new UserInterface();
                PickerUI = new PickerUI();
                PickerUI.Activate();
                _pickerInterface = new UserInterface();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _menuBar?.Update(gameTime);
            _pickerInterface?.Update(gameTime);
        }

        public void ToggleUI(TEItemUsor usor)
        {
            if (_menuBar != null)
            {
                if (_menuBar.CurrentState != null)
                {
                    if (MenuBar.Usor == usor)
                    {
                        CloseUsorInventory();
                    }
                    else
                    {
                        OpenUI(usor, true);
                    }
                }
                else
                {
                    OpenUI(usor, false);
                }
            }
        }

        public void ToggleUI(TEAutoPicker autoPicker)
        {
            if (_pickerInterface != null)
            {
                if (_pickerInterface.CurrentState != null)
                {
                    if (PickerUI.AutoPicker == autoPicker)
                    {
                        CloseAutoPickerInterface();
                    }
                    else
                    {
                        OpenUI(autoPicker);
                    }
                }
                else
                {
                    OpenUI(autoPicker);
                }
            }
        }

        private void OpenUI(TEItemUsor usor, bool hadInvOpen)
        {
            Player player = Main.LocalPlayer;
            MenuBar.Usor = usor;
            _menuBar.SetState(MenuBar);
            //credits to blueshiemagic's Magic Storage for the following
            Main.mouseRightRelease = false;
            if (player.sign > -1)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = string.Empty;
            }

            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }

            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }

            if (player.talkNPC > -1)
            {
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = string.Empty;
            }
            bool hadchestOpen = player.chest != -1;
            player.chest = -1;
            player.chestX = (int)(usor.Position.X / 16);
            player.chestY = (int)(usor.Position.Y / 16);
            Main.playerInventory = true;
            SoundEngine.PlaySound(hadchestOpen || hadInvOpen ? SoundID.MenuTick : SoundID.MenuOpen);
        }

        private void OpenUI(TEAutoPicker picker)
        {
            PickerUI.AutoPicker = picker;
            _pickerInterface.SetState(PickerUI);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public void KilledUsor(TEItemUsor usor)
        {
            if (MenuBar.Usor == usor)
            {
                CloseUsorInventory();
            }
        }

        public void checkValidUI(Player player)
        {
            if (_menuBar.CurrentState != null)
            {
                if (player.chest != -1)
                {
                    CloseUsorInventory();
                    return;
                }
                int playerX = (int)(((double)player.position.X + (double)player.width * 0.5) / 16.0);
                int playerY = (int)(((double)player.position.Y + (double)player.height * 0.5) / 16.0);
                Vector2 pos = MenuBar.Usor.Position.ToVector2() * 16;
                Rectangle rect = new Rectangle((int)(pos.X), (int)(pos.Y), 32, 32);//temp
                rect.Inflate(-1, -1);
                Point point = rect.ClosestPointInRect(player.Center).ToTileCoordinates();
                int chestPointX = point.X;
                int chestPointY = point.Y;
                if (playerX < chestPointX - Player.tileRangeX || playerX > chestPointX + Player.tileRangeX + 1 || playerY < chestPointY - Player.tileRangeY || playerY > chestPointY + Player.tileRangeY)
                {
                    CloseUsorInventory();
                }
            }
            if (_pickerInterface.CurrentState != null)
            {
                int playerX = (int)(((double)player.position.X + (double)player.width * 0.5) / 16.0);
                int playerY = (int)(((double)player.position.Y + (double)player.height * 0.5) / 16.0);
                Vector2 pos = PickerUI.AutoPicker.Position.ToVector2() * 16;
                Rectangle rect = new Rectangle((int)(pos.X), (int)(pos.Y), 32, 32);//temp
                rect.Inflate(-1, -1);
                Point point = rect.ClosestPointInRect(player.Center).ToTileCoordinates();
                int pickerX = point.X;
                int pickerY = point.Y;
                if (playerX < pickerX - Player.tileRangeX || playerX > pickerX + Player.tileRangeX + 1 || playerY < pickerY - Player.tileRangeY || playerY > pickerY + Player.tileRangeY)
                {
                    CloseAutoPickerInterface();
                }
            }
        }

        public void CloseUsorInventory()
        {
            _menuBar.SetState(null);
            Main.editChest = false;
            Main.LocalPlayer.tileEntityAnchor.Clear();
            SoundEngine.PlaySound(SoundID.MenuClose);
        }

        public void CloseAutoPickerInterface()
        {
            _pickerInterface.SetState(null);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "YourMod: A Description",
                    delegate
                    {
                        _menuBar.Draw(Main.spriteBatch, new GameTime());
                        _pickerInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void OnWorldLoad()
        {
            if (_menuBar != null && _menuBar.CurrentState != null)
                CloseUsorInventory();
            if (_pickerInterface != null && _pickerInterface.CurrentState != null)
                CloseAutoPickerInterface();
        }

        public static Vector2 AdjustPosition(Vector2 renderPosition)
        {
            Vector2 cameradistance = renderPosition - Main.Camera.Center;
            renderPosition -= Main.screenPosition;
            if (Main.player[Main.myPlayer].gravDir == -1f)
            {
                renderPosition.Y = (float)Main.screenHeight - renderPosition.Y - 16f;
            }
            cameradistance -= cameradistance * Main.GameViewMatrix.Zoom;
            return renderPosition - cameradistance;
        }

        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin();
            Main.tileBatch.Begin();
            _pickerInterface.Draw(Main.spriteBatch, new GameTime());
            Main.tileBatch.End();
            Main.spriteBatch.End();
        }


    }
}
