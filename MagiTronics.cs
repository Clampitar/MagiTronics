using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using System;

namespace MagiTronics
{
	public class MagiTronics : Mod
	{
        public override void Load()
        {
            IL_Wiring.XferWater += Wiring_XferWater;
            IL_Wiring.LogicGatePass += LogicPass;
        }

        private void Wiring_XferWater(ILContext il)
        {
            try
            {
                ILCursor cursor = new ILCursor(il);
                cursor.Goto(0);

                cursor.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(MagicWiring).GetMethod("XferWater", new Type[] { }));
            }
            catch
            {

                MonoModHooks.DumpIL(ModContent.GetInstance<MagiTronics>(), il);
            }
        }

        private void LogicPass(ILContext il)
        {
            try
            {
                ILCursor cursor = new ILCursor(il);
                cursor.Goto(0);

                cursor.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(MagicWiring).GetMethod("LogicPass", new Type[] { }));
            }
            catch
            {

                MonoModHooks.DumpIL(ModContent.GetInstance<MagiTronics>(), il);
            }
        }

        

    }
}