using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureAshen
{
    class AshenSink : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpSink(Type, true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ashen Sink");
            AddMapEntry(new Color(191, 142, 111), name);
            adjTiles = new int[] { TileID.Sinks };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<Items.AshenSink>());
        }
    }
}
