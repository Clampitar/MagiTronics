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
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        public override void HitWire(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            x -= tile.TileFrameX / 18 % 3;//taken from ExampleMod's Lamp
            y -= tile.TileFrameY / 18 % 3;

            MagicWiring.newOutPump(x, y);

            Wiring.SkipWire(x, y);
            Wiring.SkipWire(x, y + 1);
            Wiring.SkipWire(x + 1, y);
            Wiring.SkipWire(x + 1, y + 1);
        }

        
    }


}
