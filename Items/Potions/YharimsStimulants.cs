﻿using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class YharimsStimulants : ModItem
    {
        internal static readonly int CritBoost = 2;
        
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Yharim's Stimulants");
            Tooltip.SetDefault("Gives decent buffs to ALL offensive and defensive stats");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<YharimPower>();
            Item.buffTime = CalamityUtils.SecondsToFrames(1800f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyFood").
                AddIngredient(ItemID.EndurancePotion).
                AddIngredient(ItemID.IronskinPotion).
                AddIngredient(ItemID.SwiftnessPotion).
                AddIngredient(ItemID.TitanPotion).
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(50).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
