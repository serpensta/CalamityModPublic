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
	public class NightsRay : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Night's Ray");
			Tooltip.SetDefault("Fires a dark ray that splits if enemies are near it");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 35;
	        item.magic = true;
	        item.mana = 10;
	        item.width = 50;
	        item.height = 50;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.25f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
	        item.UseSound = SoundID.Item72;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("NightRay");
	        item.shootSpeed = 6f;
	    }

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.WandofSparking);
	        recipe.AddIngredient(ItemID.Vilethorn);
	        recipe.AddIngredient(ItemID.AmberStaff);
	        recipe.AddIngredient(ItemID.MagicMissile);
	        recipe.AddIngredient(null, "TrueShadowScale", 15);
	        recipe.AddIngredient(null, "PurifiedGel", 10);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
