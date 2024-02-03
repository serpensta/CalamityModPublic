using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using System.IO;
using Terraria.DataStructures;
using static CalamityMod.Items.Weapons.Ranged.PolarisParrotfish;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Boss;

namespace CalamityMod.Projectiles.Ranged
{
    public class PolarStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/PolarStar";

        public ref float Time => ref Projectile.ai[0];

        public int dustTypeColored = 66;
        public int dustTypeWhite = 91;
        public int tileBounces = 0;
        public float DustScaleMultiplier = 1;
        public bool DoSlowdown = true;
        public Vector2 StartVelocity;
        public Color EffectsColor;
        public int DustEffectsID;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 65;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Time == 1)
            {
                StartVelocity = Projectile.velocity;
                switch (Projectile.ai[1])
                {
                    case 2: // Orange shot
                        DustEffectsID = ModContent.DustType<AstralOrange>();
                        EffectsColor = Color.Coral;
                        DustScaleMultiplier = 1;
                        break;
                    case 1: // Blue shot
                        DustEffectsID = ModContent.DustType<AstralBlue>();
                        EffectsColor = Color.Turquoise;
                        DustScaleMultiplier = 1;
                        break;
                    default: // Pink shot
                        DustEffectsID = 223;
                        EffectsColor = Color.Violet;
                        DustScaleMultiplier = 0.4f;
                        break;
                }

                for (int i = 0; i <= 10; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(5) ? dustTypeWhite : DustEffectsID, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.3f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.85f, 1.4f) * DustScaleMultiplier;
                }
                for (int i = 0; i <= 2; i++)
                {
                    SquishyLightParticle energy = new(Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(0.2f, 0.6f), EffectsColor, Main.rand.Next(40, 50 + 1), 0.25f, 2f);
                    GeneralParticleHandler.SpawnParticle(energy);
                }
            }

            if (Time % 3 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15, 15), Main.rand.NextBool(5) ? dustTypeWhite : DustEffectsID);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 1.2f) * DustScaleMultiplier;
                dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.01f, 0.045f);
            }

            if (Projectile.timeLeft % 2 == 0 && Time > 4 && DoSlowdown)
            {
                PointParticle spark = new PointParticle(Projectile.Center - Projectile.velocity * 1.5f, -Projectile.velocity * 0.01f, false, 6, 2f, EffectsColor * 0.6f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Projectile.timeLeft <= 2 && DoSlowdown)
            {
                Projectile.timeLeft = Main.zenithWorld ? 3 : 7;
                Projectile.velocity = Vector2.Zero;
                DoSlowdown = false;
            }

            if (!DoSlowdown)
            {
                Projectile.extraUpdates = 0;
                Projectile.alpha = 255;
            }

            Lighting.AddLight(Projectile.Center, EffectsColor.ToVector3() * 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>(Texture).Value;

            Color drawColor = EffectsColor;
            //CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], drawColor * 0.7f, 1, texture);

            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (DoSlowdown)
            {
                Projectile.timeLeft = Main.zenithWorld ? 3 : 7;
                Projectile.velocity = Vector2.Zero;
                DoSlowdown = false;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (tileBounces >= 4)
                Projectile.Kill();
            else
                tileBounces++;

            for (int i = 0; i <= 3; i++)
            {
                SquishyLightParticle energy = new(Projectile.Center, Projectile.velocity.RotatedByRandom(100) * Main.rand.NextFloat(0.04f, 0.08f), Main.rand.NextFloat(0.2f, 0.6f), EffectsColor, Main.rand.Next(40, 50 + 1), 0.25f, 2f);
                GeneralParticleHandler.SpawnParticle(energy);
            }
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            int points = 5;
            float radians = MathHelper.TwoPi / points;
            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
            Vector2 addedPlacement = StartVelocity * Main.rand.NextFloat(0.1f, 1.5f);
            for (int k = 0; k < points; k++)
            {
                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);
                PointParticle spark = new PointParticle((Projectile.Center + velocity * 7.5f) + addedPlacement, velocity * 2.5f, false, 13, 1.2f, EffectsColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
        public override bool? CanDamage() => Projectile.numHits > 1 ? false : null;
    }
}
