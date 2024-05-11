using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryPanels : ModTile
    {
        public static readonly SoundStyle MinePlatingSound = new("CalamityMod/Sounds/Custom/PlatingMine", 3);

        public TileFraming.MergeFrameData tileAdjacency;
        public TileFraming.MergeFrameData secondTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<HazardChevronPanels>());

            HitSound = MinePlatingSound;
            DustType = 109;
            MinPick = 30;
            AddMapEntry(new Color(36, 35, 37));

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, "CalamityMod/Tiles/Merges/DirtMerge", out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, "CalamityMod/Tiles/Merges/StoneMerge", out secondTileAdjacency);
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, tileAdjacency);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, tileAdjacency);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, secondTileAdjacency);
            return true;
        }
    }
}
