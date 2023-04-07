using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BlueFlamePillar : ModProjectile
    {
        public int frameX = 0;
        public int frameY = 0;
        public int currentFrame => frameY + frameX * 6;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flame Pillar");
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 322;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        public override void AI()
        {
            //2-6
            Projectile.frameCounter += 1;
            if (Projectile.frameCounter % 7 == 6)
            {
                frameY += 1;
                if (frameY >= 6)
                {
                    frameX += 1;
                    frameY = 0;
                }
                if (frameX >= 3)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.position.Y -= Projectile.height / 2; //position adjustments
                Projectile.localAI[0] = 1f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle(frameX * 80, frameY * 322, 80, 322);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}
