using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;


namespace MagiTronics.Tiles
{
    public class ChestOutletPump : ModTile
    {

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
        }

        public override void HitWire(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            x -= tile.TileFrameX / 18 % 3;//taken from ExampleMod's Lamp
            y -= tile.TileFrameY / 18 % 3;

            Wiring._outPumpX[Wiring._numOutPump] = x;
            Wiring._outPumpY[Wiring._numOutPump] = y;
            Wiring._numOutPump++;

            MagicWiring._chestOutPumpX[MagicWiring._numChestOutPump] = x;
            MagicWiring._chestOutPumpY[MagicWiring._numChestOutPump] = y;
            MagicWiring._numChestOutPump++;

            Wiring.SkipWire(x, y);
            Wiring.SkipWire(x, y + 1);
            Wiring.SkipWire(x + 1, y);
            Wiring.SkipWire(x + 1, y + 1);
        }

        
    }


}
