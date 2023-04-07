using CalamityMod.Items.Mounts;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Buffs.Mounts
{
    public class AlicornBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Alicorn");
            // Description.SetDefault("You beat DoG while drunk, you are truly fabulous!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<AlicornMount>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().fab = true;
        }
    }
}
