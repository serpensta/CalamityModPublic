﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class CrownJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Crown Jewel");
            /* Tooltip.SetDefault("Boosts life regen even while under the effects of a damaging debuff\n" +
                "While under the effects of a damaging debuff you will gain 8 defense"); */
        }

        public override void SetDefaults()
        {
            Item.defense = 5;
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.crownJewel = true;
        }
    }
}
