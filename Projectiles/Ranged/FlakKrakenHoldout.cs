using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Ranged.FlakKraken;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<FlakKraken>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/FlakKraken";

        public Player Owner { get; set; }
        public ref float ShootingTimer => ref Projectile.ai[0];
        public ref float TimerBetweenBursts => ref Projectile.ai[1];
        public ref float OffsetLengthScalar => ref Projectile.localAI[0];

        // The type of dust used in the effects and the color of the particles that it'll use depending on the type of rocket used.
        // It'll carry over to the projectiles that it shoots.
        public static int DustEffectsID { get; set; }
        public static Color EffectsColor { get; set; }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 152;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            // If there's no player, or the player is the server, or the owner's stunned, there'll be no holdout.
            bool cantUse = Owner == null || !Owner.active || Owner.dead || !Owner.channel || Main.myPlayer != Projectile.owner || Owner.CCed || Owner.noItems;
            if (cantUse)
            {
                Projectile.Kill();
                return;
            }

            // The center of the player, taking into account if they have a mount or not.
            Vector2 mountedCenter = Owner.MountedCenter;

            // The vector between the player and the mouse.
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - mountedCenter;

            // Deals with the holdout's rotation and direction, the owner's arms, etc.
            ManageHoldout(mountedCenter, ownerToMouse);

            // If the timer reaches Item.useTime, it'll shoot.
            // It'll shoot once immediately as the timer reaches.
            if (ShootingTimer >= Owner.itemTimeMax)
            {
                // Every X frames it'll shoot a projectile.
                if (ShootingTimer % TimeBetweenShots == 0f)
                {
                    // We use the velocity of this projectile as its direction vector.
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);

                    // The position of the tip of the gun.
                    Vector2 nuzzlePosition = Projectile.Center + direction * Projectile.width / 2f;

                    // Every time we shoot we use ammo.
                    // With this method we also use the item's stats, like the shoot speed, or the type of ammo it was used.
                    Owner.PickAmmo(Owner.ActiveItem(), out _, out float itemShootSpeed, out int itemDamage, out float itemKnockback, out int rocketTypeShot);

                    // Decides the color of the effects depending on the type of rocket used.
                    switch (rocketTypeShot)
                    {
                        case ItemID.WetRocket:
                            DustEffectsID = 45;
                            EffectsColor = Color.RoyalBlue;
                            break;
                        case ItemID.LavaRocket:
                            DustEffectsID = DustID.Torch;
                            EffectsColor = Color.Red;
                            break;
                        case ItemID.HoneyRocket:
                            DustEffectsID = DustID.Honey;
                            EffectsColor = Color.Yellow;
                            break;
                        default:
                            DustEffectsID = 109;
                            EffectsColor = Color.Black;
                            break;
                    }

                    // Spawns the projectile.
                    Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromThis(),
                        nuzzlePosition,
                        direction * itemShootSpeed,
                        ProjectileType<FlakKrakenProjectile>(),
                        itemDamage,
                        itemKnockback,
                        Projectile.owner,
                        rocketTypeShot,
                        ownerToMouse.Length());

                    // Applies the knockback to the player.
                    Owner.velocity += ownerToMouse.SafeNormalize(Vector2.UnitY) * -OwnerKnockbackStrength;

                    // Inside here go all the things that dedicated servers shouldn't spend resources on.
                    // Like visuals and sounds.
                    if (!Main.dedServ)
                    {
                        // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
                        OffsetLengthScalar = 10f;

                        Owner.Calamity().GeneralScreenShakePower = 3.5f;

                        Particle shootPulse = new DirectionalPulseRing(nuzzlePosition,
                            Vector2.Zero,
                            Color.Gray * 0.7f,
                            new Vector2(0.5f, 1f),
                            Projectile.rotation,
                            0.1f,
                            0.4f,
                            20);
                        GeneralParticleHandler.SpawnParticle(shootPulse);

                        int smokeAmount = Main.rand.Next(12, 16 + 1);
                        for (int i = 0; i < smokeAmount; i++)
                        {
                            Particle smoke = new HeavySmokeParticle(
                                nuzzlePosition,
                                direction.RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(2f, 30f),
                                EffectsColor * 0.1f,
                                Main.rand.Next(45, 61),
                                Main.rand.NextFloat(.6f, 1.3f),
                                Main.rand.NextFloat(0.2f, 0.35f));
                            GeneralParticleHandler.SpawnParticle(smoke);
                        }

                        int dustAmount = Main.rand.Next(15, 20 + 1);
                        for (int i = 0; i < dustAmount; i++)
                        {
                            Dust shootDust = Dust.NewDustPerfect(
                                nuzzlePosition,
                                DustEffectsID,
                                direction.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(2f, 12f),
                                Scale: Main.rand.NextFloat(0.8f, 1f));
                            shootDust.fadeIn = 100f;
                            shootDust.noLight = false;
                            shootDust.noLightEmittence = false;
                        }

                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") { Volume = 0.6f }, nuzzlePosition);
                    }

                    NetUpdate();
                }

                // When it has shot all the projectiles in the burst, reset all the variables and back to shooting.
                if (ShootingTimer >= Owner.itemTimeMax + TimeBetweenShots * (ProjectilesPerBurst - 1))
                {
                    ShootingTimer = 0f;
                    TimerBetweenBursts = 0f;
                    NetUpdate();
                }
            }

            // When we change the distance of the gun from the arms for the recoil,
            // recover to the original position smoothly.
            if (OffsetLengthScalar != 30f)
                OffsetLengthScalar = MathHelper.Lerp(OffsetLengthScalar, 30f, 0.05f);

            ShootingTimer++;
        }

        #region AI Methods

        public void ManageHoldout(Vector2 mountedCenter, Vector2 ownerToMouse)
        {
            Vector2 rotationVector = Projectile.rotation.ToRotationVector2();
            float velocityRotation = Projectile.velocity.ToRotation();
            float proximityLookingUpwards = Vector2.Dot(ownerToMouse.SafeNormalize(Vector2.Zero), -Vector2.UnitY);
            int direction = MathF.Sign(ownerToMouse.X);

            Vector2 armPosition = Owner.RotatedRelativePoint(mountedCenter, true);
            Vector2 lengthOffset = rotationVector * OffsetLengthScalar;
            Vector2 armOffset = new Vector2(Utils.Remap(proximityLookingUpwards, -1f, 1f, 15f, -18f) * direction, -20f + Utils.Remap(MathF.Abs(proximityLookingUpwards), 0f, 1f, 0f, proximityLookingUpwards > 0f ? 20f : 10f));
            Projectile.Center = armPosition + lengthOffset + armOffset;
            Projectile.velocity = velocityRotation.AngleTowards(ownerToMouse.ToRotation(), 0.2f).ToRotationVector2();
            Projectile.rotation = velocityRotation;
            Projectile.timeLeft = 2;

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            Projectile.spriteDirection = Projectile.direction = direction;
            Owner.ChangeDir(direction);

            float armRotation = Projectile.rotation - MathHelper.PiOver2; // -Pi/2 because the arms rotation starts with arms pointing down.
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRotation);
        }

        public void NetUpdate()
        {
            Projectile.netUpdate = true;
            if (Projectile.netSpam >= 10)
                Projectile.netSpam = 9;
        }

        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            Owner = Main.player[Projectile.owner];
            OffsetLengthScalar = 30f;
        }

        // Because we use the velocity as a direction, we don't need it to change its position.
        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            return false;
        }
    }
}
