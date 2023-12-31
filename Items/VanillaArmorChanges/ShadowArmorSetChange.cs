﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class ShadowArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.ShadowHelmet;

        public override int? BodyPieceID => ItemID.ShadowScalemail;

        public override int? LegPieceID => ItemID.ShadowGreaves;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.AncientShadowHelmet };
        public override int[] AlternativeBodyPieceIDs => new int[] { ItemID.AncientShadowScalemail };
        public override int[] AlternativeLegPieceIDs => new int[] { ItemID.AncientShadowGreaves };

        public override string ArmorSetName => "Shadow";

        public const float ArmorPieceDamage = 0.05f;
        public const float ArmorPieceJumpBoost = 0.07f;

        private void ApplyAnyPieceEffect(Player player)
        {
            // Remove the vanilla crit chance buff and replace it with damage
            player.GetCritChance<GenericDamageClass>() -= 0.05f;
            player.GetDamage<GenericDamageClass>() += ArmorPieceDamage;

            // Give jump boost
            player.jumpSpeedBoost += 5f * ArmorPieceJumpBoost;
        }

        public override void ApplyHeadPieceEffect(Player player) => ApplyAnyPieceEffect(player);

        public override void ApplyBodyPieceEffect(Player player) => ApplyAnyPieceEffect(player);

        public override void ApplyLegPieceEffect(Player player) => ApplyAnyPieceEffect(player);
    }
}
