using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenPiano : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Ashen Pipe Organ");
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.value = 0;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAshen.AshenPiano>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Bone, 4).AddIngredient(ModContent.ItemType<SmoothBrimstoneSlag>(), 15).AddIngredient(ItemID.Book).AddTile(ModContent.TileType<AshenAltar>()).Register();
        }
    }
}
