
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using MagiTronics.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using MagiTronics.UI;
using Terraria.UI;
using MagiTronics.Tiles;
using Terraria.Localization;
using System;

namespace MagiTronics
{
    internal class MagitronicsWorld : ModSystem
    {
        internal AutoUsorUI MenuBar;
        private UserInterface _menuBar;

        private PickerUI PickerUI;
        private UserInterface _pickerInterface;

        private static Point16 cursorTarget = Point16.NegativeOne;

        public static void SetCursorTarget() => cursorTarget = new Point16(Player.tileTargetX, Player.tileTargetY);
        public static void resetCursorTarget() => cursorTarget = Point16.NegativeOne;


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

        public void ToggleUI(Player usor)
        {
            if (_menuBar != null)
            {
                if (_menuBar.CurrentState != null)
                {
                    if (MenuBar.Player == usor)
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
            if(_pickerInterface != null)
            {
                if (_pickerInterface.CurrentState != null)
                {
                    if(PickerUI.AutoPicker == autoPicker)
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

        private void OpenUI(Player usor, bool hadInvOpen)
        {
            Player player = Main.LocalPlayer;
            MenuBar.Player = usor;
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
            player.chestX = (int)(usor.position.X / 16);
            player.chestY = (int)(usor.position.Y / 16);
            Main.playerInventory = true;
            SoundEngine.PlaySound(hadchestOpen || hadInvOpen ? SoundID.MenuTick : SoundID.MenuOpen);
        }

        private void OpenUI(TEAutoPicker picker)
        {
            PickerUI.AutoPicker = picker;
            _pickerInterface.SetState(PickerUI);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public void KilledUsor(Player usor)
        {
            if(MenuBar.Player == usor)
            {
                CloseUsorInventory();
            }
        }

        public void checkValidUI(Player player)
        {
            if(_menuBar.CurrentState != null)
            {
                if(player.chest != -1)
                {
                    CloseUsorInventory();
                    return;
                }
                int playerX = (int)(((double)player.position.X + (double)player.width * 0.5) / 16.0);
                int playerY = (int)(((double)player.position.Y + (double)player.height * 0.5) / 16.0);
                Vector2 pos = MenuBar.Player.position;
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
                        //_pickerInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public override void LoadWorldData(TagCompound tag)
        {
            modedActuators =  tag.Get<List<Point16>>("modedActuators");
            texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;
            cursorTarget = Point16.NegativeOne;
            TerminalChecker.initialize();
            CloseUsorInventory();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("modedActuators", modedActuators);
        }

        public static List<Point16> modedActuators = new();

        public static Texture2D texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;

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
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            _pickerInterface.Draw(Main.spriteBatch, new GameTime());
            if (cursorTarget != Point16.NegativeOne)
            {
                DrawTerminalCursor();
            }
            if(WiresUI.Settings.DrawWires) {
            foreach (Point16 point in modedActuators)
                Main.spriteBatch.Draw(texture, AdjustPosition(new Vector2(point.X * 16, point.Y * 16)), new Rectangle(0, 0, 16, 16), new Color(255, 255, 255), 0f, default, zoom, SpriteEffects.None, 0f);
            }
        Main.tileBatch.End();
        Main.spriteBatch.End();
        }

        public static void DrawTerminalCursor()
        {
            Vector2 renderPosition = new Vector2(cursorTarget.X, cursorTarget.Y) * 16f;
            renderPosition = AdjustPosition(renderPosition);
            Color newColor = Lighting.GetColor(cursorTarget.X, cursorTarget.Y);
            Rectangle rec = new Rectangle(0, 0, 16, 16);
            float r = 0.3f;
            float g = 0.3f;
            float b = 0.9f;
            float a = 0.8f;
            float colormult = 0.8f;
            Color c = Main.buffColor(newColor, r, g, b, a) * colormult;
            Vector2 scale = Main.GameViewMatrix.Zoom;
            //Main.spriteBatch.Draw(TextureAssets.SmartDig.Value, renderPosition, rec, c * colormult, 0f, default, Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition , rec, c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            r = 0.1f;
            b = 0.9f;
            g = 0.1f;
            a = 0.8f;
            colormult = 1f;
            c = Main.buffColor(newColor, r, g, b, a) * colormult;
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitX * -2f * scale, rec, c, 0f, Vector2.Zero, new Vector2(0.125f, 1f) * scale , SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitX * 16 * scale, rec, c, 0f, Vector2.Zero, new Vector2(0.125f, 1f) * scale , SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitY * -2f * scale  , rec, c, 0f, Vector2.Zero, new Vector2(1f, 0.125f) *  scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitY * 16f * scale, rec, c, 0f, Vector2.Zero, new Vector2(1f, 0.125f) * scale , SpriteEffects.None, 0f);
        }

        public static bool AddData(Point16 point)
        {
            if (!modedActuators.Contains(point))
            {
                modedActuators.Add(point);
                SoundEngine.PlaySound(SoundID.Dig);
                return true;
            }
            return false;
        }

        public static void RemoveData(int x, int y)
        {
            Point16 point = new Point16(x, y);
            Vector2 pos = new Vector2(x* 16, y*16);
            if (modedActuators.Contains(point))
            {
                modedActuators.Remove(point);
                SoundEngine.PlaySound(SoundID.Dig);
                for(int i = 0; i < 5; i++)
                    Dust.NewDust(pos, 1, 1, DustID.Adamantite);
                int number = Item.NewItem(new EntitySource_TileBreak(x, y), pos, 16, 16, ModContent.ItemType<UsageTerminal>());
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }
            }
        }

    }
}
