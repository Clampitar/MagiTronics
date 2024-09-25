
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
using System.Linq;
using System;

namespace MagiTronics
{
    internal class TerminalSystem : ModSystem
    {
        private static Point16 cursorTarget = Point16.NegativeOne;

        public static void SetCursorTarget() => cursorTarget = new Point16(Player.tileTargetX, Player.tileTargetY);
        public static void resetCursorTarget() => cursorTarget = Point16.NegativeOne;

        public static List<Point16> modedActuators;

        //Modified only on world load. Used for world syncing
        private static List<Point16> savedTerminals;

        public static Texture2D texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;

        public override void LoadWorldData(TagCompound tag)
        {
            modedActuators = tag.Get<List<Point16>>("modedActuators");
            savedTerminals = new List<Point16>(modedActuators);
            texture = ModContent.Request<Texture2D>("Magitronics/Tiles/Terminal", AssetRequestMode.ImmediateLoad).Value;
            cursorTarget = Point16.NegativeOne;
        }

        public void SendWorldData()
        {
            Console.WriteLine("list is " + modedActuators.Count());
            ModPacket modPacket = Mod.GetPacket();
            modPacket.Write((byte)2);
            modPacket.Write(modedActuators.Count);
            foreach (Point16 point in modedActuators)
            {
                modPacket.Write(point.X);
                modPacket.Write(point.Y);
            }
            modPacket.Send();
        }

        public void RecieveWorldData(BinaryReader reader)
        {
            int PointCount = reader.ReadInt32();
            for (int i = 0; i < PointCount; i++)
            {
                short x = reader.ReadInt16();
                short y = reader.ReadInt16();
                Point16 point = new Point16(x, y);
                if (!modedActuators.Contains(point))
                {
                    modedActuators.Add(point);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("modedActuators", modedActuators);
        }

        public override void ClearWorld()
        {
            TerminalChecker.initialize();
            modedActuators = [];
            savedTerminals = [];
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
            Point16 testPoint = new Point16(42, 42);
            if (!modedActuators.Contains(testPoint))
            {
                modedActuators.Add(testPoint);
            }
            if (savedTerminals.Contains(testPoint))
            {
                savedTerminals.Remove(testPoint);
            }

            List<Point16> newPoints = modedActuators.Where(point => !savedTerminals.Contains(point)).ToList<Point16>();
            List<Point16> removedPoints = savedTerminals.Where(point => !modedActuators.Contains(point)).ToList<Point16>();
            writer.Write(newPoints.Count);
            foreach (Point16 point in newPoints)
            {
                writer.Write(point.X);
                writer.Write(point.Y);
            }
            writer.Write(removedPoints.Count);
            foreach (Point16 point in removedPoints)
            {
                writer.Write(point.X);
                writer.Write(point.Y);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            int newPointCount = reader.ReadInt32();
            for (int i = 0; i < newPointCount; i++)
            {
                short x = reader.ReadInt16();
                short y = reader.ReadInt16();
                Point16 point = new Point16(x, y);
                if (!modedActuators.Contains(point))
                {
                    modedActuators.Add(point);
                }
            }
            int removedPointCount = reader.ReadInt32();
            for (int i = 0; i < removedPointCount; i++)
            {
                short x = reader.ReadInt16();
                short y = reader.ReadInt16();
                Point16 point = new Point16(x, y);
                if (modedActuators.Contains(point))
                {
                    modedActuators.Remove(point);
                }
            }
        }

        //called on client
        public bool placeTerminal(short x, short y)
        {
            Point16 point = new Point16(x, y);
            bool placed = !modedActuators.Contains(point);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SyncTerminal(point, true);
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
                SyncTerminal(point, true);
            }
        }

        //called on client
        public static void AddDataClient(Point16 point)
        {
            if (!modedActuators.Contains(point))
            {
                modedActuators.Add(point);
                Vector2 pos = new Vector2(point.X * 16, point.Y * 16);
                SoundEngine.PlaySound(SoundID.Dig, pos);
            }
        }

        public void DestroyTerminal(int x, int y)
        {
            Point16 point = new Point16(x, y);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SyncTerminal(point, false);
            }
            else RemoveDataClient(point);
        }
        public void RemoveData(Point16 point)
        {
            if (modedActuators.Contains(point))
            {
                modedActuators.Remove(point);
                Vector2 pos = new Vector2(point.X * 16, point.Y * 16);
                int number = Item.NewItem(new EntitySource_TileBreak(point.X, point.Y), pos, 16, 16, ModContent.ItemType<UsageTerminal>());
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }

                SyncTerminal(point, false);
            }
        }

        public static void RemoveDataClient(Point16 point)
        {
            if (modedActuators.Contains(point))
            {
                modedActuators.Remove(point);
                Vector2 pos = new Vector2(point.X * 16, point.Y * 16);
                if(Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(new EntitySource_TileBreak(point.X, point.Y), pos, 16, 16, ModContent.ItemType<UsageTerminal>());
                }
                SoundEngine.PlaySound(SoundID.Dig, pos);
                for (int i = 0; i < 5; i++)
                    Dust.NewDust(pos, 1, 1, DustID.Adamantite);
            }
        }

        private void SyncTerminal(short x, short y, bool add)
        {
            ModPacket modPacket = Mod.GetPacket();
            modPacket.Write((byte)1);
            modPacket.Write(x);
            modPacket.Write(y);
            modPacket.Write(add);
            modPacket.Send();
        }

        private void SyncTerminal(Point16 point, bool add)
        {
            SyncTerminal(point.X, point.Y, add);
        }

    }
}
