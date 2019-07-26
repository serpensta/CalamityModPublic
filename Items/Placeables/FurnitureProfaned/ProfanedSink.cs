using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
	public class ProfanedSink: ModItem
	{
		public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Counts as a lava source");
        }

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("ProfanedSink");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ProfanedRock", 6);
            recipe.AddIngredient(ItemID.LavaBucket);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "ProfanedBasin");
            recipe.AddRecipe();
        }
	}
}
