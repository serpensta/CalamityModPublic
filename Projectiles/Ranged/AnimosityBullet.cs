using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Ranged
{
    internal class AnimosityBullet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Lighting
            Lighting.AddLight(Projectile.Center, 0.9f, 0f, 0.15f);

            // Dust
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 6f)
            {
                float scale = Main.rand.NextFloat(0.6f, 0.9f);
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, (int)CalamityDusts.Brimstone);
                Vector2 posOffset = Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;
                d.position += posOffset - 2f * Vector2.UnitY;
                d.noGravity = true;
                d.velocity *= 0.6f;
                d.velocity += Projectile.velocity * 0.15f;
                d.scale = scale;
            }
        }

        public override void Kill(int timeLeft)
        { 
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            //DesertProwelerSkullParticle is a placeholder
            Particle skull = new DesertProwlerSkullParticle(Projectile.Center, new Vector2(0f,Main.rand.NextFloat(0.5f,1f)), Color.Red, Color.DarkRed, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.Next(30, 50));
            GeneralParticleHandler.SpawnParticle(skull);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 120);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
