﻿using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    public class CrimsonSlimeSpawn2 : ModNPC
    {
        public float spikeTimer = 60f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crimson Slime Spawn");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Slime;
            NPC.GetNPCDamage();
            NPC.width = 40;
            NPC.height = 30;
            NPC.defense = 6;
            NPC.lifeMax = 130;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 12000;
            }
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0.8f;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<SplitCrimulanSlimeGod>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Embedded in these slime's bodies are shards of crimulan stone, which drip with a toxin meant to impair your vision.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            int frameY = 1;
            if (!Main.dedServ)
            {
                if (TextureAssets.Npc[NPC.type].Value is null)
                    return;
                frameY = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            }
            int aiState = 0;
            if (NPC.aiAction == 0)
                aiState = NPC.velocity.Y >= 0f ? (NPC.velocity.Y <= 0f ? (NPC.velocity.X == 0f ? 0 : 1) : 3) : 2;
            else if (NPC.aiAction == 1)
                aiState = 4;

            NPC.frameCounter++;
            if (aiState > 0)
                NPC.frameCounter++;
            if (aiState == 4)
                NPC.frameCounter++;
            if (NPC.frameCounter >= 8f)
            {
                NPC.frame.Y += frameY;
                NPC.frameCounter = 0f;
            }
            if (NPC.frame.Y >= frameY * Main.npcFrameCount[NPC.type])
                NPC.frame.Y = 0;
        }

        public override void AI()
        {
            if (spikeTimer > 0f)
                spikeTimer -= 1f;

            int type = ModContent.ProjectileType<CrimsonSpike>();
            int damage = NPC.GetProjectileDamage(type);
            if (CalamityWorld.getFixedBoi)
            {
                type = Main.rand.NextBool(2) ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
            }
            if (!NPC.wet)
            {
                Vector2 vector3 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float num14 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector3.X;
                float num15 = Main.player[NPC.target].position.Y - vector3.Y;
                float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                if (Main.expertMode && num16 < 120f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && NPC.velocity.Y == 0f)
                {
                    NPC.ai[0] = -40f;
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.9f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && spikeTimer == 0f)
                    {
                        int projcount = CalamityWorld.getFixedBoi ? 12 : 5;
                        for (int n = 0; n < projcount; n++)
                        {
                            Vector2 vector4 = new Vector2((float)(n - 2), -4f);
                            vector4.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                            vector4.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                            vector4.Normalize();
                            vector4 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), vector3.X, vector3.Y, vector4.X, vector4.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            spikeTimer = 30f;
                        }
                    }
                }
                else if (num16 < 360f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && NPC.velocity.Y == 0f)
                {
                    NPC.ai[0] = -40f;
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.9f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && spikeTimer == 0f)
                    {
                        num15 = Main.player[NPC.target].position.Y - vector3.Y - (float)Main.rand.Next(0, 200);
                        num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                        num16 = 6.5f / num16;
                        num14 *= num16;
                        num15 *= num16;
                        spikeTimer = 50f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), vector3.X, vector3.Y, num14, num15, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Color dustColor = Color.Crimson;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, NPC.alpha, dustColor, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, NPC.alpha, dustColor, 1f);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(8) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemID.Blindfold, 50);

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (damage > 0)
                player.AddBuff(BuffID.Darkness, 90, true);
        }
    }
}
