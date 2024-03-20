using MagiTronics.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class PickerUI : UIState
    {
        public AutoPickerInterface picker;
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
            picker = new AutoPickerInterface(ModContent.Request<Texture2D>("Terraria/Images/UI/TexturePackButtons"));
            UIPanel button = new UIPanel();
            button.Width.Set(64, 0);
            button.Height.Set(64, 0); 
            button.OnLeftClick += OnButtonClick; 
            Append(button);
            button.Append(picker);
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
            Main.NewText("clicked: "+ listeningElement);
        }
    }
}
