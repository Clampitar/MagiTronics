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
        private const int TEXTURE_SIZE = 32;
        private const int PANEL_SIZE = 36;
        private const int DELAY = 16;
        private TEAutoPicker autoPicker;
        private Vector2 worldPosition;

        internal TEAutoPicker AutoPicker { get => autoPicker; set => setPicker(value); }

        public void setPicker(TEAutoPicker teAutoPicker)
        {
            autoPicker = teAutoPicker;
            worldPosition = teAutoPicker.Position.ToVector2() * 16;
            Recalculate();
        }

        public override void OnInitialize()
        {
            addbutton(false, false, TEAutoPicker.Direction.UP);
            addbutton(true, false, TEAutoPicker.Direction.LEFT);
            addbutton(false, true, TEAutoPicker.Direction.DOWN);
            addbutton(true, true, TEAutoPicker.Direction.RIGHT);

            Asset<Texture2D> asset = ModContent.Request<Texture2D>("MagiTronics/UI/StopButton");

            AutoPickerInterface panel = new AutoPickerInterface(TEAutoPicker.Direction.STOP);
            panel.Height.Set(PANEL_SIZE, 0);
            panel.Width.Set(PANEL_SIZE, 0);
            panel.Left.Set(DELAY, 0);
            panel.Top.Set(DELAY, 0);

            UIImage picker = new UIImage(asset);
            picker.Top.Set(-DELAY + 4, 0);
            picker.Left.Set(-DELAY + 4, 0);
            panel.OnLeftClick += OnButtonClick;
            Append(panel);
            panel.Append(picker);
        }

        private void addbutton(bool horizontal, bool downright, TEAutoPicker.Direction dir)
        {
            Asset<Texture2D> asset = ModContent.Request<Texture2D>("Terraria/Images/UI/TexturePackButtons");
            Rectangle rec = new(downright ? TEXTURE_SIZE : 0, horizontal ? TEXTURE_SIZE : 0, TEXTURE_SIZE, TEXTURE_SIZE);
            int align = TEXTURE_SIZE * (downright ? 1 : -1);

            AutoPickerInterface panel = new AutoPickerInterface(dir);
            panel.Height.Set(PANEL_SIZE, 0);
            panel.Width.Set(PANEL_SIZE, 0);
            panel.Left.Set(horizontal ? align + DELAY : DELAY, 0);
            panel.Top.Set(horizontal ? DELAY : align + DELAY, 0);

            UIImageFramed picker = new UIImageFramed(asset, rec);
            picker.Top.Set(-DELAY + 4, 0);
            picker.Left.Set(-DELAY + 4, 0);
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
            Vector2 position = TerminalSystem.AdjustPosition(worldPosition);
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
