using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static Humanizer.In;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Melee
{
    public class DarkMasterClone : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // if the velocity is not zero, the visuals get offset weirdly
            Projectile.velocity = Vector2.Zero;
            Player owner = Main.player[Projectile.owner];
            // how far the clone should move from the player
            Vector2 moveTo = new Vector2(0, -160);
            switch (Projectile.ai[0])
            {
                case 1:
                    moveTo = new Vector2(-180, 120);
                    break;
                case 2:
                    moveTo = new Vector2(180, 120);
                    break;
                default:
                    moveTo = new Vector2(0, -160);
                    break;
            }
            // if the player isn't holding the sword, DIE.
            if (owner.HeldItem.type != ModContent.ItemType<TheDarkMaster>() || !owner.active || owner.CCed || owner == null || Projectile.type != ModContent.ProjectileType<DarkMasterClone>())
            {
                Projectile.Kill();
            }
            // if all conditions above aren't met, the clone can stick around forever
            Projectile.timeLeft = 30;
            // move the clone to the desired position
            Projectile.Center = Vector2.Lerp(Projectile.Center, owner.Center + moveTo, 0.4f);
            // assure that the clones only shoot at the owner's cursor
            if (Main.myPlayer != Projectile.owner)
                return;
            // shoot beams while the player is left clicking
            if (owner.itemTime == owner.itemTimeMax && owner.altFunctionUse != 2)
            {
                Vector2 direction = Projectile.Center.DirectionTo(Main.MouseWorld);
                Projectile.direction = Math.Sign(direction.X);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // ai[1] not being 0 determines if the projectile should always ignore tiles
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction * owner.HeldItem.shootSpeed, ModContent.ProjectileType<DarkMasterBeam>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack, Projectile.owner, ai1: 1);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                float angle = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
                Vector2 angleVec = angle.ToRotationVector2();
                Particle smoke = new HeavySmokeParticle(Projectile.Center, angleVec * Main.rand.NextFloat(2f, 4f), Color.Black, 60, Main.rand.NextFloat(0.45f, 1.22f), 0.6f, 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // make a player visual clone. it inherits the player's hair type and clothes style and is otherwise all black with red pupils
            Main.playerVisualClone[Projectile.owner] ??= new();
            Player owner = Main.player[Projectile.owner];

            Player player = Main.playerVisualClone[Projectile.owner];
            //player.CopyVisuals(Main.player[Projectile.owner]);
            player.hair = owner.hair;
            player.skinVariant = owner.skinVariant;
            player.skinColor = Color.Black;
            player.shirtColor = Color.Black;
            player.underShirtColor = Color.Black;
            player.pantsColor = Color.Black;
            player.shoeColor = Color.Black;
            player.hairColor = Color.Black;
            player.eyeColor = Color.Red;
            player.PlayerFrame();
            // copy the player's arm movements while swinging, otherwise idle
            if (owner.ItemAnimationActive && owner.altFunctionUse != 2)
                player.bodyFrame = owner.bodyFrame;
            else
                player.bodyFrame.Y = 0;
            // legs never jump or walk
            player.legFrame.Y = 0;
            // face towards the player's cursor
            player.direction = Math.Sign(Projectile.DirectionTo(Main.MouseWorld).X);
            Main.PlayerRenderer.DrawPlayer(Main.Camera, player, Projectile.position, 0f, player.fullRotationOrigin, 0f, 1f);
            // draw the sword
            if (owner.ItemAnimationActive && owner.altFunctionUse != 2)
            {
                Texture2D Sword = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TheDarkMaster").Value;
                Vector2 distToPlayer = Projectile.position - owner.position;
                Main.EntitySpriteDraw(Sword, (Vector2)owner.HandPosition + distToPlayer - Main.screenPosition, null, lightColor, owner.direction == player.direction ? owner.itemRotation : -owner.itemRotation, new Vector2(player.direction == 1 ? 0 : Sword.Width, Sword.Height), 1f, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }
}
