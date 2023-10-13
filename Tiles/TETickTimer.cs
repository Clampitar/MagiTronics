using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Tiles
{
    internal class TETickTimer : ModTileEntity
    {
        int time = -1;
        public void Activate()
        {
            time = MagicWiring.CountLamps(Position.X, Position.Y - 1);
            if (time < 1)
                time = 1;
        }

        public void Deactivate()
        {
            time = -1;
        }

        public void SpendTick()
        {
            if(time > -1)
            {
                time--;
                if(time < 1)
                {
                    Wiring.TripWire(Position.X, Position.Y, 1, 1);
                    Activate();
                }
            }

        }

        public override void Update()
        {
            SpendTick();
            base.Update();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<TickTimer>();
        }

        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: x, number2: y, number3: Type);
            }

            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            // Set "tileOrigin" to the same value you set TileObjectData.newTile.Origin to in the ModTile
            int placedEntity = Place(x, y);
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }
    }
}
