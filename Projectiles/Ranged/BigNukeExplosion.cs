﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BigNukeExplosion : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosion");
		}

        public override void SetDefaults()
        {
            projectile.width = 700;
            projectile.height = 700;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
			projectile.ranged = true;
			projectile.penetrate = -1;
            projectile.timeLeft = 5;
        }
    }
}
