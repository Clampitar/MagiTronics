using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using MagiTronics.Tiles;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;
using MagiTronics.Items;
using System;
using Terraria.GameContent;
using Terraria.GameInput;

namespace MagiTronics.UI
{
    internal class AutoPickerButton : UIImageButton
    {
        public TEAutoPicker.Direction direction;

        private float _visibilityActive = 1f;
        private float _visibilityInactive = 0.4f;
        public AutoPickerButton(Asset<Texture2D> texture, Asset<Texture2D> borderTexture, TEAutoPicker.Direction direction) : base(texture)
        {
            SetHoverImage(borderTexture);
            this.direction = direction;
            IgnoresMouseInteraction = false;
            Width.Set(16, 0);
            Height.Set(16, 0);
            SetVisibility(0.6f, 1f);
        }

    }
}
