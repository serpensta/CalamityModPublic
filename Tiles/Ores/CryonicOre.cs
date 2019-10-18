
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Ores
{
    public class CryonicOre : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileValue[Type] = 675;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeSnowTiles(Type);

            drop = ModContent.ItemType<Items.CryonicOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cryonic Ore");
            AddMapEntry(new Color(0, 0, 150), name);
            mineResist = 3f;
            minPick = 180;
            soundType = 21;
            Main.tileSpelunker[Type] = true;
        }

        public override bool CanExplode(int i, int j)
        {
            return CalamityWorld.downedCryogen;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, TileID.SnowBlock);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.02f;
            g = 0.02f;
            b = 0.06f;
        }
    }
}
