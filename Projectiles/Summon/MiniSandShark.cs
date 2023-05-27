﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class MiniSandShark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;

            Projectile.width = Projectile.height = 32;

            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation(); // The projectile looks towards where it's going.
            
            Projectile.netUpdate = true;
        }

        public override void Kill(int timeLeft) // When the projectile hits an enemy, it'll make a dust impact effect.
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int dustIndex = 0; dustIndex < 36; dustIndex++)
            {
                Dust.NewDust(Projectile.Center,
                    Projectile.width,
                    Projectile.height,
                    85,
                    Projectile.velocity.X / 2f,
                    Projectile.velocity.Y / 2f);
            }
        }
    }
}
