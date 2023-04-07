﻿using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ElementTentacle : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tentacle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(Projectile.velocity.X) < 1f)
                {
                    Projectile.velocity.X = -Projectile.velocity.X;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(Projectile.velocity.Y) < 1f)
                {
                    Projectile.velocity.Y = -Projectile.velocity.Y;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Vector2 center10 = Projectile.Center;
            Projectile.scale = 1f - Projectile.localAI[0];
            Projectile.width = (int)(20f * Projectile.scale);
            Projectile.height = Projectile.width;
            Projectile.position.X = center10.X - (float)(Projectile.width / 2);
            Projectile.position.Y = center10.Y - (float)(Projectile.height / 2);
            if ((double)Projectile.localAI[0] < 0.1)
            {
                Projectile.localAI[0] += 0.01f;
            }
            else
            {
                Projectile.localAI[0] += 0.025f;
            }
            if (Projectile.localAI[0] >= 0.95f)
            {
                Projectile.Kill();
            }
            Projectile.velocity.X = Projectile.velocity.X + Projectile.ai[0] * 2f;
            Projectile.velocity.Y = Projectile.velocity.Y + Projectile.ai[1] * 2f;
            if (Projectile.velocity.Length() > 16f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 16f;
            }
            Projectile.ai[0] *= 1.05f;
            Projectile.ai[1] *= 1.05f;
            if (Projectile.scale < 1f)
            {
                int num897 = 0;
                while ((float)num897 < Projectile.scale * 10f)
                {
                    int num898 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.1f);
                    Main.dust[num898].position = (Main.dust[num898].position + Projectile.Center) / 2f;
                    Main.dust[num898].noGravity = true;
                    Main.dust[num898].velocity *= 0.1f;
                    Main.dust[num898].velocity -= Projectile.velocity * (1.3f - Projectile.scale);
                    Main.dust[num898].scale += Projectile.scale * 0.75f;
                    num897++;
                }
            }
        }
    }
}
