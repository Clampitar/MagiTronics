using MagiTronics.Tiles;
using Terraria;
using Terraria.UI;

namespace MagiTronics.UI
{
    internal class PickerUI : UIState
    {
        public AutoPickerInterface picker;
        public TEAutoPicker AutoPicker { get => picker.te; set => picker.te = value; }
        public override void OnInitialize()
        {
            picker = new AutoPickerInterface();
            Append(picker);
        }
    }
}
