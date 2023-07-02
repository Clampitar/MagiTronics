
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace MagiTronics
{
    internal class MagitronicsWorld
    {

        internal class TileData
        {
            public bool Pickor = false;
        }

        public static Dictionary<Point16, TileData> dict = new Dictionary<Point16, TileData>();


        public static void AddData(Point16 point)
        {
            if (!dict.ContainsKey(point))
            {
                dict.Add(point, new TileData());
                SoundEngine.PlaySound(SoundID.Dig);

            }
        }
    }
}
