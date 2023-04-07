using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Ores
{
    public class AerialiteOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            // DisplayName.SetDefault("Aerialite Ore");
			ItemID.Sets.SortingPriorityMaterials[Type] = 69; // Hellstone
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Ores.AerialiteOre>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 6);
            Item.rare = ItemRarityID.Orange;
        }
    }
}
