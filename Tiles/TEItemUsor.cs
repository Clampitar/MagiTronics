
using Terraria;
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
            usorPlayer.inventory[0] = new Item(ItemID.IronPickaxe);
            usorPlayer.selectedItem = 0;
        }
        override protected void UpdateTarget(bool right, bool down)
        {
            workingTE = this;
            wiredTerminals.Clear();
            TerminalChecker.TripWire(Position.X, Position.Y+1, 2, 3);
            workingTE = null;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == ModContent.TileType<ItemUsor>());
        }

        public override Player Player => usorPlayer;
    }
}
