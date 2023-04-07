﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class OceanCrest : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Ocean Crest");
            // Tooltip.SetDefault("Most ocean enemies become friendly and provides waterbreathing");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.npcTypeNoAggro[NPCID.Shark] = true;
            player.npcTypeNoAggro[NPCID.SeaSnail] = true;
            player.npcTypeNoAggro[NPCID.PinkJellyfish] = true;
            player.npcTypeNoAggro[NPCID.Crab] = true;
            player.npcTypeNoAggro[NPCID.Squid] = true;
            player.gills = true;
        }
    }
}
