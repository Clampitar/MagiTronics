using MagiTronics.Tiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics
{
    internal class ModPlayer : Terraria.ModLoader.ModPlayer
    {
        Point16 shift = new Point16();
        public override bool PreItemCheck()
        {
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if(tile.TileType == ModContent.TileType<Redirector>())
            {
                shift = Redirector.Redirect(Player.tileTargetX, Player.tileTargetY);
                Player.tileTargetX += shift.X;
                Player.tileTargetY += shift.Y;
                Player.tileRangeX += Math.Abs(shift.X);
                Player.tileRangeY += Math.Abs(shift.Y);
                TerminalSystem.SetCursorTarget();
            }
            else
            {
                TerminalSystem.resetCursorTarget();
            }
            return true;
        }

        public override void PostItemCheck()
        {
            Player.tileTargetX -= shift.X;
            Player.tileTargetY -= shift.Y;
            Player.tileRangeX -= Math.Abs(shift.X);
            Player.tileRangeY -= Math.Abs(shift.Y);
            shift = Point16.Zero;
        }

        public override void PostUpdate()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ModContent.GetInstance<UISystem>().checkValidUI(this.Player);
            }
        }

        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket modPacket = Mod.GetPacket();
                modPacket.Write((byte)2);
                modPacket.Send();

            }
        }
    }
}
