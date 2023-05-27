﻿using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Pumpler : ModItem
    {

        public const int MaxPumpkins = 5;
        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 34;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.25f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 11f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-30f, 0f);

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<PumplerHoldout>()] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<PumplerHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.IllegalGunParts).
                AddIngredient(ItemID.Pumpkin, 30).
                AddIngredient(ItemID.PumpkinSeed, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

