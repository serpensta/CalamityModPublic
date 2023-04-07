﻿using CalamityMod.Rarities;
using CalamityMod.UI;
using CalamityMod.UI.DraedonLogs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class DraedonsLogSnowBiome : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // DisplayName.SetDefault("Cryonic Material Log");
            // Tooltip.SetDefault("[Material Science Report] - Archmage's 'Cryonic Metal'\nClick to view its contents");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                PopupGUIManager.FlipActivityOfGUIWithType(typeof(DraedonLogSnowBiomeGUI));
            return true;
        }
    }
}
