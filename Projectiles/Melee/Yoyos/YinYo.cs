﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Melee
{
    public class YinYo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yinyo");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Chik);
            projectile.width = 16;
            projectile.scale = 0.9f;
            projectile.height = 16;
            projectile.penetrate = 5;
            projectile.melee = true;
            aiType = 546;
        }

        public override void AI()
        {
            int[] array = new int[20];
            int num428 = 0;
            float num429 = 300f;
            bool flag14 = false;
            for (int num430 = 0; num430 < 200; num430++)
            {
                if (Main.npc[num430].CanBeChasedBy(projectile, false))
                {
                    float num431 = Main.npc[num430].position.X + (float)(Main.npc[num430].width / 2);
                    float num432 = Main.npc[num430].position.Y + (float)(Main.npc[num430].height / 2);
                    float num433 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num431) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num432);
                    if (num433 < num429 && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num430].Center, 1, 1))
                    {
                        if (num428 < 20)
                        {
                            array[num428] = num430;
                            num428++;
                        }
                        flag14 = true;
                    }
                }
            }
            if (flag14)
            {
                int num434 = Main.rand.Next(num428);
                num434 = array[num434];
                float num435 = Main.npc[num434].position.X + (float)(Main.npc[num434].width / 2);
                float num436 = Main.npc[num434].position.Y + (float)(Main.npc[num434].height / 2);
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 8f)
                {
                    projectile.localAI[0] = 0f;
                    float num437 = 0f;
                    Vector2 value10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    value10 += projectile.velocity * 4f;
                    float num438 = num435 - value10.X;
                    float num439 = num436 - value10.Y;
                    float num440 = (float)Math.Sqrt((double)(num438 * num438 + num439 * num439));
                    num440 = num437 / num440;
                    num438 *= num440;
                    num439 *= num440;
                    if (Main.rand.NextBool(6))
                    {
                        int choice = Main.rand.Next(2);
                        if (projectile.owner == Main.myPlayer)
                        {
                            if (choice == 0)
                            {
                                Projectile.NewProjectile(value10.X, value10.Y, num438, num439, ModContent.ProjectileType<Dark>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                            }
                            else
                            {
                                Projectile.NewProjectile(value10.X, value10.Y, num438, num439, ModContent.ProjectileType<Light>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
