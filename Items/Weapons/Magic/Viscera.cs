using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Viscera : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public const int BoomLifetime = 30;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 52;
            Item.damage = 200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.useTime = 9;
            Item.useAnimation = 28;
            Item.reuseDelay = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VisceraBeam>();
            Item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item20, position);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
