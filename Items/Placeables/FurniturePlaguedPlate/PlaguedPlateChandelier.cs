using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurniturePlaguedPlate
{
    public class PlaguedPlateChandelier : ModItem
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
            item.createTile = mod.TileType("PlaguedPlateChandelier");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PlaguedPlate", 4);
            recipe.AddIngredient(mod.GetItem("PlagueCellCluster"), 2);
            recipe.AddIngredient(ItemID.Wire, 4);
            recipe.AddIngredient(ItemID.Chain);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "PlagueInfuser");
            recipe.AddRecipe();
        }
    }
}
