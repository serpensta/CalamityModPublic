﻿using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Calamity : ModItem
    {
        public const float MaxNPCSpeed = 5f;

        // This is ONLY the direct DPS of having the cursor over the enemy, not the damage from the flames debuff.
        // The debuff is VulnerabilityHex, check that file for its DPS.
        public const int BaseDamage = 266;
        public const int HitsPerSecond = 12;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Calamity");
            Tooltip.SetDefault("Lights your cursor ablaze with the Witch's flames, engulfing it in a blazing aura\n" +
                "Enemies touching the blaze take immense damage and are inflicted with Vulnerability Hex\n" +
                "Equip in a vanity slot to change the cursor without dealing damage\n" +
                "These changes work in conjunction with the Rainbow Cursor");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 108;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.rare = ItemRarityID.Purple;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().blazingCursorDamage = true;
            player.Calamity().blazingCursorVisuals = true;
        }

        public override void UpdateEquip(Player player) => player.Calamity().blazingCursorVisuals = true;
    }
}
