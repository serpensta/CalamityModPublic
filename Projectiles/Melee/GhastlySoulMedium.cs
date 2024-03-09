using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GhastlySoulMedium : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        private const int TimeLeft = 300;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = TimeLeft;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);

            int ghostlyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 0, default, 1f);
            Main.dust[ghostlyDust].velocity *= 0.1f;
            Main.dust[ghostlyDust].scale = 1.3f;
            Main.dust[ghostlyDust].noGravity = true;

            float inertia = 25f * Projectile.ai[1];
            float velocity = VoidEdge.ShootSpeed * VoidEdge.MediumSoulStatMultiplier * Projectile.ai[1];
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                float homingDistance = 750f;
                NPC target = Projectile.FindTargetWithinRange(homingDistance);
                if (Projectile.timeLeft < TimeLeft - VoidEdge.ProjectileSpreadOutTime && target != null)
                {
                    CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, homingDistance, velocity, inertia);
                }
                else if (Projectile.Distance(Main.player[Projectile.owner].Center) > homingDistance)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * velocity) / inertia;
                }
            }
            else
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
            }

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > TimeLeft - 5)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 160;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= VoidEdge.TotalProjectilesPerSwing;
            Projectile.Damage();
            SoundEngine.PlaySound(VoidEdge.ProjectileDeathSound, Projectile.Center);

            int dustAmt = 36;
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.2f;
                rotate = rotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int killedDust = Dust.NewDust(rotate + faceDirection, 0, 0, DustID.ShadowbeamStaff, faceDirection.X, faceDirection.Y, 100, default, 3f);
                Main.dust[killedDust].noGravity = true;
                Main.dust[killedDust].noLight = true;
                Main.dust[killedDust].velocity = faceDirection;
            }

            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.15f;
                rotate = rotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int killedDust = Dust.NewDust(rotate + faceDirection, 0, 0, DustID.ShadowbeamStaff, faceDirection.X, faceDirection.Y, 100, default, 2f);
                Main.dust[killedDust].noGravity = true;
                Main.dust[killedDust].noLight = true;
                Main.dust[killedDust].velocity = faceDirection;
            }
        }
    }
}
