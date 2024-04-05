using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SupremeCataclysmFist : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public ref float Time => ref Projectile.ai[0];
        public Vector2 shootVel;
        public int rotDirection = 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 54;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            if (Projectile.ai[2] >= 3)
            {
                if (Projectile.timeLeft > 240)
                {
                    shootVel = Projectile.velocity * 0.2f;
                    Projectile.timeLeft = 240;
                    rotDirection = Main.rand.NextBool() ? -1 : 1;
                }
                else
                {
                    float randSize = Main.rand.NextFloat(0.8f, 1.2f);
                    for (int i = 0; i < 2; i++)
                    {
                        Particle bloom = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.5f), "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1.7f * randSize, 0f, 10);
                        GeneralParticleHandler.SpawnParticle(bloom);
                    }
                    Particle bloom3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.9f, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1.4f * randSize, 0f, 10);
                    GeneralParticleHandler.SpawnParticle(bloom3);

                    Projectile.velocity *= 0.985f;
                }

                // Adds rotation, but it's SUPER unfair a lot of the time
                //shootVel = shootVel.RotatedBy(Time * 0.00011f * rotDirection);

                if (Projectile.timeLeft >= 2 && Time % 3 == 0)
                {
                    Vector2 randPos = shootVel.RotatedBy(MathHelper.ToRadians(90f)) * Main.rand.NextFloat(-25, 25);
                    int type = ModContent.ProjectileType<SupremeCataclysmFist>();
                    SoundEngine.PlaySound(SupremeCalamitas.BrimstoneShotSound with { Volume = 1.2f, Pitch = 0.55f }, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + randPos, (shootVel * MathHelper.Clamp(Time * 0.1f, 1, 1.8f)) * Main.rand.NextFloat(0.6f, 0.9f), type, Projectile.damage / 2, 0f, Main.myPlayer, 0f, Main.rand.Next(0, 1 + 1), 0);
                }
                if (Projectile.timeLeft >= 2 && Time % 3 == 0 && NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>()) == false)
                {
                    Vector2 randPos = shootVel.RotatedBy(MathHelper.ToRadians(90f)) * Main.rand.NextFloat(-25, 25);
                    int type = ModContent.ProjectileType<SupremeCataclysmFist>();
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + randPos, (shootVel * MathHelper.Clamp(Time * 0.1f, 1, 1.8f)).RotatedBy(MathHelper.ToRadians(120f)) * Main.rand.NextFloat(0.6f, 0.9f), type, Projectile.damage / 2, 0f, Main.myPlayer, 0f, Main.rand.Next(0, 1 + 1), 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + randPos, (shootVel * MathHelper.Clamp(Time * 0.1f, 1, 1.8f)).RotatedBy(MathHelper.ToRadians(-120f)) * Main.rand.NextFloat(0.6f, 0.9f), type, Projectile.damage / 2, 0f, Main.myPlayer, 0f, Main.rand.Next(0, 1 + 1), 0);
                }
                if (Projectile.timeLeft == 1)
                {
                    int points = 25;
                    float radians = MathHelper.TwoPi / points;
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-4f, -4f));
                    for (int b = 0; b < 2; b++)
                    {
                        float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
                        for (int k = 0; k < points; k++)
                        {
                            Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);

                            Dust cataclysmdust = Dust.NewDustPerfect(Projectile.Center + velocity * (b == 0 ? 7 : 5), 279, velocity * (b == 0 ? 9 : 7));
                            cataclysmdust.noGravity = true;
                            cataclysmdust.scale = Main.rand.NextFloat(1.3f, 1.9f);
                            cataclysmdust.color = Color.Red;
                        }
                    }
                }

                Vector2 vel = new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                Dust cataclysmdust2 = Dust.NewDustPerfect(Projectile.Center + vel * 2, 279, vel);
                cataclysmdust2.noGravity = true;
                cataclysmdust2.scale = Main.rand.NextFloat(0.9f, 1.2f);
                cataclysmdust2.color = Color.Red;
            }
            else if (Projectile.ai[2] <= 2)
            {
                if (Projectile.ai[2] == 1)
                {
                    Projectile.extraUpdates = 2;
                }
                else
                    Projectile.velocity *= 1.0045f;

                if (Projectile.ai[2] == 0)
                {
                    Projectile.scale = 0.8f;
                    Projectile.extraUpdates = 2;
                    Projectile.Opacity = 1;
                    if (Projectile.timeLeft > 300)
                        Projectile.timeLeft = 300;
                    Projectile.velocity *= 1.009f;
                }

                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
                }
            }

            // Decide frames.
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Projectile.type];

            // Fade in and handle visuals.
            if (Projectile.ai[2] < 3)
                Projectile.Opacity = Utils.GetLerpValue(0f, 12f, Projectile.timeLeft, true) * Utils.GetLerpValue(1200f, 1193f, Projectile.timeLeft, true);
            
            Time++;

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0.5f * Projectile.Opacity, 0f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);

            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.ai[1] == 1f)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/SupremeCataclysmFistAlt").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity >= 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 240, true);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 35 * Projectile.scale, targetHitbox);
    }
}
