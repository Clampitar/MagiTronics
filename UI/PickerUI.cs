using Humanizer;
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
            worldPosition = teAutoPicker.Position.ToVector2() * 16;
            Recalculate();
        }

        public override void OnInitialize()
        {
            addbutton("Up", 16, 0, TEAutoPicker.Direction.UP);
            addbutton("Left", 0, 16, TEAutoPicker.Direction.LEFT);
            addbutton("Down", 16, 32, TEAutoPicker.Direction.DOWN);
            addbutton("Right", 32, 16, TEAutoPicker.Direction.RIGHT);
            addbutton("Stop", 16, 16, TEAutoPicker.Direction.STOP);

        }

        private void addbutton(string name, int left, int top, TEAutoPicker.Direction dir)
        {
            Asset<Texture2D> asset = ModContent.Request<Texture2D>("MagiTronics/UI/"+name+"Button");
            Asset<Texture2D> assetBorder = ModContent.Request<Texture2D>("MagiTronics/UI/"+name+"Button_Highlight");

            AutoPickerButton button = new AutoPickerButton(asset, assetBorder, dir);
            button.Left.Set(left, 0);
            button.Top.Set(top, 0);
            button.OnLeftClick += OnButtonClick;
            Append(button);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            adjustPosition();
            base.Draw(spriteBatch);
        }

        private void adjustPosition()
        {
            Vector2 position = worldPosition - Main.screenPosition;
            if (Main.player[Main.myPlayer].gravDir == -1f)
            {
                position.Y = Main.screenHeight - position.Y - 48;
            }
            Top.Set(position.Y, 0);
            Left.Set(position.X, 0);
            Recalculate();
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement is AutoPickerButton autoPickerInterface)
            {
                autoPicker.ChangeDir(autoPickerInterface.direction);
            }
        }
    }
}
