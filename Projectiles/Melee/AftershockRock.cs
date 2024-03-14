using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class AftershockRock : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 34;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            AIType = ProjectileID.BoulderStaffOfEarth;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.Center.Y > Projectile.ai[2])
                Projectile.tileCollide = true;
            else
                Projectile.tileCollide = false;
        }
    }
}
