using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class CorruptionEffigy : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.createTile = ModContent.TileType<Tiles.Furniture.CorruptionEffigy>();
        }
    }
}
