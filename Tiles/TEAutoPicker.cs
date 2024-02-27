using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Tiles
{
    internal class TEAutoPicker : TEItemUsor
    {
        private static readonly int pickTime = new Item(ItemID.NightmarePickaxe).useTime;

        private int useTime = 0;

        static readonly int power = new Item(ItemID.DeathbringerPickaxe).pick;
        enum Direction
        {
            STOP = 0,
            UP = 1,
            RIGHT = 2,
            DOWN = 3,
            LEFT = 4
        }

        private Direction dir = Direction.DOWN;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == ModContent.TileType<AutoPicker>());
        }

        public override void Update()
        {
            useTime--;
            if (useTime <= 0)
            {
                Pick();
                useTime = pickTime;
            }
        }

        private void Pick()
        {
            if (dir == Direction.STOP) { return; }
            List<Point16> terminals = TerminalChecker.TripWire(Position.X, Position.Y, 3, 3);
            int mindepth = Main.maxTilesX;
            Point16 p = Point16.NegativeOne;
            foreach (Point16 terminal in terminals)
            {
                int depth = pickterminal(terminal);
                if(depth < mindepth)
                {
                    mindepth = depth;
                    p = terminal;
                }
            }
            if(p!= Point16.NegativeOne)
            {
                GoDown(ref p, mindepth);
                usorPlayer.PickTile(p.X, p.Y, power);
            }
        }

        private int pickterminal(Point16 terminal)
        {
            int depth = 0;
            do
            {
                Tile tile = Main.tile[terminal.X, terminal.Y];
                if(tile.HasTile && WorldGen.CanPoundTile(terminal.X, terminal.Y))
                {
                    return depth;
                }
                depth++;
                GoDown(ref terminal);
            } while (terminal.Y < Main.maxTilesY);
            return -1;
        }

        void GoDown(ref Point16 p, int i = 1) => p -= new Point16(0, -i);
    }
}
