﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class HyperiusBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
                   }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(copper: 16);
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<HyperiusBulletProj>();
            Item.shootSpeed = 16f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.MusketBall, 150).
                AddIngredient<LifeAlloy>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
