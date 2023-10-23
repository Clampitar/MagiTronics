
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
using System.IO;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.GameContent.UI;

namespace MagiTronics
{
    internal class MagitronicsWorld : ModSystem
    {

        private static Point16 cursorTarget = Point16.NegativeOne;

        public static void SetCursorTarget() => cursorTarget = new Point16(Player.tileTargetX, Player.tileTargetY);
        public static void resetCursorTarget() => cursorTarget = Point16.NegativeOne;

        public override void LoadWorldData(TagCompound tag)
        {
            modedActuators =  tag.Get<List<Point16>>("modedActuators");
            texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;
            cursorTarget = Point16.NegativeOne;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("modedActuators", modedActuators);
        }

        public static List<Point16> modedActuators = new();

        public static Texture2D texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;

        public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
        {
            if(messageType == MessageID.TileManipulation && Main.netMode == NetmodeID.Server)
            {

            }
            return false;
        }

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
            float b = 1f;
            float a = 1f;
            float colormult = 255f;
            Color c = Main.buffColor(newColor, r, g, b, a);
            Main.spriteBatch.Draw(TextureAssets.SmartDig.Value, renderPosition, rec, c * colormult, 0f, default, Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);
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
