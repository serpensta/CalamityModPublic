﻿using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ThrowingBrick : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Throws a brick that shatters if stealth is full.\n" +
                "Prove its resistance by throwing it upwards and catching it with your face");
            DisplayName.SetDefault("Throwing Brick");
        }
        public override void SafeSetDefaults()
        {
            item.damage = 20;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<Brick>();
            item.width = 26;
            item.height = 20;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.knockBack = 10;
            item.crit = 20;
            item.value = Item.buyPrice(0, 0, 2, 0);
            item.rare = 2;
            item.maxStack = 999;
            item.UseSound = SoundID.Item1;
            item.consumable = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //Check if stealth is full
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Brick>(), damage, knockBack, player.whoAmI, 1);
                Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RedBrick);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 25);
            recipe.AddRecipe();
        }
    }
}
