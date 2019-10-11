using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class GacruxianMollusk : CalamityDamageItem
    {
        public static int BaseDamage = 50;
        public static float Knockback = 5f;
        public static float Speed = 8f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gacruxian Mollusk");
            Tooltip.SetDefault("Releases homing sparks while traveling\n" +
            "Stealth strikes release homing snails that create even more sparks");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.rare = 4;
            item.knockBack = Knockback;
            item.autoReuse = true;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.width = 24;
            item.height = 22;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("GacruxianProj");
            item.shootSpeed = Speed;
            item.value = Item.buyPrice(0, 0, 20, 0);
            item.Calamity().rogue = true;
            //item.maxStack = 999; not consumable because imagine knowing how to fish up more than one of an item
            //item.consumable = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), mod.ProjectileType("GacruxianProj"), damage, knockBack, player.whoAmI, 0f, 1f);
                Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
