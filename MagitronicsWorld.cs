
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

namespace MagiTronics
{
    internal class MagitronicsWorld : ModSystem
    {
        internal MenuBar MenuBar;
        private UserInterface _menuBar;

        private static Point16 cursorTarget = Point16.NegativeOne;

        public static void SetCursorTarget() => cursorTarget = new Point16(Player.tileTargetX, Player.tileTargetY);
        public static void resetCursorTarget() => cursorTarget = Point16.NegativeOne;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                MenuBar = new MenuBar();
                MenuBar.Activate();
                _menuBar = new UserInterface();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _menuBar?.Update(gameTime);
        }

        public void toggleUI()
        {
            if (_menuBar != null)
            {
                _menuBar.SetState(_menuBar.CurrentState != null ? null : MenuBar);
            }
        }

        public void ShowUI()
        {
            _menuBar?.SetState(MenuBar);
        }

        public void HideUI()
        {
            _menuBar?.SetState(null);
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
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("modedActuators", modedActuators);
        }

        public static List<Point16> modedActuators = new();

        public static Texture2D texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;

        private static Vector2 AdjustPosition(Vector2 renderPosition)
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
