using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TerraBulletSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private float speed = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 90 && target.CanBeChasedBy(Projectile);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 85;
            }

            Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(5) ? 131 : 294, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.5f));
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.35f, 0.55f);
            if (dust.type == 131)
                dust.scale = Main.rand.NextFloat(0.25f, 0.45f);
            else
                dust.fadeIn = 0.5f;

            if (speed == 0f)
                speed = Projectile.velocity.Length();

            if (Projectile.timeLeft < 90) {
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, speed, 12f);
            }
        }
    }
}
