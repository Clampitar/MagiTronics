using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using MagiTronics.Tiles;

namespace MagiTronics.UI
{
    internal class AutoPickerInterface : UIElement
    {
        Color color = new(255, 255, 255);
        

        public TEAutoPicker te;
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("Terraria/Images/UI/TexturePackButtons"), MagitronicsWorld.AdjustPosition(te.Player.position), color);
        }//"C:\Users\clamp\Documents\My Games\Terraria\Images\Images\UI\TexturePackButtons.png"
    }
}
