using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class OmicronBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];

        public Color mainColor = Color.MediumVioletRed;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 75;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            if (Projectile.timeLeft % 2 == 0 && time > 2 && targetDist < 1400f)
            {
                Particle spark = new GlowSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 25, MathHelper.Clamp(0.34f - time * 0.07f, 0.085f, 0.34f), mainColor, new Vector2(0.5f, 1.3f));
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Main.rand.NextBool())
            {
                Vector2 trailPos = Projectile.Center;
                float trailScale = Main.rand.NextFloat(1.9f, 2.3f);
                Particle Trail = new SparkParticle(trailPos, Projectile.velocity * Main.rand.NextFloat(-0.6f, 0.6f), false, Main.rand.Next(40, 50 + 1), trailScale, mainColor);
                GeneralParticleHandler.SpawnParticle(Trail);
            }
            Vector2 dustVel = new Vector2(2, 2).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.8f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel, Main.rand.NextBool(4) ? 264 : 66, dustVel, 0, default, Main.rand.NextFloat(0.9f, 1.2f));
            dust.noGravity = true;
            dust.color = Main.rand.NextBool() ? Color.Lerp(mainColor, Color.White, 0.5f) : mainColor;

            time++;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 28; i++)
            {
                Vector2 dustVel = Projectile.velocity * Main.rand.NextFloat(0.1f, 1.5f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel + Main.rand.NextVector2Circular(6, 6), Main.rand.NextBool(4) ? 264 : 66, dustVel, 0, default, Main.rand.NextFloat(0.9f, 1.2f));
                dust.noGravity = true;
                dust.color = Main.rand.NextBool() ? Color.Lerp(mainColor, Color.White, 0.5f) : mainColor;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player Owner = Main.player[Projectile.owner];
            for (int i = 0; i <= 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(4) ? 264 : 66, (Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15f).RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.2f, 1.6f));
                dust.noGravity = true;
                dust.color = Main.rand.NextBool() ? Color.Lerp(mainColor, Color.White, 0.5f) : mainColor;
            }
            if (time <= 7) // This is the sweet spot
            {
                modifiers.SourceDamage *= 3;

                Owner.velocity += -Projectile.velocity * 2;

                for (int i = 0; i <= 9; i++)
                {
                    Particle energy = new GlowSparkParticle(Projectile.Center, (Projectile.velocity * 15).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.4f), false, 11, Main.rand.NextFloat(0.05f, 0.07f), mainColor, new Vector2(2, 0.5f), true);
                    GeneralParticleHandler.SpawnParticle(energy);
                }
                for (int i = 0; i <= 13; i++)
                {
                    Particle energy = new SparkParticle(Projectile.Center, (Projectile.velocity * 10).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.4f), false, 25, Main.rand.NextFloat(0.7f, 0.9f), mainColor);
                    GeneralParticleHandler.SpawnParticle(energy);
                }

                SoundStyle fire = new("CalamityMod/Sounds/Custom/ExoMechs/ArtemisApolloDash");
                SoundEngine.PlaySound(fire with { Volume = 1.25f, Pitch = 0.6f }, Projectile.Center);
            }
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.9f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, time <= 7 ? 90 : 50, targetHitbox);
    }
}
