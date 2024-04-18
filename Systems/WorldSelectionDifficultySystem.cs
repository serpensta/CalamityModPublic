using System;
using System.Collections.Generic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Systems
{
    public class WorldSelectionDifficultySystem : ModSystem
    {
        public static bool GetShopOracle(AWorldListItem worldList, WorldFileData fileData, ModSystem system, string key)
        {
            return true;
        }
        public override void Load()
        {
            difficulties.Add(new WorldDifficulty(CalamityUtils.GetTextValue("UI.Revengeance"), new Func<AWorldListItem, WorldFileData, ModSystem, string>(GetShopOracle)));
        }

        // Since CalamityWorld is static, and therefore invalid for TryGetHeaderData, make copies for Rev and Death
        public override void SaveWorldHeader(TagCompound tag)
        {
            tag["RevengeanceMode"] = CalamityWorld.revenge;
            tag["DeathMode"] = CalamityWorld.death;
        }

        public record WorldDifficulty(Func<TagCompound, bool> function, string name, Color color);

        public static List<WorldDifficulty> difficulties = new List<WorldDifficulty>();
    }
}
