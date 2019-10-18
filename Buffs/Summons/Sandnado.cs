﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summons
{
    public class Sandnado : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Sandnado");
            Description.SetDefault("The sandnado will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Sandnado>()] > 0)
            {
                modPlayer.sandnado = true;
            }
            if (!modPlayer.sandnado)
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
