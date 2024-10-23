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

        public override bool ContainsPoint(Vector2 point)
        {
            float globalTop = Top.Pixels + Parent.Top.Pixels;
            float globalLeft = Left.Pixels + Parent.Left.Pixels;
            Vector2 pos = new Vector2(globalLeft, globalTop) ;
            Vector2 centerDistance = pos - Main.Camera.Center + Main.screenPosition;
            centerDistance -= centerDistance * Main.GameViewMatrix.Zoom;
            pos -= centerDistance;

            return point.X > pos.X
                && point.Y > pos.Y
                && point.X < pos.X + (Width.Pixels * Main.GameViewMatrix.Zoom.X)
                && point.Y < pos.Y + (Height.Pixels * Main.GameViewMatrix.Zoom.Y);
        }

    }
}
