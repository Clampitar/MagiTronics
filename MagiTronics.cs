using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using System;
using System.IO;
using Terraria.DataStructures;
using MagiTronics.Tiles;
using Terraria.ID;

namespace MagiTronics
{
	public class MagiTronics : Mod
	{
        public static bool magicStorageLoaded = false;
        public override void Load()
        {
            IL_Wiring.XferWater += Wiring_XferWater;
            IL_Wiring.LogicGatePass += LogicPass;

            magicStorageLoaded = ModLoader.HasMod("MagicStorage");
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

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Byte msgType = reader.ReadByte();
            switch (msgType)
            {
                case 0:
                    int i = reader.ReadInt32();
                    int j = reader.ReadInt32();
                    TickTimer.Switch(i, j);
                    break;
                case 1:
                    Point16 point = new Point16(reader.ReadInt16(), reader.ReadInt16());
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModContent.GetInstance<TerminalSystem>().AddData(point);
                    }
                    else
                    {
                        TerminalSystem.AddDataClient(point);
                    }
                    break;
                default:
                    Logger.Warn("id not recognized");
                    break;
            }
        }

    }
}