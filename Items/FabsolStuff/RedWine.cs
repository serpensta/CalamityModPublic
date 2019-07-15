using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class RedWine : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Red Wine");
			Tooltip.SetDefault(@"Reduces life regen by 1
Too dry for my taste");
		}

		public override void SetDefaults()
		{
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 1;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.healLife = 200;
            item.consumable = true;
            item.potion = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
		}

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(mod.BuffType("RedWine"), 900);
            return true;
        }
    }
}
