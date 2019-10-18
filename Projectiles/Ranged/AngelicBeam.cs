﻿using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles.Ranged
{
    public class AngelicBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angelic Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;

            // Hitscan laser with a long range
            projectile.timeLeft = 200;
            projectile.extraUpdates = 200;

            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // Actual laser dust
            for (int i = 0; i < 7; ++i)
            {
                int dustType = 262; // Main.rand.NextBool() ? 244 : 246;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);

                Main.dust[idx].noGravity = true;
                Main.dust[idx].position -= i * 0.1666f * projectile.velocity;
                Main.dust[idx].velocity *= 1f;
                float scale = Main.rand.NextFloat(0.8f, 1.4f);
                Main.dust[idx].scale = scale;
            }

            // Sparkles "burning off" of the laser beam
            if (Main.rand.NextBool())
            {
                int dustType = Main.rand.NextBool() ? 244 : 246;
                int idx = Dust.NewDust(projectile.position, 1, 1, dustType);

                Main.dust[idx].noGravity = true;
                float ySpeed = Main.rand.NextFloat(3.0f, 5.6f);
                Main.dust[idx].velocity.Y -= ySpeed;
                float scale = Main.rand.NextFloat(0.4f, 0.8f);
                Main.dust[idx].scale = scale;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 600);
        }
    }
}
