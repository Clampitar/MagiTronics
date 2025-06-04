using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;

namespace MagiTronics.Tiles.Worldly
{
    internal class VoidVault : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
            TileObjectData.addTile(Type);
        }

        public override bool RightClick(int x, int y)
        {
            ModContent.GetInstance<UISystem>().ToggleUI(
                ModContent.GetInstance<BankSystem>().VoidVault.item,
                new Terraria.DataStructures.Point16(x, y),
                BankSystem.BankType.VoidVault);
            return true;
        }

    }
}
