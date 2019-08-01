﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
	public class SlimeGod : ModNPC
	{
        private float bossLife;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ebonian Slime God");
			Main.npcFrameCount[npc.type] = 6;
		}

		public override void SetDefaults() //dust is 173
		{
			npc.damage = 60;
			npc.width = 150;
			npc.height = 92;
			npc.scale = 1.1f;
			npc.defense = 15;
            npc.lifeMax = CalamityWorld.revenge ? 5500 : 4000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 7700;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 2500000 : 2200000;
            }
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
			aiType = -1;
			npc.buffImmune[mod.BuffType("GlacialState")] = true;
			npc.buffImmune[mod.BuffType("TemporalSadness")] = true;
			npc.knockBackResist = 0f;
			animationType = 50;
			npc.value = 0f;
			npc.alpha = 60;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SlimeGod");
            else
                music = MusicID.Boss1;
        }

		public override void AI()
		{
			CalamityGlobalNPC.slimeGodPurple = npc.whoAmI;
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
            Vector2 vector = npc.Center;
            if (Vector2.Distance(Main.player[npc.target].Center, vector) > 5400f)
            {
                npc.position.X = (float)(Main.player[npc.target].Center.X / 16) * 16f - (float)(npc.width / 2);
                npc.position.Y = ((float)(Main.player[npc.target].Center.Y / 16) * 16f - (float)(npc.height / 2)) - 150f;
            }
            if ((double)npc.life <= (double)npc.lifeMax * 0.5 && Main.netMode != 1)
            {
                Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 1);
                Vector2 spawnAt = vector + new Vector2(0f, (float)npc.height / 2f);
                NPC.NewNPC((int)spawnAt.X - 30, (int)spawnAt.Y, mod.NPCType("SlimeGodSplit"));
                NPC.NewNPC((int)spawnAt.X + 30, (int)spawnAt.Y, mod.NPCType("SlimeGodSplit"));
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            bool flag100 = false;
            bool hyperMode = false;
			if (CalamityGlobalNPC.slimeGodRed != -1)
			{
				if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
				{
					flag100 = true;
				}
			}
            if (CalamityGlobalNPC.slimeGod < 0 || !Main.npc[CalamityGlobalNPC.slimeGod].active)
            {
                flag100 = false;
                hyperMode = true;
            }
            if (Main.netMode != 1)
			{
                npc.localAI[0] += flag100 ? 1f : 2f;
                if (expertMode && Main.rand.Next(2) == 0)
				{
					if (npc.localAI[0] >= 600f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							float num179 = (CalamityWorld.bossRushActive ? 18f : 8f);
							Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
							float num181 = Math.Abs(num180) * 0.1f;
							float num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y - num181;
							float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							npc.netUpdate = true;
							num183 = num179 / num183;
							num180 *= num183;
							num182 *= num183;
							int num184 = 24;
							int num185 = mod.ProjectileType("AbyssMine");
							value9.X += num180;
							value9.Y += num182;
							for (int num186 = 0; num186 < 3; num186++)
							{
								num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
								num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y;
								num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
								num183 = num179 / num183;
								num180 += (float)Main.rand.Next(-60, 61);
								num182 += (float)Main.rand.Next(-60, 61);
								num180 *= num183;
								num182 *= num183;
								Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
				else if (npc.localAI[0] >= 600f)
				{
					npc.localAI[0] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float num179 = (CalamityWorld.bossRushActive ? 18f : 8f);
						Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
						float num181 = Math.Abs(num180) * 0.1f;
						float num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y - num181;
						float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
						npc.netUpdate = true;
						num183 = num179 / num183;
						num180 *= num183;
						num182 *= num183;
						int num184 = expertMode ? 19 : 21;
						int num185 = mod.ProjectileType("AbyssBallVolley");
						value9.X += num180;
						value9.Y += num182;
						for (int num186 = 0; num186 < 4; num186++)
						{
							num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
							num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y;
							num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							num183 = num179 / num183;
							num180 += (float)Main.rand.Next(-60, 61);
							num182 += (float)Main.rand.Next(-60, 61);
							num180 *= num183;
							num182 *= num183;
							Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
            npc.aiAction = 0;
            npc.knockBackResist = 0.2f * Main.knockBackMultiplier;
            npc.noTileCollide = false;
            npc.noGravity = false;
            npc.reflectingProjectiles = false;
            if (npc.ai[0] != 7f && Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
                if (Main.player[npc.target].dead)
                {
                    npc.ai[0] = 7f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
            }
            else if (npc.timeLeft < 1800)
			{
				npc.timeLeft = 1800;
			}
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                Vector2 vector271 = Main.player[npc.target].Center - vector;
                npc.ai[0] = 1f;
                npc.ai[1] = 0f;
            }
            else if (npc.ai[0] == 1f)
            {
            	npc.ai[1] += 1f;
                if (npc.ai[1] > 18f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                if ((Main.player[npc.target].Center - vector).Length() > (hyperMode ? 1200f : 2400f)) //1200
                {
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                if (npc.velocity.Y == 0f)
                {
                    npc.TargetClosest(true);
                    npc.velocity.X = npc.velocity.X * 0.8f;
                    npc.ai[1] += flag100 ? 1f : 3f;
                    float num1879 = 15f + 30f * ((float)npc.life / (float)npc.lifeMax);
                    float num1880 = 3f + 4f * (1f - (float)npc.life / (float)npc.lifeMax);
                    float num1881 = 8f;
                    if (!Collision.CanHit(vector, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        num1881 += 2f;
                    }
                    if (npc.ai[1] > num1879)
                    {
                        npc.ai[3] += 1f;
                        if (npc.ai[3] >= 2f)
                        {
                            npc.ai[3] = 0f;
                            num1881 *= 2f;
                            num1880 /= 2f;
                        }
                        npc.ai[1] = 0f;
                        npc.velocity.Y = npc.velocity.Y - num1881;
                        npc.velocity.X = num1880 * (float)npc.direction;
                    }
                }
                else
                {
                    npc.knockBackResist = 0f;
                    npc.velocity.X = npc.velocity.X * 0.99f;
                    if (npc.direction < 0 && npc.velocity.X > -1f)
                    {
                        npc.velocity.X = -1f;
                    }
                    if (npc.direction > 0 && npc.velocity.X < 1f)
                    {
                        npc.velocity.X = 1f;
                    }
                }
                npc.ai[2] += 1f;
                if ((double)npc.ai[2] > 360.0 && npc.velocity.Y == 0f && Main.netMode != 1)
                {
                    int num1882 = Main.rand.Next(3);
                    if (num1882 == 0)
                    {
                        npc.ai[0] = 3f;
                    }
                    else if (num1882 == 1)
                    {
                        npc.ai[0] = 4f;
                        npc.noTileCollide = true;
                        npc.velocity.Y = -8f;
                    }
                    else if (num1882 == 2)
                    {
                        npc.ai[0] = 6f;
                    }
                    else
                    {
                        npc.ai[0] = 2f;
                    }
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.velocity.X = npc.velocity.X * 0.85f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 80f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                npc.noTileCollide = true;
                npc.noGravity = true;
                npc.knockBackResist = 0f;
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                npc.spriteDirection = npc.direction;
                npc.TargetClosest(true);
                Vector2 center40 = Main.player[npc.target].Center;
                center40.Y -= 350f;
                Vector2 vector272 = center40 - vector;
                if (npc.ai[2] == 1f)
                {
                    npc.ai[1] += 1f;
                    vector272 = Main.player[npc.target].Center - vector;
                    vector272.Normalize();
                    vector272 *= 8f;
                    npc.velocity = (npc.velocity * 4f + vector272) / 5f;
                    if (npc.ai[1] > 6f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[0] = 4.1f;
                        npc.ai[2] = 0f;
                        npc.velocity = vector272;
                    }
                }
                else
                {
                    if (Math.Abs(vector.X - Main.player[npc.target].Center.X) < 40f && vector.Y < Main.player[npc.target].Center.Y - 300f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[2] = 1f;
                        return;
                    }
                    vector272.Normalize();
                    vector272 *= 12f;
                    npc.velocity = (npc.velocity * 5f + vector272) / 6f;
                }
            }
            else if (npc.ai[0] == 4.1f)
            {
                npc.knockBackResist = 0f;
                if (npc.ai[2] == 0f && Collision.CanHit(vector, 1, 1, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.ai[2] = 1f;
                }
                if (npc.position.Y + (float)npc.height >= Main.player[npc.target].position.Y || npc.velocity.Y <= 0f)
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 10f)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.ai[0] = 5f;
                        }
                    }
                }
                else if (npc.ai[2] == 0f)
                {
                    npc.noTileCollide = true;
                    npc.noGravity = true;
                    npc.knockBackResist = 0f;
                }
                npc.velocity.Y = npc.velocity.Y + 0.2f;
                if (npc.velocity.Y > 16f)
                {
                    npc.velocity.Y = 16f;
                }
            }
            else
            {
                if (npc.ai[0] == 5f)
                {
                	npc.localAI[0] += (float)Main.rand.Next(4, 9);
                    if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    else
                    {
                        npc.direction = -1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.noTileCollide = true;
                    npc.noGravity = true;
                    npc.knockBackResist = 0f;
                    Vector2 value74 = Main.player[npc.target].Center - vector;
                    value74.Y -= 4f;
                    if (value74.Length() < 200f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    if (value74.Length() > 10f)
                    {
                        value74.Normalize();
                        value74 *= 10f;
                    }
                    npc.velocity = (npc.velocity * 4f + value74) / 4.8f; //5
                    return;
                }
                if (npc.ai[0] == 6f)
                {
                    npc.knockBackResist = 0f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.TargetClosest(true);
                        npc.velocity.X = npc.velocity.X * 0.8f;
                        npc.ai[1] += 1f;
                        if (npc.ai[1] > 5f)
                        {
                            npc.ai[1] = 0f;
                            npc.velocity.Y = npc.velocity.Y - 4f;
                            if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height < vector.Y)
                            {
                                npc.velocity.Y = npc.velocity.Y - 1.25f;
                            }
                            if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height < vector.Y - 40f)
                            {
                                npc.velocity.Y = npc.velocity.Y - 1.5f;
                            }
                            if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height < vector.Y - 80f)
                            {
                                npc.velocity.Y = npc.velocity.Y - 1.75f;
                            }
                            if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height < vector.Y - 120f)
                            {
                                npc.velocity.Y = npc.velocity.Y - 2f;
                            }
                            if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height < vector.Y - 160f)
                            {
                                npc.velocity.Y = npc.velocity.Y - 2.25f;
                            }
                            if (Main.player[npc.target].position.Y + (float)Main.player[npc.target].height < vector.Y - 200f)
                            {
                                npc.velocity.Y = npc.velocity.Y - 2.5f;
                            }
                            if (!Collision.CanHit(vector, 1, 1, Main.player[npc.target].Center, 1, 1))
                            {
                                npc.velocity.Y = npc.velocity.Y - 2f;
                            }
                            npc.velocity.X = (float)(12 * npc.direction);
                            npc.ai[2] += 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X * 0.98f;
                        if (npc.direction < 0 && npc.velocity.X > -8f)
                        {
                            npc.velocity.X = -8f;
                        }
                        if (npc.direction > 0 && npc.velocity.X < 8f)
                        {
                            npc.velocity.X = 8f;
                        }
                    }
                    if (npc.ai[2] >= 3f && npc.velocity.Y == 0f)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                }
                else if (npc.ai[0] == 7f)
                {
                    npc.life = npc.lifeMax;
                    npc.defense = 9999;
                    npc.noTileCollide = true;
                    npc.alpha += 7;
                    if (npc.timeLeft > 10)
					{
						npc.timeLeft = 10;
					}
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                    }
                    npc.velocity.X = npc.velocity.X * 0.98f;
                }
            }
			int num244 = Dust.NewDust(npc.position, npc.width, npc.height, 173, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
			Main.dust[num244].noGravity = true;
			Main.dust[num244].velocity *= 0.5f;
			if (bossLife == 0f && npc.life > 0)
			{
				bossLife = (float)npc.lifeMax;
			}
	       	float num644 = 1f;
	       	if (npc.life > 0)
			{
				float num659 = (float)npc.life / (float)npc.lifeMax;
				num659 = num659 * 0.5f + 0.75f;
				num659 *= num644;
				if (num659 != npc.scale)
				{
					npc.position.X = npc.position.X + (float)(npc.width / 2);
					npc.position.Y = npc.position.Y + (float)npc.height;
					npc.scale = num659;
					npc.width = (int)(150f * npc.scale);
					npc.height = (int)(92f * npc.scale);
					npc.position.X = npc.position.X - (float)(npc.width / 2);
					npc.position.Y = npc.position.Y - (float)npc.height;
				}
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.15);
					if ((float)(npc.life + num660) < bossLife)
					{
						bossLife = (float)npc.life;
						int num661 = Main.rand.Next(1, 3);
						for (int num662 = 0; num662 < num661; num662++)
						{
							int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
							int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
							int num663 = mod.NPCType("SlimeSpawnCorrupt");
							int num664 = NPC.NewNPC(x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num664].SetDefaults(num663, -1f);
							Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
							Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
							Main.npc[num664].ai[0] = (float)(-1000 * Main.rand.Next(3));
							Main.npc[num664].ai[1] = 0f;
							if (Main.netMode == 2 && num664 < 200)
							{
								NetMessage.SendData(23, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
							}
						}
					}
				}
			}
		}

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        // If the un-split Ebonian Slime God gets one-shotted last, it should drop the boss loot
        public override void NPCLoot()
        {
            bool otherSlimeGodsAlive =
                NPC.AnyNPCs(mod.NPCType("SlimeGodCore")) ||
                NPC.AnyNPCs(mod.NPCType("SlimeGod")) ||
                NPC.AnyNPCs(mod.NPCType("SlimeGodSplit")) ||
                NPC.AnyNPCs(mod.NPCType("SlimeGodRun")) ||
                NPC.AnyNPCs(mod.NPCType("SlimeGodRunSplit"));
            if (!otherSlimeGodsAlive)
                SlimeGodCore.DropSlimeGodLoot(npc);
        }

        public override bool CheckActive()
        {
			if (CalamityGlobalNPC.slimeGod != -1)
			{
				if (Main.npc[CalamityGlobalNPC.slimeGod].active)
					return false;
			}
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Weak, 180, true);
		}
	}
}
