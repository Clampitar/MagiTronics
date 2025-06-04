using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;

namespace MagiTronics.Tiles.Worldly
{
    internal class PiggyBank : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.addTile(Type);
        }

        public override bool RightClick(int x, int y)
        {
            ModContent.GetInstance<UISystem>().ToggleUI(
                ModContent.GetInstance<BankSystem>().PiggyBank.item,
                new Terraria.DataStructures.Point16(x, y),
                BankSystem.BankType.PiggyBank);
            return true;
        }

    }
}
