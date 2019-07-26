using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class Grax : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grax");
			Tooltip.SetDefault("Hitting an enemy will greatly boost your defense and melee stats for a short time");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 450;
			item.melee = true;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.useTime = 5;
			item.useTurn = true;
			item.axe = 50;
			item.hammer = 200;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "FellerofEvergreens");
			recipe.AddIngredient(null, "DraedonBar", 5);
			recipe.AddRecipeGroup("LunarHamaxe");
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			player.AddBuff(mod.BuffType("GraxDefense"), 480);
		}
	}
}
