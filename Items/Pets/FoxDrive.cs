﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class FoxDrive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fox Drive");
            Tooltip.SetDefault("'It contains 1 file on it'\n'Fox.cs'");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = mod.ProjectileType("Fox");
            item.buffType = mod.BuffType("Fox");
            item.rare = 9;
            item.expert = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
