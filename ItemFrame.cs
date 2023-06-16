using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;

namespace MagiTronics
{
    internal class ItemFrame : ManagableChest
    {
        private Item item;

        public ItemFrame(Item item)
        {
            this.item = item;
        }

        public static int FindByGuessing(int x, int y)
        {
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    int id = TEItemFrame.Find(x-i, y-j);
                    if(id >= 0)
                        return id;
                }
            }
            return -1;
        }

        public override bool WaterOut(out int liquidType)
        {
            switch (item.type)
            {
                case ItemID.BottomlessBucket:
                    liquidType = 0;
                    return true;
                case ItemID.BottomlessLavaBucket:
                    liquidType = 1;
                    return true;
                case ItemID.BottomlessHoneyBucket:
                    liquidType = 2;
                    return true;
                case ItemID.BottomlessShimmerBucket:
                    liquidType = LiquidID.Shimmer;
                    return true;
                case ItemID.WaterBucket:
                    liquidType = 0;
                    item.type = ItemID.EmptyBucket;
                    return true;
                case ItemID.LavaBucket:
                    liquidType = 1;
                    item.type = ItemID.EmptyBucket;
                    return true;
                case ItemID.HoneyBucket:
                    liquidType = 2;
                    item.type = ItemID.EmptyBucket;
                    return true;
                default:
                    liquidType = -1;
                    return false;

            }
        }

        public override bool WaterIn(int liquidType)
        {
            if (item.type == ItemID.EmptyBucket)
            {
                switch (liquidType)
                {
                    case LiquidID.Water:
                        item.type = ItemID.WaterBucket;
                        break;
                    case LiquidID.Lava:
                        item.type = ItemID.LavaBucket;
                        break;
                    case LiquidID.Honey:
                        item.type = ItemID.HoneyBucket;
                        break;
                    default: return false;
                }
                return true;
            }
            if (item.type == ItemID.UltraAbsorbantSponge)
                return true;
            if(item.type == ItemID.SuperAbsorbantSponge && (liquidType == LiquidID.Water || liquidType == LiquidID.Shimmer))
                return true;
            if(item.type == ItemID.LavaAbsorbantSponge && liquidType == LiquidID.Lava)
                return true;
            if (item.type == ItemID.HoneyAbsorbantSponge && liquidType == LiquidID.Honey)
                return true;
            return false;
        }
    }
}
