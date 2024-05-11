using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralSand : ModTile
    {
        public TileFraming.MergeFrameData tileAdjacency;
        public TileFraming.MergeFrameData secondTileAdjacency;
        public TileFraming.MergeFrameData thirdTileAdjacency;
        public TileFraming.MergeFrameData fourthTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSand[Type] = true;
            Main.tileBrick[Type] = true;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Sand"]);

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = 108;

            AddMapEntry(new Color(187, 220, 237));

            TileID.Sets.Suffocate[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Conversion.Sand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.Falling[Type] = true;
            TileID.Sets.FallingBlockProjectile[Type] = new TileID.Sets.FallingBlockProjectileInfo(ModContent.ProjectileType<AstralSandBallFalling>(), 15);

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, "CalamityMod/Tiles/Merges/DirtMerge", out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, "CalamityMod/Tiles/Merges/StoneMerge", out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralDirt>(), "CalamityMod/Tiles/Merges/AstralDirtMerge", out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, "CalamityMod/Tiles/Merges/SandMerge", out fourthTileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, fourthTileAdjacency, thirdTileAdjacency, secondTileAdjacency, tileAdjacency);
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, tileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, secondTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralDirt>(), thirdTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, fourthTileAdjacency);
            return true;
        }

        public override bool HasWalkDust()
        {
            return Main.rand.NextBool(3);
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            DustType = 108;
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
