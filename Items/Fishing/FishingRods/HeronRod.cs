﻿using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class HeronRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Heron Rod");
            /* Tooltip.SetDefault("Increased fishing power in space.\n" + //John Steinbeck quote but fish instead of snake
                "A silent head and beak lanced down and plucked it out by the head,\n" +
                "and the beak swallowed the little fish while its tail waved frantically."); */
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 25;
            Item.shootSpeed = 14.5f;
            Item.shoot = ModContent.ProjectileType<HeronBobber>();
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<AerialiteBar>(), 6).
                AddIngredient(ItemID.SunplateBlock, 3).
                AddTile(TileID.SkyMill).
                Register();
        }
    }
}
