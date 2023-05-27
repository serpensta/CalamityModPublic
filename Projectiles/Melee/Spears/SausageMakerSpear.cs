﻿using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class SausageMakerSpear : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 44;  //The width of the .png file in pixels divided by 2.
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.timeLeft = 90;
            Projectile.height = 42;  //The height of the .png file in pixels divided by 2.
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1.1f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 5, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 1.2f, ModContent.ProjectileType<Blood2>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
