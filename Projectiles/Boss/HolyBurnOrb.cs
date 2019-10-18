﻿using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles.Boss
{
    public class HolyBurnOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Orb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.5f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode;
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            projectile.velocity.X *= 1.01f;
            projectile.velocity.Y *= 1.01f;
            int num487 = (int)Player.FindClosest(projectile.position, projectile.width, projectile.height);
            Vector2 vector36 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num489 = Main.player[num487].Center.X - vector36.X;
            float num490 = Main.player[num487].Center.Y - vector36.Y;
            float num491 = (float)Math.Sqrt((double)(num489 * num489 + num490 * num490));
            if (num491 < 50f && projectile.position.X < Main.player[num487].position.X + (float)Main.player[num487].width && projectile.position.X + (float)projectile.width > Main.player[num487].position.X && projectile.position.Y < Main.player[num487].position.Y + (float)Main.player[num487].height && projectile.position.Y + (float)projectile.height > Main.player[num487].position.Y)
            {
                int num492 = expertMode ? -150 : -100;
                if (CalamityWorld.death || CalamityWorld.bossRushActive)
                {
                    num492 = -200;
                }
                Main.player[num487].HealEffect(num492, false);
                Main.player[num487].statLife += num492;
                if (Main.player[num487].statLife > Main.player[num487].statLifeMax2)
                {
                    Main.player[num487].statLife = Main.player[num487].statLifeMax2;
                }
                if (Main.player[num487].statLife < 0 || CalamityWorld.armageddon)
                {
                    Main.player[num487].KillMe(PlayerDeathReason.ByOther(12), 1000.0, 0, false);
                }
                NetMessage.SendData(66, -1, -1, null, num487, (float)num492, 0f, 0f, 0, 0, 0);
                projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 150, 0, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 50;
            projectile.height = 50;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 247, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 60);
        }
    }
}
