﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    public class AstrumDeusBody : ModNPC
    {
        private int spawn = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Deus");
        }

        public override void SetDefaults()
        {
            npc.damage = 85;
            npc.npcSlots = 5f;
            npc.width = 38;
            npc.height = 44;
            npc.defense = 40;
            npc.Calamity().RevPlusDR(0.1f);
            npc.LifeMaxNERD(12000, 18000, 29100, 360000, 420000);
            double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = 6;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.scale = 1.2f;
            if (Main.expertMode)
            {
                npc.scale = 1.35f;
            }
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.dontCountMe = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spawn);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spawn = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
            npc.localAI[3] = reader.ReadSingle();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
			CalamityAI.AstrumDeusAI(npc, mod, false, false);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !npc.dontTakeDamage;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Color lightColor = new Color(125, 75, Main.DiscoB, npc.alpha); //250 150 Disco
            Color newColor = npc.dontTakeDamage ? lightColor : drawColor;
            Texture2D texture = ModContent.GetTexture("CalamityMod/NPCs/AstrumDeus/AstrumDeusBodyAlt");
            CalamityMod.DrawTexture(spriteBatch, npc.localAI[3] == 1f ? texture : Main.npcTexture[npc.type], 0, npc, newColor);
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.CountNPCS(ModContent.NPCType<AstrumDeusProbe3>()) < 3 && CalamityWorld.revenge)
            {
                if (npc.life > 0 && Main.netMode != NetmodeID.MultiplayerClient && spawn == 0 && Main.rand.NextBool(25))
                {
                    spawn = 1;
                    int num660 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), ModContent.NPCType<AstrumDeusProbe3>(), 0, 0f, 0f, 0f, 0f, 255);
                    if (Main.netMode == NetmodeID.Server && num660 < 200)
                    {
                        NetMessage.SendData(23, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                    }
                    npc.netUpdate = true;
                }
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
    }
}
