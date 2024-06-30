using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent;
using Humanizer;
using Terraria.GameInput;
using MagiTronics.Tiles;

namespace MagiTronics.UI
{
    class UsorInventory : UIElement
    {
        Color color = new(255, 255, 255);
        private TEItemUsor itemUsor = new();

        public TEItemUsor Usor { get => itemUsor; set => itemUsor = value; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            Item[] inv = itemUsor.Player.inventory;
            int context = 0;
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
                        ItemSlot.Handle(inv, ItemSlot.Context.ChestItem, slot);

                    }
                    ItemSlot.Draw(spriteBatch, inv, context, slot, new Vector2(num, num2));
                }
            }
        }
    }
}
