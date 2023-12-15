﻿
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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