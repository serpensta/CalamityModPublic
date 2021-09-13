using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BlushieStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Blushie");
            Tooltip.SetDefault("Hold your mouse, wait, wait, wait, and put your trust in the power of blue magic");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 48;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useAnimation = 30;
            item.useTime = 30;
            item.channel = true;
            item.noMelee = true;
            item.damage = 1;
            item.knockBack = 1f;
            item.autoReuse = false;
            item.useTurn = false;
            item.magic = true;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;

            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<BlushieStaffProj>();
            item.mana = 200;
            item.shootSpeed = 0f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SapphireStaff);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
