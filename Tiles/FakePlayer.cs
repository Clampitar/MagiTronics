using System;
using Terraria.Audio;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Golf;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;


namespace MagiTronics.Tiles
{
    internal class FakePlayer : Player
    {
        private bool dontConsumeWand;


        public void FakeCheck()
        {
            //IL_0f3e: Unknown result type (might be due to invalid IL or missing references)
            //IL_1157: Unknown result type (might be due to invalid IL or missing references)
            float heightOffsetHitboxCenter = this.HeightOffsetHitboxCenter;
            Item item = this.inventory[this.selectedItem];
            ItemCheckContext context = default(ItemCheckContext);
            bool flag = false;
            if (Main.myPlayer == base.whoAmI)
            {
                if (PlayerInput.ShouldFastUseItem)
                {
                    this.controlUseItem = true;
                    flag = true;
                }
                if (!this.cursorItemIconEnabled && item.stack > 0 && (item.type == 779 || item.type == 5134))
                {
                    for (int i = 54; i < 58; i++)
                    {
                        if (this.inventory[i].ammo == item.useAmmo && this.inventory[i].stack > 0)
                        {
                            this.cursorItemIconEnabled = true;
                            this.cursorItemIconID = this.inventory[i].type;
                            this.cursorItemIconPush = 10;
                            break;
                        }
                    }
                    if (!this.cursorItemIconEnabled)
                    {
                        for (int j = 0; j < 54; j++)
                        {
                            if (this.inventory[j].ammo == item.useAmmo && this.inventory[j].stack > 0)
                            {
                                this.cursorItemIconEnabled = true;
                                this.cursorItemIconID = this.inventory[j].type;
                                this.cursorItemIconPush = 10;
                                break;
                            }
                        }
                    }
                }
            }
            Item item2 = ((this.itemAnimation > 0) ? this.lastVisualizedSelectedItem : item);
            Rectangle drawHitbox = Item.GetDrawHitbox(item2.type, this);
            this.compositeFrontArm.enabled = false;
            this.compositeBackArm.enabled = false;
            if (this.itemAnimation > 0)
            {
                if (item.mana > 0)
                {
                    //this.ItemCheck_ApplyManaRegenDelay(item);
                }
                if (Main.dedServ)
                {
                    this.itemHeight = item.height;
                    this.itemWidth = item.width;
                }
                else
                {
                    this.itemHeight = drawHitbox.Height;
                    this.itemWidth = drawHitbox.Width;
                }
                this.itemAnimation--;
                if (this.itemAnimation == 0 && base.whoAmI == Main.myPlayer)
                {
                    PlayerInput.TryEndingFastUse();
                }
            }
            if (this.itemTime > 0)
            {
                this.itemTime--;
                if (this.ItemTimeIsZero && base.whoAmI == Main.myPlayer && !this.JustDroppedAnItem)
                {
                    int type = item.type;
                    if (type == 65 || type == 724 || type == 989 || type == 1226)
                    {
                        //this.EmitMaxManaEffect();
                    }
                }
            }
            this.ItemCheck_HandleMount();
            int weaponDamage = this.GetWeaponDamage(item);
            this.ItemCheck_HandleMPItemAnimation(item);
            this.ItemCheck_HackHoldStyles(item);
            if (this.itemAnimation < 0)
            {
                this.itemAnimation = 0;
            }
            if (this.itemTime < 0)
            {
                this.itemTime = 0;
            }
            if (this.itemAnimation == 0 && this.reuseDelay > 0)
            {
                this.ApplyReuseDelay();
            }
            this.UpdatePlacementPreview(item);
            if (this.itemAnimation == 0 && this.altFunctionUse == 2)
            {
                this.altFunctionUse = 0;
            }
            bool flag2 = true;
            if (this.gravDir == -1f && GolfHelper.IsPlayerHoldingClub(this))
            {
                flag2 = false;
            }
            if (flag2 && this.controlUseItem && this.releaseUseItem && this.itemAnimation == 0 && item.useStyle != 0)
            {
                if (this.altFunctionUse == 1)
                {
                    this.altFunctionUse = 2;
                }
                if (item.shoot == 0)
                {
                    this.itemRotation = 0f;
                }
                bool flag3 = this.ItemCheck_CheckCanUse(item);
                if (item.potion && flag3)
                {
                    this.ApplyPotionDelay(item);
                }
                if (item.mana > 0 && flag3 && base.whoAmI == Main.myPlayer && item.buffType != 0 && item.buffTime != 0)
                {
                    this.AddBuff(item.buffType, item.buffTime);
                }
                if (item.shoot <= 0 || !ProjectileID.Sets.MinionTargettingFeature[item.shoot] || this.altFunctionUse != 2)
                {
                    this.ItemCheck_ApplyPetBuffs(item);
                }
                if (base.whoAmI == Main.myPlayer && this.gravDir == 1f && item.mountType != -1 && this.mount.CanMount(item.mountType, this))
                {
                    this.mount.SetMount(item.mountType, this);
                }
                if (flag3)
                {
                    this.ItemCheck_StartActualUse(item);
                }
            }
            bool flag4 = this.controlUseItem;
            if (this.mount.Active && this.mount.Type == 8)
            {
                flag4 = this.controlUseItem || this.controlUseTile;
            }
            if (!flag4)
            {
                this.channel = false;
            }
            this.releaseUseItem = !this.controlUseItem;
            ItemLoader.HoldItem(item, this);
            if (this.itemAnimation > 0)
            {
                this.ItemCheck_ApplyUseStyle(heightOffsetHitboxCenter, item2, drawHitbox);
            }
            else
            {
                this.ItemCheck_ApplyHoldStyle(heightOffsetHitboxCenter, item2, drawHitbox);
            }
            if (!this.JustDroppedAnItem)
            {
                _ = base.whoAmI;
                _ = Main.myPlayer;
                this.ItemCheck_OwnerOnlyCode(ref context, item, weaponDamage, drawHitbox);
                if (this.ItemTimeIsZero && this.itemAnimation > 0)
                {
                    if (ItemLoader.UseItem(item, this) == true)
                    {
                        this.ApplyItemTime(item, 1f, false);
                    }
                    if (item.hairDye >= 0)
                    {
                        this.ApplyItemTime(item);
                        if (base.whoAmI == Main.myPlayer)
                        {
                            this.hairDye = item.hairDye;
                            NetMessage.SendData(4, -1, -1, null, base.whoAmI);
                        }
                    }
                    if (item.healLife > 0 || item.healMana > 0)
                    {
                        this.ApplyLifeAndOrMana(item);
                        this.ApplyItemTime(item);
                        if (Main.myPlayer == base.whoAmI && item.type == 126 && this.breath == 0)
                        {
                            AchievementsHelper.HandleSpecialEvent(this, 25);
                        }
                    }
                    if (item.buffType > 0)
                    {
                        if (base.whoAmI == Main.myPlayer && item.buffType != 90 && item.buffType != 27)
                        {
                            this.AddBuff(item.buffType, item.buffTime);
                        }
                        this.ApplyItemTime(item);
                    }
                    if (item.type == 678)
                    {
                        if (Main.getGoodWorld)
                        {
                            this.ApplyItemTime(item);
                            if (base.whoAmI == Main.myPlayer)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    int type2 = 0;
                                    int timeToAdd = 108000;
                                    switch (Main.rand.Next(18))
                                    {
                                        case 0:
                                            type2 = 16;
                                            break;
                                        case 1:
                                            type2 = 111;
                                            break;
                                        case 2:
                                            type2 = 114;
                                            break;
                                        case 3:
                                            type2 = 8;
                                            break;
                                        case 4:
                                            type2 = 105;
                                            break;
                                        case 5:
                                            type2 = 17;
                                            break;
                                        case 6:
                                            type2 = 116;
                                            break;
                                        case 7:
                                            type2 = 5;
                                            break;
                                        case 8:
                                            type2 = 113;
                                            break;
                                        case 9:
                                            type2 = 7;
                                            break;
                                        case 10:
                                            type2 = 6;
                                            break;
                                        case 11:
                                            type2 = 104;
                                            break;
                                        case 12:
                                            type2 = 115;
                                            break;
                                        case 13:
                                            type2 = 2;
                                            break;
                                        case 14:
                                            type2 = 9;
                                            break;
                                        case 15:
                                            type2 = 3;
                                            break;
                                        case 16:
                                            type2 = 117;
                                            break;
                                        case 17:
                                            type2 = 1;
                                            break;
                                    }
                                    this.AddBuff(type2, timeToAdd);
                                }
                            }
                        }
                        else
                        {
                            this.ApplyItemTime(item);
                            if (base.whoAmI == Main.myPlayer)
                            {
                                this.AddBuff(20, 216000);
                                this.AddBuff(22, 216000);
                                this.AddBuff(23, 216000);
                                this.AddBuff(24, 216000);
                                this.AddBuff(30, 216000);
                                this.AddBuff(31, 216000);
                                this.AddBuff(32, 216000);
                                this.AddBuff(33, 216000);
                                this.AddBuff(35, 216000);
                                this.AddBuff(36, 216000);
                                this.AddBuff(68, 216000);
                            }
                        }
                    }
                }
                if ((item.type == 50 || item.type == 3124 || item.type == 3199 || item.type == 5358) && this.itemAnimation > 0)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        Dust.NewDust(base.position, base.width, base.height, 15, 0f, 0f, 150, default(Color), 1.1f);
                    }
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                    }
                    else if (this.itemTime == this.itemTimeMax / 2)
                    {
                        for (int l = 0; l < 70; l++)
                        {
                            Dust.NewDust(base.position, base.width, base.height, 15, base.velocity.X * 0.5f, base.velocity.Y * 0.5f, 150, default(Color), 1.5f);
                        }
                        this.RemoveAllGrapplingHooks();
                        this.Spawn(PlayerSpawnContext.RecallFromItem);
                        for (int m = 0; m < 70; m++)
                        {
                            Dust.NewDust(base.position, base.width, base.height, 15, 0f, 0f, 150, default(Color), 1.5f);
                        }
                    }
                }
                if ((item.type == 4263 || item.type == 5360) && this.itemAnimation > 0)
                {
                    Vector2 vector = Vector2.UnitY.RotatedBy((float)this.itemAnimation * ((float)Math.PI * 2f) / 30f) * new Vector2(15f, 0f);
                    for (int n = 0; n < 2; n++)
                    {
                        if (Main.rand.Next(3) == 0)
                        {
                            Dust dust = Dust.NewDustPerfect(base.Bottom + vector, Dust.dustWater());
                            dust.velocity.Y *= 0f;
                            dust.velocity.Y -= 4.5f;
                            dust.velocity.X *= 1.5f;
                            dust.scale = 0.8f;
                            dust.alpha = 130;
                            dust.noGravity = true;
                            dust.fadeIn = 1.1f;
                        }
                    }
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                    }
                    else if (this.itemTime == item.useTime / 2)
                    {
                        if (Main.netMode == 0)
                        {
                            this.MagicConch();
                        }
                        else if (Main.netMode == 1 && base.whoAmI == Main.myPlayer)
                        {
                            NetMessage.SendData(73, -1, -1, null, 1);
                        }
                    }
                }
                if ((item.type == 4819 || item.type == 5361) && this.itemAnimation > 0)
                {
                    Vector2 vector2 = Vector2.UnitY.RotatedBy((float)this.itemAnimation * ((float)Math.PI * 2f) / 30f) * new Vector2(15f, 0f);
                    for (int num = 0; num < 2; num++)
                    {
                        if (Main.rand.Next(3) == 0)
                        {
                            Dust dust2 = Dust.NewDustPerfect(base.Bottom + vector2, 35);
                            dust2.velocity.Y *= 0f;
                            dust2.velocity.Y -= 4.5f;
                            dust2.velocity.X *= 1.5f;
                            dust2.scale = 0.8f;
                            dust2.alpha = 130;
                            dust2.noGravity = true;
                            dust2.fadeIn = 1.1f;
                        }
                    }
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                    }
                    else if (this.itemTime == item.useTime / 2)
                    {
                        if (Main.netMode == 0)
                        {
                            this.DemonConch();
                        }
                        else if (Main.netMode == 1 && base.whoAmI == Main.myPlayer)
                        {
                            NetMessage.SendData(73, -1, -1, null, 2);
                        }
                    }
                }
                if (item.type == 5359 && this.itemAnimation > 0)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        int num8 = Main.rand.Next(4);
                        Color color = Color.Green;
                        switch (num8)
                        {
                            case 0:
                            case 1:
                                color = new Color(100, 255, 100);
                                break;
                            case 2:
                                color = Color.Yellow;
                                break;
                            case 3:
                                color = Color.White;
                                break;
                        }
                        Dust dust3 = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(base.Hitbox), 267);
                        dust3.noGravity = true;
                        dust3.color = color;
                        dust3.velocity *= 2f;
                        dust3.scale = 0.8f + Main.rand.NextFloat() * 0.6f;
                    }
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                    }
                    else if (this.itemTime == item.useTime / 2)
                    {
                        if (Main.netMode == 0)
                        {
                            this.Shellphone_Spawn();
                        }
                        else if (Main.netMode == 1 && base.whoAmI == Main.myPlayer)
                        {
                            NetMessage.SendData(73, -1, -1, null, 3);
                        }
                    }
                }
                if (item.type == 2350 && this.itemAnimation > 0)
                {
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                        SoundEngine.PlaySound(in SoundID.Item3, base.position);
                        for (int num9 = 0; num9 < 10; num9++)
                        {
                            Main.dust[Dust.NewDust(base.position, base.width, base.height, 15, base.velocity.X * 0.2f, base.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
                        }
                    }
                    else if (this.itemTime == 20)
                    {
                        SoundEngine.PlaySound(this.HeldItem.UseSound, base.position);
                        for (int num10 = 0; num10 < 70; num10++)
                        {
                            Main.dust[Dust.NewDust(base.position, base.width, base.height, 15, base.velocity.X * 0.2f, base.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
                        }
                        this.RemoveAllGrapplingHooks();
                        bool flag5 = this.immune;
                        int num11 = this.immuneTime;
                        this.Spawn(PlayerSpawnContext.RecallFromItem);
                        this.immune = flag5;
                        this.immuneTime = num11;
                        for (int num12 = 0; num12 < 70; num12++)
                        {
                            Main.dust[Dust.NewDust(base.position, base.width, base.height, 15, 0f, 0f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
                        }
                        if (ItemLoader.ConsumeItem(item, this) && item.stack > 0)
                        {
                            item.stack--;
                        }
                    }
                }
                if (item.type == 4870 && this.itemAnimation > 0)
                {
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                        SoundEngine.PlaySound(in SoundID.Item3, base.position);
                        for (int num13 = 0; num13 < 10; num13++)
                        {
                            Main.dust[Dust.NewDust
                                (base.position, base.width, base.height, 15, base.velocity.X * 0.2f, base.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
                        }
                    }
                    else if (this.itemTime == 20)
                    {
                        SoundEngine.PlaySound(this.HeldItem.UseSound, base.position);
                        for (int num14 = 0; num14 < 70; num14++)
                        {
                            Main.dust[Dust.NewDust(
                                base.position, base.width, base.height, 15, base.velocity.X * 0.2f, base.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
                        }
                        if (base.whoAmI == Main.myPlayer)
                        {
                            this.DoPotionOfReturnTeleportationAndSetTheComebackPoint();
                        }
                        for (int num15 = 0; num15 < 70; num15++)
                        {
                            Main.dust[Dust.NewDust(base.position, base.width, base.height, 15, 0f, 0f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
                        }
                        if (ItemLoader.ConsumeItem(item, this) && item.stack > 0)
                        {
                            item.stack--;
                        }
                    }
                }
                if (item.type == 2351 && this.itemAnimation > 0)
                {
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                    }
                    else if (this.itemTime == 2)
                    {
                        if (Main.netMode == 0)
                        {
                            this.TeleportationPotion();
                        }
                        else if (Main.netMode == 1 && base.whoAmI == Main.myPlayer)
                        {
                            NetMessage.SendData(73);
                        }
                        if (ItemLoader.ConsumeItem(item, this) && item.stack > 0)
                        {
                            item.stack--;
                        }
                    }
                }
                if (item.type == 2756 && this.itemAnimation > 0)
                {
                    if (this.ItemTimeIsZero)
                    {
                        this.ApplyItemTime(item);
                    }
                    else if (this.itemTime == 2)
                    {
                        if (base.whoAmI == Main.myPlayer)
                        {
                            this.Male = !this.Male;
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(4, -1, -1, null, base.whoAmI);
                            }
                        }
                        if (ItemLoader.ConsumeItem(item, this) && item.stack > 0)
                        {
                            item.stack--;
                        }
                    }
                    else
                    {
                        float num2 = this.itemTimeMax;
                        num2 = (num2 - (float)this.itemTime) / num2;
                        float num3 = 44f;
                        float num4 = (float)Math.PI * 3f;
                        Vector2 vector3 = new Vector2(15f, 0f).RotatedBy(num4 * num2);
                        vector3.X *= base.direction;
                        for (int num5 = 0; num5 < 2; num5++)
                        {
                            int type3 = 221;
                            if (num5 == 1)
                            {
                                vector3.X *= -1f;
                                type3 = 219;
                            }
                            Vector2 vector4 = new Vector2(vector3.X, num3 * (1f - num2) - num3 + (float)(base.height / 2));
                            vector4 += base.Center;
                            int num6 = Dust.NewDust(vector4, 0, 0, type3, 0f, 0f, 100);
                            Main.dust[num6].position = vector4;
                            Main.dust[num6].noGravity = true;
                            Main.dust[num6].velocity = Vector2.Zero;
                            Main.dust[num6].scale = 1.3f;
                            Main.dust[num6].customData = this;
                        }
                    }
                }
                if (base.whoAmI == Main.myPlayer)
                {
                    if ((this.itemTimeMax != 0 && this.itemTime == this.itemTimeMax)
                        | (!item.IsAir && item.IsNotSameTypePrefixAndStack(this.lastVisualizedSelectedItem)))
                    {
                        this.lastVisualizedSelectedItem = item.Clone();
                    }
                }
                else
                {
                    this.lastVisualizedSelectedItem = item.Clone();
                }
                if (base.whoAmI == Main.myPlayer)
                {
                    if (!this.dontConsumeWand && this.itemTimeMax != 0 && this.itemTime == this.itemTimeMax && item.tileWand > 0)
                    {
                        int tileWand = item.tileWand;
                        for (int num7 = 0; num7 < 58; num7++)
                        {
                            if (tileWand == this.inventory[num7].type && this.inventory[num7].stack > 0)
                            {
                                if (ItemLoader.ConsumeItem(this.inventory[num7], this))
                                {
                                    this.inventory[num7].stack--;
                                }
                                if (this.inventory[num7].stack <= 0)
                                {
                                    this.inventory[num7] = new Item();
                                }
                                break;
                            }
                        }
                    }
                    if (this.itemTimeMax != 0 && this.itemTime == this.itemTimeMax && item.consumable && !context.SkipItemConsumption)
                    {
                        bool flag6 = true;
                        if (item.CountsAsClass(DamageClass.Throwing))
                        {
                            if (this.ThrownCost50 && Main.rand.Next(100) < 50)
                            {
                                flag6 = false;
                            }
                            if (this.ThrownCost33 && Main.rand.Next(100) < 33)
                            {
                                flag6 = false;
                            }
                        }
                        if (item.IsACoin)
                        {
                            flag6 = true;
                        }
                        bool? flag7 = ItemID.Sets.ForceConsumption[item.type];
                        if (flag7.HasValue)
                        {
                            flag6 = flag7.Value;
                        }
                        if (flag6 && ItemLoader.ConsumeItem(item, this))
                        {
                            if (item.stack > 0)
                            {
                                item.stack--;
                            }
                            if (item.stack <= 0)
                            {
                                this.itemTime = this.itemAnimation;
                                Main.blockMouse = true;
                            }
                        }
                    }
                    if (item.stack <= 0 && this.itemAnimation == 0)
                    {
                        this.inventory[this.selectedItem] = new Item();
                    }
                    if (this.selectedItem == 58 && this.itemAnimation != 0)
                    {
                        Main.mouseItem = item.Clone();
                    }
                }
            }
            if (this.itemAnimation == 0)
            {
                this.JustDroppedAnItem = false;
            }
            if (base.whoAmI == Main.myPlayer && flag)
            {
                PlayerInput.TryEndingFastUse();
            }
        }

        private void ApplyLifeAndOrMana(Item item)
        {
            //throw new NotImplementedException();
        }

        private void ItemCheck_OwnerOnlyCode(ref ItemCheckContext context, Item sItem, int weaponDamage, Rectangle heldItemFrame)

        {
            bool flag = true;
            int type = sItem.type;
            if ((type == 65 || type == 676 || type == 723 || type == 724 || type == 757 || type == 674 || type == 675 
                || type == 989 || type == 1226 || type == 1227) && !this.ItemAnimationJustStarted)
            {
                flag = false;
            }
            if (sItem.useLimitPerAnimation.HasValue && this.ItemUsesThisAnimation >= sItem.useLimitPerAnimation.Value)
            {
                flag = false;
            }
            //this.ItemCheck_TurretAltFeatureUse(sItem, flag);
            //this.ItemCheck_MinionAltFeatureUse(sItem, flag);
            bool flag2 = this.itemAnimation > 0 && this.ItemTimeIsZero && flag;
            if (sItem.shootsEveryUse)
            {
                flag2 = this.ItemAnimationJustStarted;
            }
            if (base.whoAmI != Main.myPlayer)
            {
                return;
            }
            if (!this.channel)
            {
                this.toolTime = this.itemTime;
            }
            else
            {
                this.toolTime--;
                if (this.toolTime < 0)
                {
                    this.toolTime = sItem.useTime;
                }
            }
            //this.ItemCheck_TryDestroyingDrones(sItem);
            this.ItemCheck_UseMiningTools(sItem);
            /*this.ItemCheck_UseTeleportRod(sItem);
            this.ItemCheck_UseLifeCrystal(sItem);
            this.ItemCheck_UseLifeFruit(sItem);
            this.ItemCheck_UseManaCrystal(sItem);
            this.ItemCheck_UseDemonHeart(sItem);
            this.ItemCheck_UseMinecartPowerUp(sItem);
            this.ItemCheck_UseTorchGodsFavor(sItem);
            this.ItemCheck_UseArtisanLoaf(sItem);
            this.ItemCheck_UseEventItems(sItem);
            this.ItemCheck_UseBossSpawners(base.whoAmI, sItem);
            this.ItemCheck_UseCombatBook(sItem);
            this.ItemCheck_UsePeddlersSatchel(sItem);
            this.ItemCheck_UsePetLicenses(sItem);
            this.ItemCheck_UseShimmerPermanentItems(sItem);*/
            if (sItem.type == 4095 && this.itemAnimation == 2)
            {
                Main.LocalGolfState.ResetGolfBall();
            }
            this.PlaceThing(ref context);
            /*if (sItem.makeNPC > 0)
            {
                if (!Main.GamepadDisableCursorItemIcon && base.position.X / 16f - (float)Player.tileRangeX - (float)sItem.tileBoost <= (float)Player.tileTargetX && (base.position.X + (float)base.width) / 16f 
            + (float)Player.tileRangeX + (float)sItem.tileBoost - 1f >= (float)Player.tileTargetX && base.position.Y / 16f - (float)Player.tileRangeY - (float)sItem.tileBoost <= (float)Player.tileTargetY 
            && (base.position.Y + (float)base.height) / 16f + (float)Player.tileRangeY + (float)sItem.tileBoost - 2f >= (float)Player.tileTargetY)
                {
                    this.cursorItemIconEnabled = true;
                    Main.ItemIconCacheUpdate(sItem.type);
                }
                if (this.ItemTimeIsZero && this.itemAnimation > 0 && this.controlUseItem)
                {
                    this.ItemCheck_ReleaseCritter(sItem);
                }
            }*/
            /*if (this.boneGloveItem != null && !this.boneGloveItem.IsAir && this.boneGloveTimer == 0 && this.itemAnimation > 0 && sItem.damage > 0)
            {
                this.boneGloveTimer = 60;
                Vector2 center = base.Center;
                Vector2 vector = base.DirectionTo(this.ApplyRangeCompensation(0.2f, center, Main.MouseWorld)) * 10f;
                Projectile.NewProjectile(this.GetProjectileSource_Accessory(this.boneGloveItem), center.X, center.Y, vector.X, vector.Y, 532, 25, 5f, base.whoAmI);
            }*/
            if (((sItem.damage < 0 || sItem.type <= 0 || sItem.noMelee) && sItem.type != 1450 && !ItemID.Sets.CatchingTool[sItem.type] && sItem.type != 3542 && sItem.type != 3779) || this.itemAnimation <= 0)
            {
                return;
            }
            /*this.ItemCheck_GetMeleeHitbox(sItem, heldItemFrame, out var dontAttack, out var itemRectangle);
            if (!dontAttack)
            {
                itemRectangle = this.ItemCheck_EmitUseVisuals(sItem, itemRectangle);
                if (Main.myPlayer == base.whoAmI && ItemID.Sets.CatchingTool[sItem.type])
                {
                    itemRectangle = this.ItemCheck_CatchCritters(sItem, itemRectangle);
                }
                if (sItem.type == 3183 || sItem.type == 4821)
                {
                    bool[] shouldIgnore = this.ItemCheck_GetTileCutIgnoreList(sItem);
                    this.ItemCheck_CutTiles(sItem, itemRectangle, shouldIgnore);
                }
                if (sItem.damage > 0)
                {
                    this.UpdateMeleeHitCooldowns();
                    float knockBack = this.GetWeaponKnockback(sItem, sItem.knockBack);
                    bool[] shouldIgnore2 = this.ItemCheck_GetTileCutIgnoreList(sItem);
                    this.ItemCheck_CutTiles(sItem, itemRectangle, shouldIgnore2);
                    this.ItemCheck_MeleeHitNPCs(sItem, itemRectangle, weaponDamage, knockBack);
                    this.ItemCheck_MeleeHitPVP(sItem, itemRectangle, weaponDamage, knockBack);
                    this.ItemCheck_EmitHammushProjectiles(base.whoAmI, sItem, itemRectangle, weaponDamage);
                }
            }*/
        }

        private struct SpecialToolUsageSettings
        {
            public delegate bool CanUseToolCondition(Player user, Item item, int targetX, int targetY);

            public delegate void UseToolAction(Player user, Item item, int targetX, int targetY);

            public bool IsAValidTool;

            public CanUseToolCondition UsageCondition;

            public UseToolAction UsageAction;
        }

        private void ItemCheck_UseMiningTools(Item sItem)
        {
            SpecialToolUsageSettings specialToolUsageSettings = default(SpecialToolUsageSettings);
            if (sItem.type == 4711)
            {
                SpecialToolUsageSettings specialToolUsageSettings2 = default(SpecialToolUsageSettings);
                specialToolUsageSettings2.IsAValidTool = true;
                //specialToolUsageSettings2.UsageAction = UseShovel;
                specialToolUsageSettings = specialToolUsageSettings2;
            }
            if (sItem.pick <= 0 && sItem.axe <= 0 && sItem.hammer <= 0 && !specialToolUsageSettings.IsAValidTool)
            {
                return;
            }
            bool flag = this.RangeCheck(sItem);
            if (this.noBuilding)
            {
                flag = false;
            }
            if (flag && specialToolUsageSettings.UsageCondition != null)
            {
                flag = specialToolUsageSettings.UsageCondition(this, sItem, Player.tileTargetX, Player.tileTargetY);
            }
            if (this.toolTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
            {
                Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
                if (!tile.HasTile || (this.IsTilePoundable(tile) && !TileID.Sets.CanBeSloped[tile.TileType]))
                {
                    this.poundRelease = false;
                }
            }
            if (!flag)
            {
                return;
            }
            if (!Main.GamepadDisableCursorItemIcon)
            {
                this.cursorItemIconEnabled = true;
                Main.ItemIconCacheUpdate(sItem.type);
            }
            bool canHitWalls = false;
            if (this.toolTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
            {
                if (specialToolUsageSettings.UsageAction != null)
                {
                    specialToolUsageSettings.UsageAction(this, sItem, Player.tileTargetX, Player.tileTargetY);
                    return;
                }
                //this.ItemCheck_UseMiningTools_ActuallyUseMiningTool(sItem, out canHitWalls, Player.tileTargetX, Player.tileTargetY);
            }
            if (this.releaseUseItem)
            {
                this.poundRelease = true;
            }
            /*if (this.toolTime == 0 && this.itemAnimation > 0 && this.controlUseItem && canHitWalls)
            {
                Player.ItemCheck_UseMiningTools_TryFindingWallToHammer(out var wX, out var wY);
                this.ItemCheck_UseMiningTools_TryHittingWall(sItem, wX, wY);
            }*/
        }

        private void ItemCheck_EmitDrinkParticles(Item item)
        {
            //throw new NotImplementedException();
        }

        private void ItemCheck_ApplyHoldStyle(float heightOffsetHitboxCenter, Item item2, Rectangle drawHitbox)
        {
            //throw new NotImplementedException();
        }

        private void ItemCheck_StartActualUse(Item sItem)
        {
            bool flag = sItem.type == 4711;
            if (sItem.pick > 0 || sItem.axe > 0 || sItem.hammer > 0 || flag)
            {
                this.toolTime = 1;
            }
            if (this.grappling[0] > -1)
            {
                this.pulley = false;
                this.pulleyDir = 1;
                if (this.controlRight)
                {
                    base.direction = 1;
                }
                else if (this.controlLeft)
                {
                    base.direction = -1;
                }
            }
            this.StartChanneling(sItem);
            this.attackCD = 0;
            this.ResetMeleeHitCooldowns();
            this.ApplyItemAnimation(sItem);
            bool flag2 = ItemID.Sets.SkipsInitialUseSound[sItem.type];
            if (sItem.UseSound.HasValue && !flag2)
            {
                SoundEngine.PlaySound(sItem.UseSound, base.Center);
            }
        }

        private void ItemCheck_ApplyPetBuffs(Item item)
        {
            //throw new NotImplementedException();
        }

        private void ApplyPotionDelay(Item item)
        {
            //throw new NotImplementedException();
        }

        private bool ItemCheck_CheckCanUse(Item item)
        {
            //throw new NotImplementedException();
            return true;
        }

        private void UpdatePlacementPreview(Item item)
        {
            //throw new NotImplementedException();
        }

        private void ApplyReuseDelay()
        {
            //throw new NotImplementedException();
        }

        private void ItemCheck_HackHoldStyles(Item item)
        {
            //throw new NotImplementedException();
        }

        private void ItemCheck_HandleMPItemAnimation(Item item)
        {
            //throw new NotImplementedException();
        }

        private void ItemCheck_HandleMount()
        {
            //throw new NotImplementedException();
        }

        private bool IsTilePoundable(Tile targetTile)
        {
            if (!Main.tileHammer[targetTile.TileType] && !Main.tileSolid[targetTile.TileType] && targetTile.TileType != 314 &&
                targetTile.TileType != 424 && targetTile.TileType != 442)
            {
                return targetTile.TileType != 351;
            }
            return false;
        }

        public bool RangeCheck(Item sItem)
        {
            if ((position.X / 16f) - tileRangeX - sItem.tileBoost <= tileTargetX)
            {
                if ((position.X + width) / 16f + tileRangeX + sItem.tileBoost - 1f >= tileTargetX
)
                {
                    if ((position.Y / 16f) - tileRangeY - sItem.tileBoost <= tileTargetY)
                    {
                        return (position.Y + height) / 16f + tileRangeY + sItem.tileBoost - 2f >= tileTargetY;
                    }
                }
            }

            return false;
        }
    }
}
