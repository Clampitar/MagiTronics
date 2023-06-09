﻿using Terraria.ID;
using Terraria;
using System.Collections.Generic;

namespace MagiTronics
{
    internal class ChestPumpInventory
    {
        public Item[] items;
        Queue<int> emptySpaces;
        public int numBuckets = 0;
        int bucketindex = -1;
        int bottomlessBucketType = -1;
        readonly int[] bucketSpaces = new int[4];
        readonly bool[] sponge = new bool[4];
        readonly int[] bucketID;
        private bool invChanged;

        public Item usedItem = new Item(ItemID.EmptyBucket);
        public Item madeItem = new Item(ItemID.EmptyBucket);


        public ChestPumpInventory(Item[] items)
        {
            this.items = items;
            for (int i = 0; i < bucketSpaces.Length; i++)
            {
                bucketSpaces[i] = -1;
                sponge[i] = false;
            }
            bucketID = new int[3];
            bucketID[0] = ItemID.WaterBucket;
            bucketID[1] = ItemID.LavaBucket;
            bucketID[2] = ItemID.HoneyBucket;
            //int infiniteWaterID = ItemID.BottomlessBucket;
            //int infiniteLavaID = ItemID.BottomlessLavaBucket;
            emptySpaces = new Queue<int>();
            for (int i = 0; i < items.Length; i++)
            {
                Item item = items[i];
                if (item.IsAir)
                {
                    emptySpaces.Enqueue(i);
                }
                else switch(item.type)
                    {
                        case ItemID.EmptyBucket: numBuckets += item.stack;
                            bucketindex = i; break;
                        case ItemID.WaterBucket: bucketSpaces[0] = i; break;
                        case ItemID.LavaBucket:  bucketSpaces[1] = i; break;
                        case ItemID.HoneyBucket: bucketSpaces[2] = i; break;

                        case ItemID.BottomlessBucket: bottomlessBucketType = 0; break;
                        case ItemID.BottomlessLavaBucket: bottomlessBucketType = 1; break;
                        case ItemID.BottomlessHoneyBucket: bottomlessBucketType = 2; break;
                        case ItemID.BottomlessShimmerBucket: bottomlessBucketType = 3; break;

                        case ItemID.SuperAbsorbantSponge: sponge[0] = true; sponge[3] = true; break;
                        case ItemID.LavaAbsorbantSponge: sponge[1] = true; break;
                        case ItemID.HoneyAbsorbantSponge: sponge[2] = true; break;
                        case ItemID.UltraAbsorbantSponge:
                            sponge[0] = true; sponge[1] = true; sponge[2] = true; sponge[3] = true; break;

                    }
                    

            }
        }

        public bool WaterOut(out int liquidType)
        {
            if(bottomlessBucketType != -1)
            {
                liquidType = bottomlessBucketType; return true;
            }
            for(int type = 0; type < 3; type++)
                if (bucketSpaces[type] != -1 && TransferFromBucket(type))
                {
                    if (items[bucketSpaces[type]].stack <= 1)
                    {
                        items[bucketSpaces[type]].TurnToAir();
                        bucketSpaces[type] = -1;
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (items[i].type == bucketID[type])
                            {
                                bucketSpaces[type] = i; break;
                            }
                        }
                    }
                    else
                    {
                        items[bucketSpaces[type]].stack--;
                    }

                    invChanged = true;
                    liquidType = type;
                    return true;
                }
            liquidType = -1;
            return false;
        }

        public bool WaterIn(int liquidType)
        {
            if (sponge[liquidType]) return true;
            if (liquidType == 3) return false;
            if (bucketindex != -1)
                if (TransferToBucket(liquidType))
                {
                    if (items[bucketindex].stack <= 1)
                    {
                        items[bucketindex].TurnToAir();
                        bucketindex = -1;
                        for(int i = 0; i < items.Length; i++)
                        {
                            if (items[i].type == ItemID.EmptyBucket)
                            {
                                bucketindex = i; break;
                            }
                        }
                    } else items[bucketindex].stack--;
                    invChanged = true;
                    return true;
                }
            return false;
        }

        private bool TransferFromBucket(int liquidType)
        {
            if(bucketindex == -1)
            {
                if(!emptySpaces.TryDequeue(out bucketindex))
                {
                    bucketindex = -1;
                    return false;
                }
                else
                {
                    items[bucketindex] = new Item(ItemID.EmptyBucket);
                    usedItem = items[bucketindex];
                    return true;
                }
            }
            if (items[bucketindex].stack >= items[bucketindex].maxStack)
            {
                bucketindex = -1;
                return TransferFromBucket(liquidType);
            }
            items[bucketindex].stack++;
            usedItem = items[bucketindex];
            return true;
        }

        private bool TransferToBucket(int liquidType)
        {
            
            if (bucketSpaces[liquidType] == -1)
            {
                if (!emptySpaces.TryDequeue(out bucketSpaces[liquidType]))
                {
                    bucketSpaces[liquidType] = -1;
                    return false;
                }
                else {
                    items[bucketSpaces[liquidType]] = new Item(bucketID[liquidType]);
                    madeItem = items[bucketSpaces[liquidType]];
                    return true;
                }
            }
            if (items[bucketSpaces[liquidType]].stack >= items[bucketSpaces[liquidType]].maxStack)
            {
                bucketSpaces[liquidType] = -1;
                return TransferToBucket(liquidType);
            }
            items[bucketSpaces[liquidType]].stack++;
            madeItem = items[bucketSpaces[liquidType]];
            return true;
        }

        public bool getInvChanged()
        {
            return invChanged;
        }

    }
}
