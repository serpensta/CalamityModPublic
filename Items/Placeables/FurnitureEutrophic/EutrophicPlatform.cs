using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureEutrophic
{
    public class EutrophicPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 8;
            item.height = 10;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = mod.TileType("EutrophicPlatform");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("Navystone"));
            recipe.SetResult(this, 2);
            recipe.AddTile(null, "EutrophicCrafting");
            recipe.AddRecipe();
        }
    }
}
