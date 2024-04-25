using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GenisisBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];

        public Color mainColor = Color.Cyan;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 31;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 75;
            Projectile.timeLeft = 30;
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 2 == 0 && (Projectile.ai[1] == 1 && time > 4f || time > 6f && Projectile.timeLeft > 4))
            {
                Particle spark = new GlowSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 17, 0.095f, mainColor, new Vector2(0.5f, 1.3f));
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Main.rand.NextBool())
            {
                Vector2 dustVel = new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.8f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel, Main.rand.NextBool(4) ? 264 : 66, dustVel, 0, default, Main.rand.NextFloat(0.9f, 1.2f));
                dust.noGravity = true;
                dust.color = Main.rand.NextBool() ? Color.Lerp(mainColor, Color.White, 0.5f) : mainColor;
            }

            time++;
        }
        public override void OnKill(int timeLeft)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(20);
            if (Projectile.ai[1] == 0)
            {
                for (int i = 0; i < numProj; i++)
                {
                    Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<GenisisBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 1f);
                    for (int k = 0; k < 3; k++)
                    {
                        Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, mainColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.9f, 0.6f, 30);
                        GeneralParticleHandler.SpawnParticle(blastRing);
                        Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.8f, 0.4f, 30);
                        GeneralParticleHandler.SpawnParticle(blastRing2);
                    }
                }
                for (int i = 0; i < 40; i++)
                {
                    Vector2 dustVel = new Vector2(13, 13).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.8f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel, Main.rand.NextBool(4) ? 264 : 66, dustVel, 0, default, Main.rand.NextFloat(0.9f, 1.2f));
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool() ? Color.Lerp(mainColor, Color.White, 0.5f) : mainColor;
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    Particle spark2 = new GlowSparkParticle(Projectile.Center + Main.rand.NextVector2Circular(6, 6), Projectile.velocity * Main.rand.NextFloat(0.5f, 2f), false, 8, 0.03f, mainColor, new Vector2(0.4f, 1.3f));
                    GeneralParticleHandler.SpawnParticle(spark2);

                    Vector2 dustVel = Projectile.velocity * Main.rand.NextFloat(0.5f, 2f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel, Main.rand.NextBool(4) ? 264 : 66, dustVel, 0, default, Main.rand.NextFloat(0.9f, 1.2f));
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool() ? Color.Lerp(mainColor, Color.White, 0.5f) : mainColor;
                }
            }
        }
    }
}
