using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ApolloMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollo Mask");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }

        public override bool DrawHead() => false;
    }
}
