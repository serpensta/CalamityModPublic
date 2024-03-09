using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("SoulEdge")]
    public class VoidEdge : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static readonly SoundStyle ProjectileDeathSound = SoundID.Item100 with { Volume = 0.5f };

        internal const int TotalProjectilesPerSwing = 3;

        internal const int ProjectileSpreadOutTime = 20;

        internal const float ShootSpeed = 20f;

        internal const float SmallSoulStatMultiplier = 0.8f;

        internal const float MediumSoulStatMultiplier = 0.9f;

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 88;
            Item.damage = 210;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GhastlySoulLarge>();
            Item.shootSpeed = ShootSpeed;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numShots = TotalProjectilesPerSwing;
            for (int i = 0; i < numShots; i++)
            {
                float speedX = velocity.X + (float)Main.rand.Next(-60, 61) * 0.05f;
                float speedY = velocity.Y + (float)Main.rand.Next(-60, 61) * 0.05f;
                velocity = new Vector2(speedX, speedY);
                float ai1 = MathHelper.Lerp(0.75f, 1.25f, Main.rand.NextFloat());
                switch (i)
                {
                    default:
                    case 0:
                        break;

                    case 1:
                        type = ModContent.ProjectileType<GhastlySoulMedium>();
                        velocity *= MediumSoulStatMultiplier;
                        knockback *= MediumSoulStatMultiplier;
                        break;

                    case 2:
                        type = ModContent.ProjectileType<GhastlySoulSmall>();
                        velocity *= SmallSoulStatMultiplier;
                        knockback *= SmallSoulStatMultiplier;
                        break;
                }
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, ai1);
                Main.projectile[proj].netUpdate = true;
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, MathHelper.Lerp(0.75f, 2.25f, Main.rand.NextFloat()));
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
    }
}
