using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class LightBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public Player Owner => Main.player[Projectile.owner];

        private const float MaxVelocity = DarklightGreatsword.ShootSpeed * 12f;

        private const int FadeOutTime = 85;

        private const int TimeLeft = 300 + FadeOutTime;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            AIType = ProjectileID.DeathSickle;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = TimeLeft;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.6f / 255f, 0f, (255 - Projectile.alpha) * 0.2f / 255f);

            if (Projectile.timeLeft > FadeOutTime)
            {
                if (Projectile.velocity.Length() < MaxVelocity)
                    Projectile.velocity *= 1.025f;
            }
            else
                Projectile.velocity *= 0.925f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < FadeOutTime)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > FadeOutTime)
                Projectile.timeLeft = FadeOutTime;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
            int slashCreatorID = ModContent.ProjectileType<DarklightGreatswordSlashCreator>();
            int damage = (int)(Projectile.damage * DarklightGreatsword.SlashProjectileDamageMultiplier);
            float knockback = Projectile.knockBack * DarklightGreatsword.SlashProjectileDamageMultiplier;
            if (Owner.ownedProjectileCounts[slashCreatorID] < DarklightGreatsword.SlashProjectileLimit)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, slashCreatorID, damage, knockback, Projectile.owner, target.whoAmI, Main.rand.NextFloat(MathHelper.TwoPi));
                Owner.ownedProjectileCounts[slashCreatorID]++;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire, 120);

        public override bool? CanDamage() => Projectile.timeLeft < FadeOutTime ? false : null;
    }
}
