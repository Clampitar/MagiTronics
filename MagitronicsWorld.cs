
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using MagiTronics.Items;

namespace MagiTronics
{
    internal class MagitronicsWorld : ModSystem
    {

        internal class TileData
        {
            public bool Pickor = false;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            modedActuators =  tag.Get<List<Point16>>("modedActuators");
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("modedActuators", modedActuators);
        }

        public static List<Point16> modedActuators = new();


        public static bool AddData(Point16 point)
        {
            if (!modedActuators.Contains(point))
            {
                modedActuators.Add(point);
                SoundEngine.PlaySound(SoundID.Dig);
                return true;
            }
            return false;
        }

        public static void RemoveData(int x, int y)
        {
            Point16 point = new Point16(x, y);
            Vector2 pos = new Vector2(x* 16, y*16);
            if (modedActuators.Contains(point))
            {
                modedActuators.Remove(point);
                SoundEngine.PlaySound(SoundID.Dig);
                for(int i = 0; i < 5; i++)
                    Dust.NewDust(pos, 1, 1, DustID.Adamantite);
                Item.NewItem(new EntitySource_TileBreak(x, y), pos, 16, 16, ModContent.ItemType<UsageTerminal>());

            }
        }

    }
}
