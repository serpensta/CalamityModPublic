using System;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class TheHiveHoldout : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TheHive>();

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/TheHive";

        private ref float ShootingTimer => ref Projectile.ai[0];

        private ref float OffsetLength => ref Projectile.localAI[0];

        private Player Owner;

        private const float MaxOffsetLength = 15f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Item heldItem = Owner.ActiveItem();

            // If there's no player, or the player is the server, or the owner's stunned, there'll be no holdout.
            if (Owner.CantUseHoldout() || heldItem.type != ItemType<TheHive>())
            {
                Projectile.Kill();
                NetUpdate();
                return;
            }

            // The center of the player, taking into account if they have a mount or not.
            Vector2 ownerPosition = Owner.MountedCenter;

            // The vector between the player and the mouse.
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - ownerPosition;

            // Deals with the holdout's rotation and direction, the owner's arms, etc.
            ManageHoldout(ownerPosition, ownerToMouse);

            // If the timer reaches Item.useTime, it'll shoot.
            // It'll shoot once immediately as the timer reaches.
            if (ShootingTimer >= heldItem.useAnimation)
                ShootRocket(heldItem);

            // When we change the distance of the gun from the arms for the recoil,
            // recover to the original position smoothly.
            if (OffsetLength != MaxOffsetLength)
                OffsetLength = MathHelper.Lerp(OffsetLength, MaxOffsetLength, 0.1f);

            ShootingTimer++;
        }

        private void ManageHoldout(Vector2 mountedCenter, Vector2 ownerToMouse)
        {
            Vector2 rotationVector = Projectile.rotation.ToRotationVector2();
            float velocityRotation = Projectile.velocity.ToRotation();
            float proximityLookingUpwards = Vector2.Dot(ownerToMouse.SafeNormalize(Vector2.Zero), -Vector2.UnitY);
            int direction = MathF.Sign(ownerToMouse.X);

            Vector2 armPosition = Owner.RotatedRelativePoint(mountedCenter, true);
            Vector2 lengthOffset = rotationVector * OffsetLength;
            Vector2 armOffset = new Vector2(Utils.Remap(proximityLookingUpwards, -1f, 1f, 0f, -12f) * direction, -10f + Utils.Remap(MathF.Abs(proximityLookingUpwards), 0f, 1f, 0f, proximityLookingUpwards > 0f ? 15f : 0f));
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
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, armRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRotation + MathHelper.ToRadians(15f) * direction);
        }

        private void ShootRocket(Item item)
        {
            // We use the velocity of this projectile as its direction vector.
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);

            // The position of the tip of the gun.
            Vector2 tipPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 33f;

            // Every time we shoot we use ammo.
            // With this method we also use the item's stats, like the shoot speed, or the type of ammo it was used.
            Owner.PickAmmo(item, out _, out float projSpeed, out int damage, out float knockback, out int rocketType);

            // Spawns the projectile.
            Projectile.NewProjectileDirect(
                Projectile.GetSource_FromThis(),
                tipPosition,
                shootDirection * projSpeed,
                ProjectileID.None, // TEMPORARY.
                damage,
                knockback,
                Projectile.owner,
                rocketType);

            ShootingTimer = 0f;

            NetUpdate();

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
            OffsetLength -= 8f;
        }

        private void NetUpdate()
        {
            Projectile.netUpdate = true;
            if (Projectile.netSpam >= 10)
                Projectile.netSpam = 9;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Owner = Main.player[Projectile.owner];
            OffsetLength = MaxOffsetLength;
        }

        // Because we use the velocity as a direction, we don't need it to change its position.
        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            Main.EntitySpriteDraw(glowTexture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            return false;
        }
    }
}
