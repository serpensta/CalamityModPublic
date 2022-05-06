﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Summon
{
    public class WitherBlossomsStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wither Blossoms Staff");
            Tooltip.SetDefault("Summons four decaying flowers over your head\n" +
                               "The combined flowers consume two minion slots");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 61;
            Item.mana = 10;
            Item.width = 52;
            Item.height = 60;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item46;
            Item.shoot = ModContent.ProjectileType<WitherBlossom>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 4; //If you already have all 4, no need to resummon

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(false, type, player);
            for (int i = 0; i < 4; i++)
            {
                Projectile blossom = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
                blossom.ai[0] = MathHelper.TwoPi * i / 4f;
                blossom.rotation = blossom.ai[0];
                blossom.originalDamage = Item.damage;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TundraFlameBlossomsStaff>().
                AddIngredient<PlagueCellCluster>(15).
                AddIngredient<CoreofCalamity>(5).
                AddIngredient<BarofLife>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
