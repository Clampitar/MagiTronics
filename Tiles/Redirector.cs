using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MagiTronics.Tiles
{
    internal class Redirector : ModTile
    {

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new Terraria.DataStructures.PlacementHook(ModContent.GetInstance<TERedirector>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            //TODO implement
            int id = FindByGuessing(i, j);
            if(id == -1)
            {
                return true;
            }
            TERedirector ut = (TERedirector)TileEntity.ByID[id];
            return ut.CanKill();
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            fail = false;
            int id = FindByGuessing(i, j);
            if (id == -1)
            {
                return;
            }
            TERedirector ut = (TERedirector)TileEntity.ByID[id];
            if (ut.target == Point16.NegativeOne)
            {
                return;
            }
            WorldGen.KillTile(ut.target.X, ut.target.Y);
            fail = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i + 16, j + 16), new Vector2(i + 16, j + 16), new Vector2(0, 0), ModContent.GetInstance<Items.Redirector>().Type);
            ModContent.GetInstance<TERedirector>().Kill(i, j);
        }

        public static int FindByGuessing(int x, int y)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (TileEntity.ByPosition.TryGetValue(new Point16(x-i, y-j), out var value) && value is TERedirector)
                    {
                        return value.ID;
                    }
                }
            }
            return -1;
        }
    }
}
