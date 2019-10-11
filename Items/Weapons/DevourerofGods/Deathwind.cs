using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DevourerofGods
{
    public class Deathwind : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deathwind");
            Tooltip.SetDefault("Fires a spread of arrows\n" +
                "Wooden arrows are converted to nebula shots");
        }

        public override void SetDefaults()
        {
            item.damage = 265;
            item.ranged = true;
            item.width = 40;
            item.height = 82;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("NebulaShot");
            item.shootSpeed = 20f;
            item.useAmmo = 40;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(20f, 41f);
            spriteBatch.Draw(mod.GetTexture("Items/Weapons/DevourerofGods/DeathwindGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedA = speedX;
            float SpeedB = speedY;
            int num6 = Main.rand.Next(4, 6);
            for (int index = 0; index < num6; ++index)
            {
                float num7 = speedX;
                float num8 = speedY;
                float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("NebulaShot"), damage, knockBack, player.whoAmI, 0f, 0f);
                }
                else
                {
                    int num121 = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[num121].noDropItem = true;
                }
            }
            return false;
        }
    }
}
