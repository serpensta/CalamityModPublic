﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class BoundingBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Bounding");
			Description.SetDefault("Increased jump height, jump speed, and fall damage resistance");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).bounding = true;
		}
	}
}
