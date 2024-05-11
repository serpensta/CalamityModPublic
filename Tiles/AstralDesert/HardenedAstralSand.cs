
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class HardenedAstralSand : ModTile
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
            CalamityUtils.MergeAstralTiles(Type);


            DustType = 108;

            AddMapEntry(new Color(128, 128, 158));

            TileID.Sets.Conversion.HardenedSand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralSand>(), "CalamityMod/Tiles/Merges/AstralSandMerge", out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralSandstone>(), "CalamityMod/Tiles/Merges/AstralSandstoneMerge", out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sandstone, "CalamityMod/Tiles/Merges/SandstoneMerge", out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.HardenedSand, "CalamityMod/Tiles/Merges/HardenedSandMerge", out fourthTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, "CalamityMod/Tiles/Merges/SandMerge", out fifthTileAdjacency);
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, tileAdjacency, thirdTileAdjacency, fourthTileAdjacency, fifthTileAdjacency);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralSand>(), tileAdjacency);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralSandstone>(), secondTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Sandstone, thirdTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.HardenedSand, fourthTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, fifthTileAdjacency);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
