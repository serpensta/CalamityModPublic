using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GenisisHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Genisis>();
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitY.RotatedBy(Projectile.rotation) * 2f * Projectile.spriteDirection;
        public override float MaxOffsetLengthFromArm => 10f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;

        public Color StaticEffectsColor = Color.Cyan;

        public float FiringTime = 15;
        public float Windup = 60;
        public bool WindingUp = false;
        public float ProjectileSpeed = 12f;
        public int time = 0;
        public bool fireYBeam = false;
        public int maxFirerateShots = 0;

        private ref float ShootingTimer => ref Projectile.ai[0];

        public override void KillHoldoutLogic()
        {
            if (HeldItem.type != Owner.ActiveItem().type || Owner is null || !Owner.active || Owner.CCed || Owner.dead || Owner.noItems)
            {
                Projectile.Kill();
                Projectile.netUpdate = true;
            }
        }

        public override void HoldoutAI()
        {
            Item heldItem = Owner.ActiveItem();
            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (Owner.CantUseHoldout())
            {
                Projectile.Kill();
            }
            else if (ShootingTimer >= FiringTime)
            {
                if (Owner.CheckMana(Owner.ActiveItem(), -1, true, false))
                {
                    maxFirerateShots++;

                    if (maxFirerateShots == 6)
                    {
                        fireYBeam = true;
                        Windup = 60;
                    }

                    if (fireYBeam)
                    { 
                        Shoot(heldItem, true);
                        maxFirerateShots = 0;
                    }
                    else
                        Shoot(heldItem, false);

                    ShootingTimer = 0;
                    
                    if (Windup > 10)
                    {
                        Windup -= 10;
                    }
                    else
                    {
                        WindingUp = !WindingUp;
                        Windup = 10;
                    }

                    fireYBeam = false;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f }, Projectile.Center);
                    Projectile.Kill();
                }

            }
            ShootingTimer++;
        }
        private void Shoot(Item item, bool yBeam)
        {
            Vector2 shootDirection = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);

            // Spawns the projectile.

            Vector2 firingVelocity1 = (shootDirection * 8).RotatedBy(0.1f * Utils.GetLerpValue(10, 55, Windup, true));
            Vector2 firingVelocity2 = (shootDirection * 8).RotatedBy(-0.1f * Utils.GetLerpValue(10, 55, Windup, true));
            Vector2 firingVelocity3 = (shootDirection * 10);

            if (yBeam)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/LanceofDestinyStrong");
                SoundEngine.PlaySound(fire with { Volume = 0.35f, Pitch = 1f, PitchVariance = 0.15f }, Projectile.Center);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3, ModContent.ProjectileType<GenisisBeam>(), Projectile.damage * 10, Projectile.knockBack, Projectile.owner, 0, 0);

                Particle pulse3 = new GlowSparkParticle((GunTipPosition - firingVelocity3 * 2), firingVelocity3 * 2, false, Main.rand.Next(7, 11 + 1), 0.047f, StaticEffectsColor, new Vector2(1.5f, 0.9f), true);
                GeneralParticleHandler.SpawnParticle(pulse3);
                for (int i = 0; i < 8; i++)
                {
                    SparkParticle spark2 = new SparkParticle(GunTipPosition + Main.rand.NextVector2Circular(10, 10), firingVelocity3 * Main.rand.NextFloat(0.7f, 1.3f), false, Main.rand.Next(20, 30), Main.rand.NextFloat(0.4f, 0.55f), StaticEffectsColor);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
            else
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/MagnaCannonShot");
                SoundEngine.PlaySound(fire with { Volume = 0.25f, Pitch = 1f, PitchVariance = 0.35f }, Projectile.Center);

                for (int i = 0; i < 4; i++)
                {
                    firingVelocity3 = (shootDirection * 10).RotatedBy((0.045f * (i + 1)) * Utils.GetLerpValue(0, 55, Windup, true));
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3 * (1 - i * 0.1f), ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                }
                for (int i = 0; i < 4; i++)
                {
                    firingVelocity3 = (shootDirection * 10).RotatedBy((-0.045f * (i + 1)) * Utils.GetLerpValue(0, 55, Windup, true));
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3 * (1 - i * 0.1f), ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                }
                //Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity2, ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                //Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3, ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);

                Particle pulse = new GlowSparkParticle((GunTipPosition - firingVelocity1 * 2), firingVelocity1 * 2, false, Main.rand.Next(7, 11 + 1), 0.035f, StaticEffectsColor, new Vector2(1.5f, 0.9f), true);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new GlowSparkParticle((GunTipPosition - firingVelocity2 * 2), firingVelocity2 * 2, false, Main.rand.Next(7, 11 + 1), 0.035f, StaticEffectsColor, new Vector2(1.5f, 0.9f), true);
                GeneralParticleHandler.SpawnParticle(pulse2);
                //Particle pulse3 = new GlowSparkParticle((GunTipPosition - firingVelocity3 * 2), firingVelocity3 * 2, false, Main.rand.Next(7, 11 + 1), 0.04f, StaticEffectsColor, new Vector2(1.5f, 0.9f), true);
                //GeneralParticleHandler.SpawnParticle(pulse3);
                //Particle sparkle = new GenericSparkle(GunTipPosition, Vector2.Zero, Color.White, StaticEffectsColor, 1.7f, 4, Main.rand.NextFloat(-7f, 7f), 2);
                //GeneralParticleHandler.SpawnParticle(sparkle);
            }

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            for (int k = 0; k < 10; k++)
            {
                Vector2 shootVel = (shootDirection * 15).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(4) ? 267 : 66, shootVel);
                dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool() ? Color.Lerp(StaticEffectsColor, Color.White, 0.5f) : StaticEffectsColor;
            }

            // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
            if (yBeam)
                OffsetLengthFromArm -= 27f;
            else
                OffsetLengthFromArm -= 5f;
        }
    }
}
