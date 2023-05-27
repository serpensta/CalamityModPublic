﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class Regenator : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 56;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 10;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.regenator = true;

            //Hard-lock the player's health to 50%.
            //No lifesteal, no regen, no healing pots
            if (player.statLife >= (int)(player.statLifeMax2 * 0.5f))
            {
                player.statLife = (int)(player.statLifeMax2 * 0.5f);    
                player.moonLeech = true;
                modPlayer.healingPotBonus = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;
            }
        }
    }
}
