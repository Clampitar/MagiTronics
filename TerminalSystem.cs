
using MagiTronics.Items;
using MagiTronics.Tiles;
using MagiTronics.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
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
    internal class TerminalSystem : ModSystem
    {
        private static Point16 cursorTarget = Point16.NegativeOne;

        public static void SetCursorTarget() => cursorTarget = new Point16(Player.tileTargetX, Player.tileTargetY);
        public static void resetCursorTarget() => cursorTarget = Point16.NegativeOne;

        public static List<Point16> modedActuators = new();

        public static Texture2D texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;

        private Point16 pointToSync;

        private static byte syncmode = 2;
        public override void LoadWorldData(TagCompound tag)
        {
            modedActuators = tag.Get<List<Point16>>("modedActuators");
            texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;
            cursorTarget = Point16.NegativeOne;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("modedActuators", modedActuators);
        }

        public override void ClearWorld()
        {
            TerminalChecker.initialize();
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
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            if (cursorTarget != Point16.NegativeOne)
            {
                DrawTerminalCursor();
            }
            if (WiresUI.Settings.DrawWires)
            {
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
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition, rec, c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            r = 0.1f;
            b = 0.9f;
            g = 0.1f;
            a = 0.8f;
            colormult = 1f;
            c = Main.buffColor(newColor, r, g, b, a) * colormult;
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitX * -2f * scale, rec, c, 0f, Vector2.Zero, new Vector2(0.125f, 1f) * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitX * 16 * scale, rec, c, 0f, Vector2.Zero, new Vector2(0.125f, 1f) * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitY * -2f * scale, rec, c, 0f, Vector2.Zero, new Vector2(1f, 0.125f) * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, renderPosition + Vector2.UnitY * 16f * scale, rec, c, 0f, Vector2.Zero, new Vector2(1f, 0.125f) * scale, SpriteEffects.None, 0f);
        }

        public override void NetSend(BinaryWriter writer)
        {
           /* int x = pointToSync.X;
            int y = pointToSync.Y;
            writer.Write(syncmode);
            if (syncmode != 2)
            {
                writer.Write(x);
                writer.Write(y);
            }
            syncmode = 2;*/
        }

        public override void NetReceive(BinaryReader reader)
        {
           /* Byte msgType = reader.ReadByte();
            switch (msgType)
            {
                case 0:
                    int x = reader.ReadInt16();
                    int y = reader.ReadInt16();
                    AddDataClient(new Point16(x, y));
                    break;
            }
            syncmode = 2;*/
        }

        //called on client
        public bool placeTerminal(short x, short y)
        {
            Point16 point = new Point16(x, y);
            bool placed = !modedActuators.Contains(point);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket modPacket = Mod.GetPacket();
                modPacket.Write((byte)1);
                modPacket.Write(x);
                modPacket.Write(y);
                syncmode = 0;
                modPacket.Send();
            } else AddDataClient(point);
            return placed;
        }
        //called on server
        public void AddData(Point16 point)
        {
            //modedActuators.Clear();
            if (!modedActuators.Contains(point))
            {
                modedActuators.Add(point);
                ModPacket modPacket = Mod.GetPacket();
                modPacket.Write((byte)1);
                modPacket.Write(point.X);
                modPacket.Write(point.Y);
                syncmode = 0;
                modPacket.Send();
            }
        }

        //called on client
        public static void AddDataClient(Point16 point)
        {
            if (!modedActuators.Contains(point))
            {
                modedActuators.Add(point);
                SoundEngine.PlaySound(SoundID.Dig);
            }
        }

        public static void RemoveData(int x, int y)
        {
            Point16 point = new Point16(x, y);
            Vector2 pos = new Vector2(x * 16, y * 16);
            if (modedActuators.Contains(point))
            {
                modedActuators.Remove(point);
                SoundEngine.PlaySound(SoundID.Dig);
                for (int i = 0; i < 5; i++)
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
