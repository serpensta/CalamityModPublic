using CalamityMod.World;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Systems
{
    public class WorldSelectionDifficultySystem : ModSystem
    {
        // Since CalamityWorld is static, and therefore invalid for TryGetHeaderData, make copies for Rev and Death
        public override void SaveWorldHeader(TagCompound tag)
        {
            tag["RevengeanceMode"] = CalamityWorld.revenge;
            tag["DeathMode"] = CalamityWorld.death;
        }
    }
}
