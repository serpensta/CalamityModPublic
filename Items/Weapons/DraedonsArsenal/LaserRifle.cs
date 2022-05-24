﻿using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class LaserRifle : ModItem
    {
        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/LaserRifleFire");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavy Laser Rifle");
            Tooltip.SetDefault("Laser weapon used by heavy infantry units in Yharim's army");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 84;
            Item.height = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 185;
            Item.knockBack = 4f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = FireSound;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            Item.shoot = ModContent.ProjectileType<LaserRifleShot>();
            Item.shootSpeed = 5f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 190f;
            modItem.ChargePerUse = 0.125f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = velocity.X + Main.rand.Next(-1, 2) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-1, 2) * 0.05f;
                Projectile.NewProjectile(source, position, new Vector2(SpeedX, SpeedY), ModContent.ProjectileType<LaserRifleShot>(), damage, knockback, player.whoAmI, i, 0f);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 4);

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(15).
                AddIngredient<DubiousPlating>(15).
                AddIngredient<UeliaceBar>(8).
                AddIngredient(ItemID.LunarBar, 4).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(4, out Predicate<Recipe> condition), condition).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
