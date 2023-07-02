using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using System;
using Mono.Cecil.Cil;

namespace MagiTronics
{
	public class MagiTronics : Mod
	{
        public override void Load()
        {
            IL_Wiring.XferWater += Wiring_XferWater;
            IL_Wiring.HitWireSingle += HitWireSingle;
            IL_Main.DrawWires += DrawWire;
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

        private void HitWireSingle(ILContext il)
        {
            try
            {
                ILCursor cursor = new ILCursor(il);
                cursor.Goto(0);

                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Call, typeof(MagicWiring).GetMethod("HitwireChest", new Type[] { typeof(Int32), typeof(Int32) }));
            }
            catch
            {

                MonoModHooks.DumpIL(ModContent.GetInstance<MagiTronics>(), il);
            }
        }

        private void DrawWire(ILContext il)
        {
            try
            {
                ILCursor cursor = new ILCursor(il);
                cursor.Goto(0);
    
                cursor.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(MagicWiring).GetMethod("DrawMagicWire", new Type[] { }));

            }
            catch
            {

                MonoModHooks.DumpIL(ModContent.GetInstance<MagiTronics>(), il);
            }
        }

        

    }
}