﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class NightsRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Night's Ray");
            /* Tooltip.SetDefault("Fires a dark ray\n" +
                "When hitting enemies, they are hit by several new beams from their sides"); */
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item72;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NightsRayBeam>();
            Item.shootSpeed = 6f;
        }

        
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Vilethorn).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ItemID.WandofSparking).
                AddIngredient(ItemID.AmberStaff).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
