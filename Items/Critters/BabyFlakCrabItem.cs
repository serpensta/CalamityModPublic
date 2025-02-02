﻿using CalamityMod.NPCs.AcidRain;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Critters
{
    public class BabyFlakCrabItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 0, 30, 0);
            //item.CloneDefaults(2004); //Lightning Bug item
            Item.makeNPC = (short)ModContent.NPCType<BabyFlakCrab>();
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}
