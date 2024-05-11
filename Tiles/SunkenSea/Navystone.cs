using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class Navystone : ModTile
    {
        public TileFraming.MergeFrameData tileAdjacency;
        public TileFraming.MergeFrameData secondTileAdjacency;
        public TileFraming.MergeFrameData thirdTileAdjacency;
        public TileFraming.MergeFrameData fourthTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);

            TileID.Sets.ChecksForMerge[Type] = true;
            DustType = 96;
            AddMapEntry(new Color(31, 92, 114));
            HitSound = SoundID.Tink;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<EutrophicSand>(), "CalamityMod/Tiles/Merges/EutrophicSandMerge", out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sandstone, "CalamityMod/Tiles/Merges/SandstoneMerge", out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, "CalamityMod/Tiles/Merges/SandMerge", out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.HardenedSand, "CalamityMod/Tiles/Merges/HardenedSandMerge", out fourthTileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, secondTileAdjacency, thirdTileAdjacency, fourthTileAdjacency);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<EutrophicSand>(), tileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Sandstone, secondTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, thirdTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.HardenedSand, thirdTileAdjacency);
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
