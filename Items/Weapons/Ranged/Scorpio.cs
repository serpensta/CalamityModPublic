using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("Scorpion")]
    public class Scorpio : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        #region Other stats

        // Weapon stats.
        public static int OriginalUseTime = 60;
        public static int TimeBetweenBursts = 10;
        public static int ProjectilesPerBurst = 6;

        // Small rocket stats.
        public static float EnemyDetectionDistance = 2000f;
        public static float TrackingSpeed = 0.06f; // VERY DELICATE VALUE, CHANGE SLOWLY.

        // Large rocket stats. 
        public static float NukeEnemyDistanceDetection = 320f;
        public static float NukeRequiredRotationProximity = 0.95f;
        public static float NukeTrackingSpeed = 0.01f; // VERY DELICATE VALUE, CHANGE SLOWLY.

        #endregion

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = OriginalUseTime;
            Item.shoot = ModContent.ProjectileType<ScorpioHoldout>();
            Item.shootSpeed = 15f;
            Item.knockBack = 6.5f;

            Item.width = 96;
            Item.height = 42;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Rocket;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/DudFire") with { Volume = .4f, Pitch = -.9f, PitchVariance = 0.1f };
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        // Spawning the holdout won't consume ammo.
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] != 0;

        // Makes the rotation of the mouse around the player sync in multiplayer.
        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<ScorpioHoldout>(), 0, 0f, player.whoAmI);

            // We set the rotation to the direction to the mouse so the first frame doesn't appear bugged out.
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SnowmanCannon).
                AddIngredient(ItemID.FragmentNebula, 6).
                AddIngredient(ItemID.Nanites, 100).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
