using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class AcidEel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Eel");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 72;
            npc.height = 18;

            npc.damage = 40;
            npc.lifeMax = 260;
            npc.defense = 4;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 160;
                npc.lifeMax = 6650;
                npc.defense = 45;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 80;
                npc.lifeMax = 705;
            }

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 3, 32);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AcidEelBanner>();
        }
        public override void AI()
        {
            npc.TargetClosest(false);

            if (Main.rand.NextBool(480))
                Main.PlaySound(SoundID.Zombie, npc.Center, 32); // Slither sound

            if (npc.wet)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] % 150f == 0f || npc.direction == 0)
                {
                    npc.direction = (Main.player[npc.target].position.X > npc.position.X).ToDirectionInt();
                }
                float acceleration = 0.3f;
                float yAcceleration = 0.08f;
                float maxSpeedX = 15f;
                float maxSpeedY = 4f;
                if (CalamityWorld.downedPolterghast)
                {
                    acceleration = 0.75f;
                    yAcceleration = 0.25f;
                    maxSpeedX = 24f;
                    maxSpeedY = 9f;
                }
                npc.velocity.X += npc.direction * acceleration;

                if (npc.collideX)
                    npc.direction *= -1;

                npc.spriteDirection = npc.direction;

                npc.velocity.Y += (Main.player[npc.target].position.Y > npc.position.Y).ToDirectionInt() * yAcceleration;
                npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-maxSpeedX, -maxSpeedY), new Vector2(maxSpeedX, maxSpeedY));
                npc.rotation = npc.velocity.X * 0.02f;
            }
            else
            {
                npc.rotation = npc.rotation.AngleLerp(0f, 0.1f);
                npc.velocity.X *= 0.95f;
                if (npc.velocity.Y < 14f)
                    npc.velocity.Y += 0.15f;
            }
        }
        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulfuricScale>(), 2 * (CalamityWorld.downedAquaticScourge ? 6 : 1), 1, 3);
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 5)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AcidEelGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AcidEelGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AcidEelGore3"), npc.scale);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
