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

namespace MagiTronics.UI
{
    internal class AutoPickerInterface : UIPanel
    {
        public TEAutoPicker.Direction direction;
        public AutoPickerInterface(TEAutoPicker.Direction direction) : base()
        {
            this.direction = direction;
        }

    }
}
