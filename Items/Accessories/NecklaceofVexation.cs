﻿using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class NecklaceofVexation : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.vexation = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AvengerEmblem).
                AddIngredient<PerennialBar>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
