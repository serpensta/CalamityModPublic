using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VisceraBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 100;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[Projectile.owner].moonLeech)
                return;

            Player player = Main.player[Projectile.owner];
            player.statLife += 1;
            player.HealEffect(1);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VisceraBoom>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {
            Particle bloodsplosion = new CustomPulse(Projectile.Center, Vector2.Zero, new Color(255, 32, 32), "CalamityMod/Particles/PlasmaExplosion", Vector2.One, Main.rand.NextFloat(-15f, 15f), 0.03f, 0.0875f, Viscera.BoomLifetime);
            GeneralParticleHandler.SpawnParticle(bloodsplosion);
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 17f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * ((float)i * 0.25f);
                    int bloody = Dust.NewDust(projPos, 1, 1, DustID.Blood, 0f, 0f, 0, default, 1f);
                    Main.dust[bloody].position = projPos;
                    Main.dust[bloody].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[bloody].velocity *= 0.1f;
                }
            }
        }
    }
}
