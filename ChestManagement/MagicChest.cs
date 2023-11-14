

using MagicStorage.Components;
using System.Net.Sockets;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MagiTronics.ChestManagement;

internal class MagicChest : ManagableChest
{
    public bool valid = false;
    private TEStorageHeart heart;
    readonly bool[] sponge = new bool[4];
    readonly int[] bucketID = new int[3];

    [JITWhenModsEnabled("MagicStorage")]
    public MagicChest(int x, int y)
    {

        Tile tile = Main.tile[x, y];
        if(tile.TileFrameX % 36 == 18)
        {
            x--;
        }
        if(tile.TileFrameY % 36  == 18)
        {
            y--;
        }
        Point16 point = new Point16(x, y);
        if (TEStorageHeart.IsStorageCenter(point))
        {
            accessHeart(point);
        } else
        {
            point = TEStorageComponent.FindStorageCenter(point);
            if (point.X > 0 && point.Y > 0 && TileEntity.ByPosition.ContainsKey(point))
            {
                accessHeart(point);
            }
        }
    }
    [JITWhenModsEnabled("MagicStorage")]
    private void accessHeart(Point16 point)
    {
        TileEntity te = TileEntity.ByPosition[point];
        if (!(te is TEStorageCenter))
        {
            heart = null;
        }
        heart = ((TEStorageCenter)te).GetHeart();
        valid = true;
        bucketID[0] = ItemID.WaterBucket;
        bucketID[1] = ItemID.LavaBucket;
        bucketID[2] = ItemID.HoneyBucket;
        sponge[0] = heart.HasItem(new Item(ItemID.SuperAbsorbantSponge));
        sponge[1] = heart.HasItem(new Item(ItemID.LavaAbsorbantSponge));
        sponge[2] = heart.HasItem(new Item(ItemID.HoneyAbsorbantSponge));
        sponge[3] = heart.HasItem(new Item(ItemID.UltraAbsorbantSponge));
    }
    [JITWhenModsEnabled("MagicStorage")]
    public override bool WaterIn(int liquidType)
    {
        if (sponge[liquidType] || sponge[3]) return true;
        if(liquidType == LiquidID.Shimmer) return sponge[0];
        Item bucket = new Item(ItemID.EmptyBucket);
        if(!heart.HasItem(bucket)) return false;
        Item fullBucket = new Item(bucketID[liquidType]);
        heart.DepositItem(fullBucket);
        if (fullBucket.stack < 1)
        {
            heart.Withdraw(bucket, false);
            return true;
        }
        return false;
    }
    [JITWhenModsEnabled("MagicStorage")]
    public override bool WaterOut(out int liquidType)
    {
        liquidType = -1;
        if (heart.HasItem(new Item(ItemID.BottomlessBucket)))
            liquidType = 0;
        if (heart.HasItem(new Item(ItemID.BottomlessLavaBucket)))
            liquidType = LiquidID.Lava;
        if (heart.HasItem(new Item(ItemID.BottomlessHoneyBucket)))
            liquidType = LiquidID.Honey;
        if (heart.HasItem(new Item(ItemID.BottomlessShimmerBucket)))
            liquidType = LiquidID.Shimmer;
        if(liquidType != -1) return true;
        liquidType = LiquidID.Water;
        Item fullBucket = new Item(ItemID.WaterBucket);
        if (!heart.HasItem(fullBucket))
        {
            liquidType = LiquidID.Lava;
            fullBucket = new Item(ItemID.LavaBucket);
            if (!heart.HasItem(fullBucket))
            {
                liquidType = LiquidID.Honey;
                fullBucket = new Item(ItemID.HoneyBucket);
                if(!heart.HasItem(fullBucket))
                    return false;
            }
        }
        Item emptyBucket = new Item(ItemID.EmptyBucket);
        heart.DepositItem(emptyBucket);
        if (emptyBucket.stack < 1)
        {
            heart.Withdraw(fullBucket, false);
            return true;
        }
        return false;
    }
}
