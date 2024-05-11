using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class HardenedEutrophicSand : ModTile
    {
        public TileFraming.MergeFrameData tileAdjacency;
        public TileFraming.MergeFrameData secondTileAdjacency;
        public TileFraming.MergeFrameData thirdTileAdjacency;
        public TileFraming.MergeFrameData fourthTileAdjacency;
        public TileFraming.MergeFrameData fifthTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);

            Main.tileShine[Type] = 2200;
            Main.tileShine2[Type] = false;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 108;
            AddMapEntry(new Color(67, 107, 143));

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<EutrophicSand>(), "CalamityMod/Tiles/Merges/EutrophicSandMerge", out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<Navystone>(), "CalamityMod/Tiles/Merges/NavystoneMerge", out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sandstone, "CalamityMod/Tiles/Merges/SandstoneMerge", out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.HardenedSand, "CalamityMod/Tiles/Merges/HardenedSandMerge", out fourthTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, "CalamityMod/Tiles/Merges/SandMerge", out fifthTileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, tileAdjacency, thirdTileAdjacency, fourthTileAdjacency, fifthTileAdjacency);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<EutrophicSand>(), tileAdjacency);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<Navystone>(), secondTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Sandstone, thirdTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.HardenedSand, fourthTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, fifthTileAdjacency);
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
