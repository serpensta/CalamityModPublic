﻿using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Perforator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.TreasureBags
{
    public class PerforatorBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<PerforatorHive>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (The Perforators)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Cyan;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void OpenBossBag(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetSource_OpenItem(Item.type);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<BloodSample>(), 45, 60);
            DropHelper.DropItem(s, player, ItemID.CrimtaneBar, 15, 20);
            DropHelper.DropItem(s, player, ItemID.Vertebrae, 15, 20);
            DropHelper.DropItemCondition(s, player, ItemID.Ichor, Main.hardMode, 15, 30);
            DropHelper.DropItem(s, player, ItemID.CrimsonSeeds, 10, 15);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<VeinBurster>(w),
                DropHelper.WeightStack<BloodyRupture>(w),
                DropHelper.WeightStack<SausageMaker>(w),
                DropHelper.WeightStack<Aorta>(w),
                DropHelper.WeightStack<Eviscerator>(w),
                DropHelper.WeightStack<BloodBath>(w),
                DropHelper.WeightStack<BloodClotStaff>(w),
                DropHelper.WeightStack<ToothBall>(w, 50, 75),
                DropHelper.WeightStack<BloodstainedGlove>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<BloodyWormTooth>());

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<PerforatorMask>(), 7);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<BloodyVein>(), 10);
        }
    }
}
