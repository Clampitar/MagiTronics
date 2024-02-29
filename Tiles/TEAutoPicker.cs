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

        private delegate bool MapCondition(Point16 p);

        MapCondition InMap;

        private delegate Point16 SearchDirection(int i = 1);

        SearchDirection Go;
        public enum Direction
        {
            STOP = 0,
            UP = 1,
            RIGHT = 2,
            DOWN = 3,
            LEFT = 4
        }

        private Direction dir = Direction.STOP;

        public TEAutoPicker()
        {
            ChangeDir(Direction.STOP);
        }
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

        public void ChangeDir(Direction direction)
        {
            dir = direction;
            switch (direction)
            {
                case Direction.STOP:
                    InMap = (Point16 p) => false;
                    break;
                case Direction.UP:
                    InMap = (Point16 p) => p.Y > 0;
                    Go = (int i = 1) => new Point16(0, -i);
                    break;
                case Direction.RIGHT:
                    InMap = (Point16 p) => p.X < Main.maxTilesX;
                    Go = (int i = 1) => new Point16(i, 0);
                    break;
                case Direction.DOWN:
                    InMap = (Point16 p) => p.Y < Main.maxTilesY;
                    Go = (int i = 1) => new Point16(0, i);
                    break;
                case Direction.LEFT:
                    InMap = (Point16 p) => p.X > 0;
                    Go = (int i = 1) => new Point16(-i, 0);
                    break;
            }
        }

        private void Pick()
        {
            if (dir == Direction.STOP) { return; }
            List<Point16> terminals = TerminalChecker.TripWire(Position.X, Position.Y, 3, 3);
            int mindepth = -1;
            Point16 p = Point16.NegativeOne;
            foreach (Point16 terminal in terminals)
            {
                int depth = Pickterminal(terminal);
                if (depth >= 0 && (depth < mindepth || mindepth == -1))
                {
                    mindepth = depth;
                    p = terminal;
                }
            }
            if(p!= Point16.NegativeOne && mindepth != -1)
            {
                p += Go(mindepth);
                usorPlayer.PickTile(p.X, p.Y, power);
            }
        }

        private int Pickterminal(Point16 terminal)
        {
            int depth = 0;
            do
            {
                Tile tile = Main.tile[terminal.X, terminal.Y];
                int type = tile.TileType;
                if (tile.HasTile && WorldGen.CanKillTile(terminal.X, terminal.Y) && !Main.tileHammer[type] && !Main.tileAxe[type])
                {
                    return depth;
                }
                depth++;
                terminal += Go();
            } while (InMap(terminal));
            return -1;
        }

    }
}
