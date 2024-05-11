
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Ores
{
    public class UelibloomOre : ModTile
    {
        public TileFraming.MergeFrameData tileAdjacency;
        public TileFraming.MergeFrameData secondTileAdjacency;
        public TileFraming.MergeFrameData thirdTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 950;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            AddMapEntry(new Color(0, 255, 0), CreateMapEntryName());
            MineResist = 3f;
            MinPick = 225;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;


            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, "CalamityMod/Tiles/Merges/DirtMerge", out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, "CalamityMod/Tiles/Merges/StoneMerge", out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Mud, "CalamityMod/Tiles/Merges/MudMerge", out thirdTileAdjacency);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, secondTileAdjacency, tileAdjacency);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, tileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, secondTileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Mud, thirdTileAdjacency);
            return true;
        }
    }
}
