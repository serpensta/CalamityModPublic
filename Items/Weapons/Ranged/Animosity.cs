using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static CalamityMod.CalamityUtils;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Ranged;

using System;
using System.Collections.Generic;
using Terraria.Localization;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Animosity : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle ShootAndReloadSound = new("CalamityMod/Sounds/Item/WulfrumBlunderbussFireAndReload") { PitchVariance = 0.25f }; 
        // Very cool sound and it would be a shame for it to not be used elsewhere, would be even better if a new sound is made
        
        public static float SniperDmgMult = 3.5f;
        public static float SniperCritMult = 4f;
        public static float SniperVelocityMult = 3.5f;
         public new string LocalizationCategory => "Items.Weapons.Ranged";

        //ITS MY REWORK SO I CAN PUT A REFERENCE: Shotgun full of hate, returns Animosity otherwise
        public override LocalizedText DisplayName => Main.zenithWorld ? CalamityUtils.GetText("Items.Weapons.Ranged.AnimosityGfb") : GetText("Items.Weapons.Ranged.Animosity.DisplayName");

        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 18;
            Item.damage = 34;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 18;
            Item.scale = 0.85f;
            Item.useTime = 16;
            Item.reuseDelay = 10;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = ShootAndReloadSound;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 6.5f;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = 8;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        #region Stat changing
        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (player.altFunctionUse == 2)
            {
                crit *= SniperCritMult;
            }

        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.altFunctionUse == 2)
            {
                damage *= SniperDmgMult;
            }

        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1/2.5f;

            return 1f;
        }


        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
               Item.reuseDelay = 5;
            else
               Item.reuseDelay = 10;

            return base.CanUseItem(player);
        }
        #endregion

        #region Shooting
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //It should feel powerful but also not too much given feedback
            player.Calamity().GeneralScreenShakePower = 1f;

            if (player.altFunctionUse == 2)
            {
                //Shoot from muzzle
                Vector2 nuzzlePos = player.MountedCenter + velocity*4f;

                int p = Projectile.NewProjectile(source, nuzzlePos, velocity*SniperVelocityMult, ModContent.ProjectileType<AnimosityBullet>(), damage, knockback, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().supercritHits = 1;
                    Main.projectile[p].Calamity().brimstoneBullets = true;
                }


            }
            else
            {
                //Shoot from muzzle
                Vector2 nuzzlePos = player.MountedCenter + velocity*4f;

                // Fire a shotgun spread of bullets.
                for (int i = 0; i < 6; ++i)
                {
                    float dx = Main.rand.NextFloat(-1.3f, 1.3f);
                    float dy = Main.rand.NextFloat(-1.3f, 1.3f);
                    Vector2 randomVelocity = velocity + new Vector2(dx, dy);
                    Projectile shot = Projectile.NewProjectileDirect(source, nuzzlePos, randomVelocity, type, damage, knockback, player.whoAmI);
                    CalamityGlobalProjectile cgp = shot.Calamity();
                    cgp.brimstoneBullets = true; //add a brimstone trail to all bullets
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                // TO DO: Replace with actual bullet shells or used casings
                Gore.NewGore(source, position, velocity * Main.rand.NextFloat(-0.15f, -0.35f), Mod.Find<ModGore>("Polt5").Type);
            }
            return false;
        }
        #endregion

        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 35f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-5, 6);

            
            //Sniper's horizontal recoil; can be a bit subtle but it is noticeable
            if (player.altFunctionUse == 2)
            { 
                //Recoil:
                int anim = 0;
                for (int r = 0; r < Item.useAnimation; ++r)
                {
                    if (anim == 10 && r < Item.useAnimation / 2) //animates every 10 frames so that the player notices a recoil because this happens way too fast
                    {
                        itemPosition.X -= player.direction * 0.025f;
                        itemPosition.Y -= player.direction * 0.025f;
                        anim = 0;
                    }
                    else if (anim == 10 && r > Item.useAnimation / 2)
                    {
                        itemPosition.X += player.direction * 0.025f;
                        itemPosition.Y += player.direction * 0.025f;
                        anim = 0;
                    }
                    ++anim;
                }
            }

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        // Recoil + Not having the gun aim downwards
        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (player.altFunctionUse == 2 ? -1f : -0.45f) * (float)Math.Pow((0.5f - animProgress)/0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation); //must be here otherwise it will vibrate

            
            //Reloads the gun 
            if (animProgress > 0.5f)
            {
                float backArmRotation = rotation + 0.52f * player.direction;

                Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                player.SetCompositeArmBack(true, stretch, backArmRotation);
            }
            
        }
        #endregion
    }
}
