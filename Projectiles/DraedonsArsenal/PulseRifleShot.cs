using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseRifleShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Color mainColor = Color.Orchid;
        private bool notSplit;
        private bool doDamage = false;
        private NPC closestTarget = null;
        private NPC lastTarget = null;
        private float distance;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            notSplit = Projectile.ai[1] == 0f;

            Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0.5f);
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            Projectile.rotation = Projectile.velocity.ToRotation();

            float createDustVar = 10f;

            // Set split projectile stats
            if (Projectile.localAI[0] == 0 && !notSplit)
            {
                Projectile.extraUpdates = 1;
                Projectile.timeLeft = 1240;
                Projectile.penetrate = 3;
                if (Projectile.ai[1] == 1)
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal with { Volume = 1.4f, Pitch = 0.6f }, Projectile.Center);
            }
            if (notSplit)
            {
                doDamage = true;
                Projectile.extraUpdates = 100;
            }

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar && notSplit)
            {
                float sizeBonus = MathHelper.Clamp(Projectile.localAI[0] * 0.009f, 0, 3.5f);
                if (targetDist < 1400f)
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center, -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 30, 1.1f - Projectile.ai[1] + sizeBonus, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (targetDist < 1400f && Main.rand.NextBool())
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(-2f, 2f), -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 30, 1.1f - Projectile.ai[1] + sizeBonus, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (Projectile.localAI[0] > 20)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), 66);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.7f);
                    dust.velocity = Projectile.velocity * Main.rand.NextFloat(-3, 3);
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                    dust.noLight = true;
                }
            }

            #region Split Projectile AI
            if (!notSplit)
            {
                if (targetDist < 1400f)
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center, -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 5, 1.7f - Projectile.ai[1] * 0.18f, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (targetDist < 1400f && Main.rand.NextBool())
                {
                    GlowOrbParticle spark = new GlowOrbParticle(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(-2f, 2f), -Projectile.velocity * Main.rand.NextFloat(-0.01f, 0.01f), false, 5, 1.7f - Projectile.ai[1] * 0.18f, Main.rand.NextBool(3) ? Color.DarkViolet : mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (Projectile.localAI[0] > 90)
                {
                    if (Projectile.localAI[0] == 91)
                        Projectile.velocity *= 0.001f;

                    if (Projectile.localAI[0] == 120)
                    {
                        doDamage = true;
                        distance = 1500;
                        if (Projectile.ai[1] == 1)
                        {
                            SoundStyle fire = new("CalamityMod/Sounds/Item/OpalFire");
                            SoundEngine.PlaySound(fire with { Volume = 0.35f, Pitch = 1f }, Projectile.Center);
                        }
                            

                        Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, mainColor, new Vector2(1f, 1f), Main.rand.NextFloat(12f, 25f), 0f, 0.5f, 15);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        for (int k = 0; k < 6; k++)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, 66);
                            dust.scale = Main.rand.NextFloat(0.6f, 1.1f);
                            dust.velocity = new Vector2(6, 6).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 0.8f);
                            dust.noGravity = true;
                            dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                            dust.noLight = true;
                        }
                    }
                    if (Projectile.localAI[0] > 120)
                    {
                        Projectile.extraUpdates = 5;
                        NPC prevTarg = null;
                        {
                            for (int index = 0; index < Main.npc.Length; index++)
                            {
                                if (Main.npc[index].CanBeChasedBy(null, false) || Main.npc[index] == lastTarget)
                                {
                                    float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                                    if (Vector2.Distance(Projectile.Center, Main.npc[index].Center) < (distance + extraDistance))
                                    {
                                        closestTarget = prevTarg == null ? Main.npc[index] : Main.npc[index] == lastTarget ? prevTarg : Main.npc[index];
                                        if (closestTarget == prevTarg && closestTarget == lastTarget)
                                            closestTarget = null;
                                        prevTarg = closestTarget;

                                        distance = Vector2.Distance(Projectile.Center, Main.npc[index].Center);
                                    }
                                }
                            }
                        }

                        float projectileSpeed = 9.5f;
                        if (closestTarget is not null)
                        {
                            float targetDirectionRotation = Projectile.SafeDirectionTo(closestTarget.Center).ToRotation();
                            float turningRate = 10f + Projectile.localAI[0] * 0.00008f;
                            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetDirectionRotation, turningRate).ToRotationVector2() * projectileSpeed;
                        }
                        else
                        {
                            Projectile.velocity = Projectile.rotation.ToRotationVector2() * projectileSpeed;
                            Projectile.velocity *= 0.99f;
                            Projectile.timeLeft--;
                        }
                    }
                }
                else
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), 66);
                        dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f);
                        dust.noGravity = true;
                        dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                        dust.noLight = true;
                    }
                    Projectile.velocity *= 0.98f;
                }
            }
            #endregion

            // Visuals for the shot as it exits the tip of the rifle
            if (Projectile.localAI[0] == createDustVar && notSplit)
                PulseBurst(4f, 5f);
        }

        public override bool? CanHitNPC(NPC target) => doDamage ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool onKill = target.life <= 0;
            if (notSplit)
            {
                Projectile.Kill();

                for (int i = 0; i <= 15; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 66);
                    dust.scale = Main.rand.NextFloat(0.4f, 1.1f);
                    dust.velocity = (Projectile.velocity * 4).RotateRandom(0.6f) * Main.rand.NextFloat(0.2f, 0.9f);
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                    dust.noLight = true;
                }

                float sizeBonus = MathHelper.Clamp(Projectile.localAI[0] * 0.01f, 0, 3.5f);
                for (int k = 0; k < 9; k++)
                {
                    SparkParticle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8) - Projectile.velocity * 7, Projectile.velocity * Main.rand.NextFloat(0.7f, 3.1f), false, 30, 0.9f * Main.rand.NextFloat(0.9f, 1.1f) + sizeBonus, mainColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                int numProj = 4;
                for (int i = 0; i < numProj; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.velocity.SafeNormalize(Vector2.Zero) * (14f - i * 0.7f)) * ((i + 1) * 0.25f), ModContent.ProjectileType<PulseRifleShot>(), (int)(Projectile.damage * 0.35), Projectile.knockBack, Projectile.owner, 0f, 1f + i);
                }
            }
            else
            {
                lastTarget = target;
                distance = 1500;
                Projectile.localAI[0] = 60;
                Projectile.velocity *= MathHelper.Clamp(1.5f - Projectile.numHits * 0.5f, 1f, 1.5f);

                for (int i = 0; i <= 3; i++)
                {
                    SquishyLightParticle energy = new SquishyLightParticle(Projectile.Center, (Projectile.velocity * 2).RotatedByRandom(0.7f) * Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(0.2f, 0.5f), Main.rand.NextBool(3) ? Color.DarkViolet : mainColor, Main.rand.Next(20, 30 + 1), 0.25f, 2f);
                    GeneralParticleHandler.SpawnParticle(energy);
                }

                if (onKill)
                {
                    Projectile.penetrate++;
                }

                if (hit.Damage <= 2)
                    Projectile.penetrate++;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (notSplit)
            {

            }
            else
            {
                //SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
            }
        }

        private void PulseBurst(float speed1, float speed2)
        {
            for (int i = 0; i <= 25; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 66);
                dust.scale = Main.rand.NextFloat(0.4f, 1.4f);
                dust.velocity = (Projectile.velocity * 4).RotateRandom(0.6f) * Main.rand.NextFloat(0.2f, 0.9f);
                dust.noGravity = true;
                dust.color = Main.rand.NextBool(3) ? Color.DarkViolet : mainColor;
                dust.noLight = true;
            }
            for (int i = 0; i <= 15; i++)
            {
                SquishyLightParticle energy = new SquishyLightParticle(Projectile.Center, (Projectile.velocity * 4).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextBool(3) ? Color.DarkViolet : mainColor, Main.rand.Next(30, 40 + 1), 0.25f, 2f);
                GeneralParticleHandler.SpawnParticle(energy);
            }
        }
    }
}
