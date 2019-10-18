using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Tiles.FurnitureOccult
{
    class OccultSofa : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpSofa(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Otherworldly Sofa");
            AddMapEntry(new Color(191, 142, 111), name);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(125, 94, 128), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<OccultTileCloth>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<Items.OccultSofa>());
        }
    }
}
