using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace MagiTronics.UI
{
    class InventoryUI : UIElement
    {
        Color color = new(255, 255, 255);

        public Item[] Inv;
        public BankSystem.BankType bankType;
        public override void Draw(SpriteBatch spriteBatch)
        {
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int num = (int)(73f + (float)(i * 56) * Main.inventoryScale);
                    int num2 = (int)((float)Main.instance.invBottom + (float)(j * 56) * Main.inventoryScale);
                    int slot = i + j * 10;
                    new Color(100, 100, 100, 100);
                    if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, num, num2, TextureAssets.InventoryBack.Width() * Main.inventoryScale,
                        TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.LocalPlayer.mouseInterface = true;
                        int context = ItemSlot.Context.ChestItem;
                        ItemSlot.OverrideHover(Inv, context, slot);
                        ItemSlot.LeftClick(Inv, context, slot);
                        if (bankType == BankSystem.BankType.ItemUsor)
                        {
                            ModContent.GetInstance<BankSystem>().currentIU.OverrideRightClick(slot);
                        }
                        else
                        {
                            ItemSlot.RightClick(Inv, ItemSlot.Context.InventoryItem, slot);
                        }
                        if (Main.mouseLeftRelease && Main.mouseLeft)
                            Recipe.FindRecipes();

                        ItemSlot.MouseHover(Inv, context, slot);

                    }
                    ItemSlot.Draw(spriteBatch, Inv, ItemSlot.Context.InventoryItem, slot, new Vector2(num, num2));
                }
            }
        }
    }
}
