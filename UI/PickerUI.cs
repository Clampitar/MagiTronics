using MagiTronics.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class PickerUI : UIState
    {
        private TEAutoPicker autoPicker;
        private Vector2 worldPosition;

        internal TEAutoPicker AutoPicker { get => autoPicker; set => setPicker(value); }

        public void setPicker(TEAutoPicker teAutoPicker)
        {
            autoPicker = teAutoPicker;
            worldPosition = teAutoPicker.Player.position;
            Recalculate();
        }

        public override void OnInitialize()
        {
            addbutton(false, false, TEAutoPicker.Direction.UP);
            addbutton(true, false, TEAutoPicker.Direction.LEFT);
            addbutton(false, true, TEAutoPicker.Direction.DOWN);
            addbutton(true, true, TEAutoPicker.Direction.RIGHT);
        }

        private void addbutton(bool horizontal, bool downright, TEAutoPicker.Direction dir)
        {
            int delay = 16;
            Asset<Texture2D> asset = ModContent.Request<Texture2D>("Terraria/Images/UI/TexturePackButtons");
            Rectangle rec = new Rectangle(downright ? 32 : 0, horizontal ? 32 : 0, 32, 32);
            int align = 32 * (downright ? 1 : -1);

            AutoPickerInterface panel = new AutoPickerInterface(dir);
            panel.Height.Set(36, 0);
            panel.Width.Set(36, 0);
            panel.Left.Set(horizontal ? align + delay : delay, 0);
            panel.Top.Set(horizontal ? delay : align + delay, 0);

            UIImageFramed picker = new UIImageFramed(asset, rec);
            picker.Top.Set(-delay + 4, 0);
            picker.Left.Set(-delay + 4, 0);
            panel.OnLeftClick += OnButtonClick;
            Append(panel);
            panel.Append(picker);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            adjustPosition();
            base.Draw(spriteBatch);
        }

        private void adjustPosition()
        {
            Vector2 position = MagitronicsWorld.AdjustPosition(worldPosition);
            Top.Set(position.Y, 0);
            Left.Set(position.X, 0);
            Recalculate();
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement is AutoPickerInterface autoPickerInterface)
            {
                autoPicker.ChangeDir(autoPickerInterface.direction);
            }
        }
    }
}
