﻿using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    [LegacyName("MonolithCrafting")]
    public class MonolithAmalgam : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // Tooltip.SetDefault("Used for special crafting");
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Monolith Amalgam");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.MonolithAmalgam>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralMonolith>(20).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
