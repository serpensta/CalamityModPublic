﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DevourerMunsters
{
    public class RuneofCos : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune of Kos");
            Tooltip.SetDefault("A relic of the profaned flame\n" +
                "Contains the power hunted relentlessly by the sentinels of the cosmic devourer\n" +
                "When used in certain areas of the world it will unleash them\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.rare = 9;
            item.UseSound = SoundID.Item44;
            item.consumable = false;
            item.shoot = mod.ProjectileType("VoidSpawn");
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool CanUseItem(Player player)
        {
            return (player.ZoneSkyHeight || player.ZoneUnderworldHeight || player.ZoneDungeon) &&
                (!NPC.AnyNPCs(mod.NPCType("StormWeaverHead")) && !NPC.AnyNPCs(mod.NPCType("StormWeaverHeadNaked")) && !NPC.AnyNPCs(mod.NPCType("CeaselessVoid")) && !NPC.AnyNPCs(mod.NPCType("CosmicWraith")));
        }

        public override bool UseItem(Player player)
        {
            if (player.ZoneDungeon)
            {
                for (int num662 = 0; num662 < 2; num662++)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("DarkEnergySpawn"), 0, 0f, Main.myPlayer, 0f, 0f);
                }
                NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("CeaselessVoid"));
            }
            else if (player.ZoneUnderworldHeight)
            {
                NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("CosmicWraith"));
            }
            else if (player.ZoneSkyHeight)
            {
                NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("StormWeaverHead"));
            }
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UnholyEssence", 40);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.FragmentSolar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
