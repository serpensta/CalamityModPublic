using CalamityMod.NPCs.DevourerofGods;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DoGBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) || Main.LocalPlayer.Calamity().monolithDevourerShader > 0;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:DevourerofGodsHead", isActive);
        }
    }
}
