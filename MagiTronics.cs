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

        public enum PacketId : byte
        {
            TICKTIMER = 0,
            SINGLETERMINAL = 1,
            WORLDLOAD = 2,
            USORITEM = 3,
            PICKERDIR = 4
        }
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
            byte msgType = reader.ReadByte();
            switch ((PacketId)msgType)
            {
                case PacketId.TICKTIMER:
                    int i = reader.ReadInt32();
                    int j = reader.ReadInt32();
                    TickTimer.Switch(i, j);
                    break;
                case PacketId.SINGLETERMINAL:
                    Point16 point = new Point16(reader.ReadInt16(), reader.ReadInt16());
                    bool add = reader.ReadBoolean();
                    if (Main.netMode == NetmodeID.Server)
                    {
                        if (add)
                            ModContent.GetInstance<TerminalSystem>().AddData(point);
                        else
                            ModContent.GetInstance<TerminalSystem>().RemoveData(point);
                    }
                    else
                    {
                        if (add)
                            TerminalSystem.AddDataClient(point);
                        else
                            TerminalSystem.RemoveDataClient(point);
                    }
                    break;
                case PacketId.WORLDLOAD:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModContent.GetInstance<TerminalSystem>().SendWorldData();
                    }
                    else
                    {
                        ModContent.GetInstance<TerminalSystem>().RecieveWorldData(reader);
                    }
                    break;
                case PacketId.USORITEM:
                    Point16 usorPos = new Point16(reader.ReadInt16(), reader.ReadInt16());
                    if (TileEntity.ByPosition.TryGetValue(usorPos, out var value) && value is TEItemUsor usor)
                    {
                        usor.SyncedItem(reader);
                    }
                    break;
                case PacketId.PICKERDIR:
                    Point16 pickerPos = new Point16(reader.ReadInt16(), reader.ReadInt16());
                    if (TileEntity.ByPosition.TryGetValue(pickerPos, out var te) && te is TEAutoPicker picker)
                    {
                        if(Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            picker.PickTile(reader.ReadInt16(), reader.ReadInt16());
                        } else
                        {
                            byte dir = reader.ReadByte();
                            picker.ChangeDir((TEAutoPicker.Direction)dir);
                        }
                    }
                    break;
                default:
                    Logger.Warn("id not recognized");
                    break;
            }
        }

    }
}