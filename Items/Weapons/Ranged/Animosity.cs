using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static CalamityMod.CalamityUtils;
using CalamityMod.Projectiles;
using System;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Animosity : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle ShootAndReloadSound = new("CalamityMod/Sounds/Item/WulfrumBlunderbussFireAndReload") { PitchVariance = 0.2f }; // Very cool sound and it would be a shame for it to not be used elsewhere
        
        // If stuff is here then DragonLens can easily detect it so it can change it for balancing
        private static float ShotgunBulletSpeed = 11.5f;
        private static float SniperBulletSpeed = 15f;
        private static float SniperDmgMult = 5.8f;
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Animosity");
            //Tooltip.SetDefault(@"Fires a powerful shotgun blast
//Right click to fire a powerful brimstone round
//All bullets fired from this gun are imbued with Brimstone Flames");
// These are temporary til I fix the localization
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 18;
            Item.scale = 0.85f;
            Item.useTime = 15;
            Item.reuseDelay = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = ShootAndReloadSound;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = ShotgunBulletSpeed;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = -18; // Explained later
            Item.Calamity().canFirePointBlankShots = true;
        }
        
        // Terraria dislikes high crit on SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 20; // 6% Shotgun mode, 32% Sniper Mode. This is after dealing with vanilla adding 4% more

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 40;
                Item.reuseDelay = 0;
                Item.useAnimation = 40;
                Item.shootSpeed = SniperBulletSpeed;
                Item.crit = 8;
            }
            else
            {
                Item.useTime = 18;
                Item.reuseDelay = 8;
                Item.useAnimation = 18;
                Item.shootSpeed = ShotgunBulletSpeed;
                Item.crit = -18;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                // TO DO: Replace with actual bullet shells, surprised no one has even tried this
                Gore.NewGore(source, position, velocity * Main.rand.NextFloat(-0.15f,-0.35f), Mod.Find<ModGore>("Polt5").Type);
            }

            if (player.altFunctionUse == 2)
            {
                // It should feel powerful but less than the shotgun
                if (player.Calamity().GeneralScreenShakePower < 3f)
                    player.Calamity().GeneralScreenShakePower = 1.5f;
                //Shoot from muzzle
                Vector2 baseVelocity = velocity.SafeNormalize(Vector2.Zero) * SniperBulletSpeed;
                Vector2 nuzzlePos = player.MountedCenter + baseVelocity * 4f;


                // TO DO: Make a custom big explosive bullet, it should actually look like a bullet being fired and not a line or whatever I'm using as a placeholder
                Projectile shot = Projectile.NewProjectileDirect(source, nuzzlePos, velocity, ProjectileID.RocketI, (int)(damage * SniperDmgMult), knockback, player.whoAmI, 0f, 0f);
                CalamityGlobalProjectile cgp = shot.Calamity();
                cgp.supercritHits = 1;
                cgp.brimstoneBullets = true;

            }
            else
            {
                // It should feel powerful
                if (player.Calamity().GeneralScreenShakePower < 3f)
                    player.Calamity().GeneralScreenShakePower = 3f;

                //Shoot from muzzle
                Vector2 baseVelocity = velocity.SafeNormalize(Vector2.Zero) * ShotgunBulletSpeed;
                Vector2 nuzzlePos = player.MountedCenter + baseVelocity * 4f;

                // Fire a shotgun spread of bullets.
                for (int i = 0; i < 6; ++i)
                {
                    float dx = Main.rand.NextFloat(-1.3f, 1.3f);
                    float dy = Main.rand.NextFloat(-1.3f, 1.3f);
                    Vector2 randomVelocity = baseVelocity + new Vector2(dx, dy);
                    Projectile shot = Projectile.NewProjectileDirect(source, nuzzlePos, randomVelocity, type, damage, knockback, player.whoAmI);
                    CalamityGlobalProjectile cgp = shot.Calamity();
                    cgp.brimstoneBullets = true;
                }
            }
            return false;
        }

        // Animation zone
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            // Only the shotgun gets upwards recoil
            if (player.altFunctionUse != 2)
            {
                player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);
                float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

                Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 35f;
                Vector2 itemSize = new Vector2(Item.width, Item.height);
                Vector2 itemOrigin = new Vector2(-5, 6);

                CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
                base.UseStyle(player, heldItemFrame);
            }
            // TO DO: Make Sniper have horizontal recoil
        }

        // Recoil + Not having the gun aim downwards
        public override void UseItemFrame(Player player)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            rotation += -0.45f * (float)Math.Pow((1f - animProgress), 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

            //Reloads the gun 
            if (animProgress < 0.4f)
            {
                float backArmRotation = rotation + 0.52f * player.direction;

                Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                player.SetCompositeArmBack(true, stretch, backArmRotation);
            }
        }
    }
}
