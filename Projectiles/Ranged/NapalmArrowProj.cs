using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Ammo;
using CalamityMod.Dusts;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Ranged
{
    public class NapalmArrowProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Ammo/NapalmArrow";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.ai[1] > 4)
            {
                float velMulti = Main.rand.NextFloat(0.1f, 0.75f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 2, 90, -Projectile.velocity.RotatedBy(0.45) * velMulti);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.45f, 0.75f);
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 2, 90, -Projectile.velocity.RotatedBy(-0.45) * velMulti);
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.45f, 0.75f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int b = 0; b < 20; b++)
            {
                int dustType = Main.rand.NextBool() ? 90 : ModContent.DustType<BrimstoneFlame>();
                float velMulti = Main.rand.NextFloat(0.1f, 0.75f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, new Vector2(14, 14).RotatedByRandom(100) * velMulti);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.75f, 1.35f);
            }
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Crimson, new Vector2(2f, 2f), Main.rand.NextFloat(12f, 25f), 0.1f, 0.5f, 15);
            GeneralParticleHandler.SpawnParticle(pulse);
            SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.45f, Pitch = -0.3f, PitchVariance = 0.15f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item69 with { Volume = 0.35f, Pitch = 0.9f, PitchVariance = 0.15f }, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.damage = (int)(Projectile.damage * 0.3f);
                Projectile.penetrate = -1;
                Projectile.ExpandHitboxBy(110);
                Projectile.Damage();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/BloodfireBulletProj").Value;
            if (Projectile.ai[1] > 6)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], Color.White, 1, texture);
            return true;
        }
    }
}
