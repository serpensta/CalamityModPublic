﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class RadiantStarKnife : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RadiantStar";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (Projectile.ai[0] == 1f)
            {
                float num472 = Projectile.Center.X;
                float num473 = Projectile.Center.Y;
                float num474 = Projectile.Calamity().stealthStrike ? 1800f : 600f;
                float homingSpeed = 0.25f;
                for (int num475 = 0; num475 < Main.npc.Length; num475++)
                {
                    NPC npc = Main.npc[num475];
                    if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1) && !npc.boss)
                    {
                        float num476 = npc.position.X + (float)(npc.width / 2);
                        float num477 = npc.position.Y + (float)(npc.height / 2);
                        float num478 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num476) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num477);
                        if (num478 < num474)
                        {
                            if (npc.position.X < num472)
                            {
                                npc.velocity.X += homingSpeed;
                            }
                            else
                            {
                                npc.velocity.X -= homingSpeed;
                            }
                            if (npc.position.Y < num473)
                            {
                                npc.velocity.Y += homingSpeed;
                            }
                            else
                            {
                                npc.velocity.Y -= homingSpeed;
                            }
                        }
                    }
                }
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] == 25f)
            {
                int numProj = Projectile.Calamity().stealthStrike ? 7 : 3;
                float rotation = MathHelper.ToRadians(50);
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        while (speed.X == 0f && speed.Y == 0f)
                        {
                            speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        }
                        speed.Normalize();
                        speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2.5f;
                        int stabber2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<RadiantStar2>(), Projectile.damage, Projectile.knockBack, Projectile.owner,
                            Projectile.ai[0] == 1f ? 1f : 0f, 0f);
                        Main.projectile[stabber2].Calamity().stealthStrike = Projectile.Calamity().stealthStrike;
                    }
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                    int boomer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RadiantExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (Projectile.Calamity().stealthStrike && boomer.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[boomer].Calamity().stealthStrike = true;
                        Main.projectile[boomer].height = 300;
                        Main.projectile[boomer].width = 300;
                    }
                    Main.projectile[boomer].Center = Projectile.Center;
                    Projectile.active = false;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
