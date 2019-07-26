﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class BloodyWormScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Worm Scarf");
            Tooltip.SetDefault("10% increased damage reduction and increased melee stats");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 42;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.expert = true;
			item.rare = 9;
			item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeDamage += 0.1f;
            player.meleeSpeed += 0.1f;
            player.endurance += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodyWormTooth");
            recipe.AddIngredient(ItemID.WormScarf);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
