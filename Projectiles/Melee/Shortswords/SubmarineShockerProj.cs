﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Shortswords
{
    public class SubmarineShockerProj: BaseShortswordProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/SubmarineShocker";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
        }

        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage *= 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack, Main.myPlayer);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack, Main.myPlayer);
        }
    }
}
