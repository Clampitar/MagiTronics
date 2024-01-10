using MagiTronics.Tiles;
using System;
using Terraria;
using Terraria.DataStructures;
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
                MagitronicsWorld.SetCursorTarget();
            }
            else
            {
                MagitronicsWorld.resetCursorTarget();
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
            ModContent.GetInstance<MagitronicsWorld>().checkValidUI(this.Player);
        }

    }
}
