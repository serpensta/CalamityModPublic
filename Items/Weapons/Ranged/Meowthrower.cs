﻿using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Meowthrower : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meowthrower");
            Tooltip.SetDefault("50% chance to not consume gel\n" +
                "Fires blue and pink flames that emit meows on enemy hits");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 24;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.25f;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MeowFire>();
            Item.shootSpeed = 5.5f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num6 = Main.rand.Next(1, 3);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-15, 16) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-15, 16) * 0.05f;
                switch (Main.rand.Next(3))
                {
                    case 1:
                        type = ModContent.ProjectileType<MeowFire>();
                        break;
                    case 2:
                        type = ModContent.ProjectileType<MeowFire2>();
                        break;
                    default:
                        break;
                }
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }
    }
}
