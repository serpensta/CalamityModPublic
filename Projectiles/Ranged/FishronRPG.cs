﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FishronRPG : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("RPG");
		}

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 95;
            projectile.ranged = true;
        }

        public override void AI()
        {
        	if (projectile.ai[1] == 0f)
        	{
        		for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
        		projectile.ai[1] = 1f;
        		Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 92);
        	}
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.tileCollide = false;
				projectile.ai[1] = 0f;
				projectile.alpha = 255;
				projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
				projectile.width = 200;
				projectile.height = 200;
				projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				projectile.knockBack = 10f;
			}
			else
			{
				if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
				{
					for (int num246 = 0; num246 < 2; num246++)
					{
						float num247 = 0f;
						float num248 = 0f;
						if (num246 == 1)
						{
							num247 = projectile.velocity.X * 0.5f;
							num248 = projectile.velocity.Y * 0.5f;
						}
						int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 33, 0f, 0f, 100, default(Color), 1f);
						Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
						Main.dust[num249].velocity *= 0.2f;
						Main.dust[num249].noGravity = true;
						num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 33, 0f, 0f, 100, default(Color), 0.5f);
						Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
						Main.dust[num249].velocity *= 0.05f;
					}
				}
			}
			float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 600f;
			bool flag17 = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
				{
					float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
					float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			if (flag17)
			{
				float num483 = 12f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
			}
        }

        public override void Kill(int timeLeft)
        {
        	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("MiniRocketExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 92);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 50;
			projectile.height = 50;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num621 = 0; num621 < 10; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 20; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default(Color), 3f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num624].velocity *= 2f;
			}
        }
    }
}
