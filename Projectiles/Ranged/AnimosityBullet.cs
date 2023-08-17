using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Projectiles;

namespace CalamityMod.Projectiles.Ranged
{
    internal class AnimosityBullet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Lighting
            Lighting.AddLight(Projectile.Center, 0.9f, 0f, 0.15f);

            // Dust
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 6f)
            {
                float scale = Main.rand.NextFloat(0.6f, 0.9f);
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, (int)CalamityDusts.Brimstone);
                Vector2 posOffset = Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;
                d.position += posOffset - 2f * Vector2.UnitY;
                d.noGravity = true;
                d.velocity *= 0.6f;
                d.velocity += Projectile.velocity * 0.15f;
                d.scale = scale;
            }
        }

        public override void Kill(int timeLeft)
        { 
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Brimsplosion>(), Projectile.damage/3, Projectile.knockBack, Projectile.owner);
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
    }
}
