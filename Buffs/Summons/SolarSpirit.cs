﻿using CalamityMod.CalPlayer;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summons
{
    public class SolarSpirit : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Solar Spirit");
            Description.SetDefault("The solar spirit will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SolarPixie>()] > 0)
            {
                modPlayer.SP = true;
            }
            if (!modPlayer.SP)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
