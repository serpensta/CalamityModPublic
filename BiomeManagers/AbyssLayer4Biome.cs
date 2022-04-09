﻿using CalamityMod.CalPlayer;
using CalamityMod.Waters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbyssLayer4Biome : ModBiome
    {
        public override int Music
        {
            get
            {
                if (CalamityPlayer.areThereAnyDamnBosses)
                    return Main.curMusic;
                return CalamityMod.Instance.GetMusicFromMusicMod("Abyss3") ?? MusicID.Hell;
            }
        }

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AbyssWater");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fourth Layer of the Abyss");
        }

        public override bool IsBiomeActive(Player player)
        {
            return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                playerYTileCoords > Main.rockLayer + Main.maxTilesY * 0.26;
        }
    }
}
