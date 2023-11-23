using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.Tiles
{
    internal class TEBossSensor : ModTileEntity
    {
        bool on;

        short onX => (short)(on ? 18 : 0);
        private void Activate(bool newState)
        {
            if(newState != on)
            {
                on = newState;
                Tile tile = Main.tile[Position.X, Position.Y];
                tile.TileFrameX = onX;
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(Position.X, Position.Y));
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, base.Position.X, base.Position.Y);
                }
                Wiring.TripWire(Position.X, Position.Y, 1,1);
            }
        }

        public override void Update()
        {
            Activate(NPC.AnyDanger());
            base.Update();
        }

        public override bool IsTileValidForEntity(int x, int y) //this is only called when entering a world
        {
            Tile tile = Main.tile[x, y];
            if (tile.HasTile && tile.TileType == ModContent.TileType<BossSensor>())
            {
                on = NPC.AnyDanger();
                if(tile.TileFrameX != onX)
                {
                    tile.TileFrameX = onX;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, base.Position.X, base.Position.Y);
                    }
                    Wiring.TripWire(Position.X, Position.Y, 1, 1);
                }
                return true;
            }
            return false;
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
