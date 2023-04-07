﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class ElectriciansGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Electrician's Glove");
            // Tooltip.SetDefault(@"Stealth strikes summon sparks on enemy hits
Stealth strikes also have +8 armor penetration, deal 8% more damage, and heal for 2 HP");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 40;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.electricianGlove = true;
            modPlayer.bloodyGlove = true;
            modPlayer.filthyGlove = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FilthyGlove>().
                AddIngredient(ItemID.Wire, 100).
                AddRecipeGroup("AnyMythrilBar", 5).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient<BloodstainedGlove>().
                AddIngredient(ItemID.Wire, 100).
                AddRecipeGroup("AnyMythrilBar", 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
