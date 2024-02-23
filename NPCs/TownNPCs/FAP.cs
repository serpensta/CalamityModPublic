using CalamityMod.Events;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class FAP : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 27;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[NPC.type] = 15;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPC.Happiness
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Love)
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Stylist, AffectionLevel.Love)
                .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
                .SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Like)
                .SetNPCAffection(NPCID.DD2Bartender, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Hate);
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.lavaImmune = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0.5f;
            //AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,                
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.FAP")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            int num236 = (NPC.isLikeATownNPC ? NPCID.Sets.ExtraFramesCount[NPC.type] : 0);
            /*if (false && !Main.dedServ && TownNPCProfiles.Instance.GetProfile(this, out var profile))
            {
                Asset<Texture2D> textureNPCShouldUse = profile.GetTextureNPCShouldUse(this);
                if (textureNPCShouldUse.IsLoaded)
                {
                    num = textureNPCShouldUse.Height() / Main.npcFrameCount[type];
                    frame.Width = textureNPCShouldUse.Width();
                    frame.Height = num;
                }
            }*/

            if (NPC.velocity.Y == 0f)
            {
                if (NPC.direction == 1)
                    NPC.spriteDirection = 1;

                if (NPC.direction == -1)
                    NPC.spriteDirection = -1;

                int num237 = Main.npcFrameCount[NPC.type] - NPCID.Sets.AttackFrameCount[NPC.type];
                if (NPC.ai[0] == 23f)
                {
                    NPC.frameCounter += 1D;
                    int num238 = NPC.frame.Y / frameHeight;
                    int num17 = num237 - num238;
                    if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num238 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num239 = 0;
                    num239 = ((!(NPC.frameCounter < 6D)) ? (num237 - 4) : (num237 - 5));
                    if (NPC.ai[1] < 6f)
                        num239 = num237 - 5;

                    NPC.frame.Y = frameHeight * num239;
                }
                else if (NPC.ai[0] >= 20f && NPC.ai[0] <= 22f)
                {
                    int num240 = NPC.frame.Y / frameHeight;
                    switch ((int)NPC.ai[0])
                    {
                        case 20:
                        case 21:
                        case 22:
                            break;
                    }

                    NPC.frame.Y = num240 * frameHeight;
                }
                else if (NPC.ai[0] == 2f)
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frame.Y / frameHeight == num237 - 1 && NPC.frameCounter >= 5D)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }
                    else if (NPC.frame.Y / frameHeight == 0 && NPC.frameCounter >= 40D)
                    {
                        NPC.frame.Y = frameHeight * (num237 - 1);
                        NPC.frameCounter = 0D;
                    }
                    else if (NPC.frame.Y != 0 && NPC.frame.Y != frameHeight * (num237 - 1))
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }
                }
                else if (NPC.ai[0] == 11f)
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frame.Y / frameHeight == num237 - 1 && NPC.frameCounter >= 50D)
                    {
                        if (NPC.frameCounter == 50D)
                        {
                            int num242 = Main.rand.Next(4);
                            for (int m = 0; m < 3 + num242; m++)
                            {
                                int num243 = Dust.NewDust(NPC.Center + Vector2.UnitX * -NPC.direction * 8f - Vector2.One * 5f + Vector2.UnitY * 8f, 3, 6, 216, -NPC.direction, 1f);
                                Dust dust = Main.dust[num243];
                                dust.velocity /= 2f;
                                Main.dust[num243].scale = 0.8f;
                            }

                            if (Main.rand.NextBool(30))
                            {
                                int num244 = Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * -NPC.direction * 8f, Vector2.Zero, Main.rand.Next(580, 583));
                                Gore gore = Main.gore[num244];
                                gore.velocity /= 2f;
                                Main.gore[num244].velocity.Y = Math.Abs(Main.gore[num244].velocity.Y);
                                Main.gore[num244].velocity.X = (0f - Math.Abs(Main.gore[num244].velocity.X)) * (float)NPC.direction;
                            }
                        }

                        if (NPC.frameCounter >= 100D && Main.rand.NextBool(20))
                        {
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0D;
                        }
                    }
                    else if (NPC.frame.Y / frameHeight == 0 && NPC.frameCounter >= 20D)
                    {
                        NPC.frame.Y = frameHeight * (num237 - 1);
                        NPC.frameCounter = 0D;
                        EmoteBubble.NewBubble(EmoteID.EmoteSleep, new WorldUIAnchor(NPC), 90);
                    }
                    else if (NPC.frame.Y != 0 && NPC.frame.Y != frameHeight * (num237 - 1))
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }
                }
                else if (NPC.ai[0] == 5f)
                {
                    NPC.frame.Y = frameHeight * (num237 - 3);
                    NPC.frameCounter = 0D;
                }
                else if (NPC.ai[0] == 6f)
                {
                    NPC.frameCounter += 1D;
                    int num245 = NPC.frame.Y / frameHeight;
                    int num17 = num237 - num245;
                    if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num245 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num246 = 0;
                    num246 = ((!(NPC.frameCounter < 10D)) ?
                        ((NPC.frameCounter < 16D) ?
                        (num237 - 5) : ((NPC.frameCounter < 46D) ?
                        (num237 - 4) : ((NPC.frameCounter < 60D) ?
                        (num237 - 5) : ((!(NPC.frameCounter < 66D)) ?
                        ((NPC.frameCounter < 72D) ?
                        (num237 - 5) : ((NPC.frameCounter < 102D) ?
                        (num237 - 4) : ((NPC.frameCounter < 108D) ?
                        (num237 - 5) : ((!(NPC.frameCounter < 114D)) ?
                        ((NPC.frameCounter < 120D) ?
                        (num237 - 5) : ((NPC.frameCounter < 150D) ?
                        (num237 - 4) : ((NPC.frameCounter < 156D) ?
                        (num237 - 5) : ((!(NPC.frameCounter < 162D)) ?
                        ((NPC.frameCounter < 168D) ?
                        (num237 - 5) : ((NPC.frameCounter < 198D) ?
                        (num237 - 4) : ((NPC.frameCounter < 204D) ?
                        (num237 - 5) : ((!(NPC.frameCounter < 210D)) ?
                        ((NPC.frameCounter < 216D) ?
                        (num237 - 5) : ((NPC.frameCounter < 246D) ?
                        (num237 - 4) : ((NPC.frameCounter < 252D) ?
                        (num237 - 5) : ((!(NPC.frameCounter < 258D)) ?
                        ((NPC.frameCounter < 264D) ?
                        (num237 - 5) : ((NPC.frameCounter < 294D) ?
                        (num237 - 4) : ((NPC.frameCounter < 300D) ?
                        (num237 - 5) : 0))) : 0)))) : 0)))) : 0)))) : 0)))) : 0)))) : 0);

                    if (num246 == num237 - 4 && num245 == num237 - 5)
                    {
                        Vector2 vector4 = NPC.Center + new Vector2(10 * NPC.direction, -4f);
                        for (int n = 0; n < 8; n++)
                        {
                            int num247 = Main.rand.Next(139, 143);
                            int num248 = Dust.NewDust(vector4, 0, 0, num247, NPC.velocity.X + (float)NPC.direction, NPC.velocity.Y - 2.5f, 0, default(Color), 1.2f);
                            Main.dust[num248].velocity.X += (float)NPC.direction * 1.5f;
                            Dust dust = Main.dust[num248];
                            dust.position -= new Vector2(4f);
                            dust = Main.dust[num248];
                            dust.velocity *= 2f;
                            Main.dust[num248].scale = 0.7f + Main.rand.NextFloat() * 0.3f;
                        }
                    }

                    NPC.frame.Y = frameHeight * num246;
                    if (NPC.frameCounter >= 300D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.ai[0] == 7f || NPC.ai[0] == 19f)
                {
                    NPC.frameCounter += 1D;
                    int num249 = NPC.frame.Y / frameHeight;
                    int num17 = num237 - num249;
                    if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num249 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num250 = 0;
                    if (NPC.frameCounter < 16D)
                        num250 = 0;
                    else if (NPC.frameCounter == 16D)
                        EmoteBubble.NewBubbleNPC(new WorldUIAnchor(NPC), 112);
                    else if (NPC.frameCounter < 128D)
                        num250 = ((NPC.frameCounter % 16D < 8D) ? (num237 - 2) : 0);
                    else if (NPC.frameCounter < 160D)
                        num250 = 0;
                    else if (NPC.frameCounter != 160D)
                        num250 = ((NPC.frameCounter < 220D) ? ((NPC.frameCounter % 12D < 6D) ? (num237 - 2) : 0) : 0);
                    else
                        EmoteBubble.NewBubbleNPC(new WorldUIAnchor(NPC), 60);

                    NPC.frame.Y = frameHeight * num250;
                    if (NPC.frameCounter >= 220D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.ai[0] == 9f)
                {
                    NPC.frameCounter += 1D;
                    int num251 = NPC.frame.Y / frameHeight;
                    int num17 = num237 - num251;
                    if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num251 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num252 = 0;
                    num252 = ((!(NPC.frameCounter < 10D)) ? ((!(NPC.frameCounter < 16D)) ? (num237 - 4) : (num237 - 5)) : 0);
                    if (NPC.ai[1] < 16f)
                        num252 = num237 - 5;

                    if (NPC.ai[1] < 10f)
                        num252 = 0;

                    NPC.frame.Y = frameHeight * num252;
                }
                else if (NPC.ai[0] == 18f)
                {
                    NPC.frameCounter += 1D;
                    int num253 = NPC.frame.Y / frameHeight;
                    int num17 = num237 - num253;
                    if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num253 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num254 = 0;
                    if (NPC.frameCounter < 10D)
                        num254 = 0;
                    else if (NPC.frameCounter < 16D)
                        num254 = num237 - 1;
                    else
                        num254 = num237 - 2;

                    if (NPC.ai[1] < 16f)
                        num254 = num237 - 1;

                    if (NPC.ai[1] < 10f)
                        num254 = 0;

                    num254 = Main.npcFrameCount[NPC.type] - 2;
                    NPC.frame.Y = frameHeight * num254;
                }
                else if (NPC.ai[0] == 10f || NPC.ai[0] == 13f)
                {
                    NPC.frameCounter += 1D;
                    int num255 = NPC.frame.Y / frameHeight;
                    int num17 = num255 - num237;
                    if ((uint)num17 > 3u && num255 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num256 = 10;
                    int num257 = 6;
                    int num258 = 0;
                    num258 = ((!(NPC.frameCounter < (double)num256)) ?
                        ((NPC.frameCounter < (double)(num256 + num257)) ?
                        num237 : ((NPC.frameCounter < (double)(num256 + num257 * 2)) ?
                        (num237 + 1) : ((NPC.frameCounter < (double)(num256 + num257 * 3)) ?
                        (num237 + 2) : ((NPC.frameCounter < (double)(num256 + num257 * 4)) ?
                        (num237 + 3) : 0)))) : 0);

                    NPC.frame.Y = frameHeight * num258;
                }
                else if (NPC.ai[0] == 15f)
                {
                    NPC.frameCounter += 1D;
                    int num259 = NPC.frame.Y / frameHeight;
                    int num17 = num259 - num237;
                    if ((uint)num17 > 3u && num259 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    float num260 = NPC.ai[1] / (float)NPCID.Sets.AttackTime[NPC.type];
                    int num261 = 0;
                    num261 = ((num260 > 0.65f) ?
                        num237 : ((num260 > 0.5f) ?
                        (num237 + 1) : ((num260 > 0.35f) ?
                        (num237 + 2) : ((num260 > 0f) ?
                        (num237 + 3) : 0))));

                    NPC.frame.Y = frameHeight * num261;
                }
                else if (NPC.ai[0] == 25f)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.ai[0] == 12f)
                {
                    NPC.frameCounter += 1D;
                    int num262 = NPC.frame.Y / frameHeight;
                    int num17 = num262 - num237;
                    if ((uint)num17 > 4u && num262 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num263 = num237 + NPC.GetShootingFrame(NPC.ai[2]);
                    NPC.frame.Y = frameHeight * num263;
                }
                else if (NPC.ai[0] == 14f || NPC.ai[0] == 24f)
                {
                    NPC.frameCounter += 1D;
                    int num264 = NPC.frame.Y / frameHeight;
                    int num17 = num264 - num237;
                    if ((uint)num17 > 1u && num264 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num265 = 12;
                    int num266 = ((NPC.frameCounter % (double)num265 * 2D < (double)num265) ? num237 : (num237 + 1));
                    NPC.frame.Y = frameHeight * num266;
                    if (NPC.ai[0] == 24f)
                    {
                        if (NPC.frameCounter == 60D)
                            EmoteBubble.NewBubble(EmoteID.EmoteConfused, new WorldUIAnchor(NPC), 60);

                        if (NPC.frameCounter == 150D)
                            EmoteBubble.NewBubble(EmoteID.EmotionAlert, new WorldUIAnchor(NPC), 90);

                        if (NPC.frameCounter >= 240D)
                            NPC.frame.Y = 0;
                    }
                }
                else if (NPC.ai[0] == 1001f)
                {
                    NPC.frame.Y = frameHeight * (num237 - 1);
                    NPC.frameCounter = 0D;
                }
                else if (NPC.CanTalk && (NPC.ai[0] == 3f || NPC.ai[0] == 4f))
                {
                    NPC.frameCounter += 1D;
                    int num267 = NPC.frame.Y / frameHeight;
                    int num17 = num237 - num267;
                    if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num267 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    bool displayEmote = NPC.ai[0] == 3f;
                    int num268 = 0;
                    int num269 = 0;
                    int emoteDisplayTime = -1;
                    int emoteDisplayTime2 = -1;
                    if (NPC.frameCounter < 10D)
                        num268 = 0;
                    else if (NPC.frameCounter < 16D)
                        num268 = num237 - 5;
                    else if (NPC.frameCounter < 46D)
                        num268 = num237 - 4;
                    else if (NPC.frameCounter < 60D)
                        num268 = num237 - 5;
                    else if (NPC.frameCounter < 216D)
                        num268 = 0;
                    else if (NPC.frameCounter == 216D && Main.netMode != NetmodeID.MultiplayerClient)
                        emoteDisplayTime = 70;
                    else if (NPC.frameCounter < 286D)
                        num268 = ((NPC.frameCounter % 12D < 6D) ? (num237 - 2) : 0);
                    else if (NPC.frameCounter < 320D)
                        num268 = 0;
                    else if (NPC.frameCounter != 320D || Main.netMode == NetmodeID.MultiplayerClient)
                        num268 = ((NPC.frameCounter < 420D) ? ((NPC.frameCounter % 16D < 8D) ? (num237 - 2) : 0) : 0);
                    else
                        emoteDisplayTime = 100;

                    if (NPC.frameCounter < 70D)
                    {
                        num269 = 0;
                    }
                    else if (NPC.frameCounter != 70D || Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        num269 = ((NPC.frameCounter < 160D) ?
                            ((NPC.frameCounter % 16D < 8D) ?
                            (num237 - 2) : 0) : ((NPC.frameCounter < 166D) ?
                            (num237 - 5) : ((NPC.frameCounter < 186D) ?
                            (num237 - 4) : ((NPC.frameCounter < 200D) ?
                            (num237 - 5) : ((!(NPC.frameCounter < 320D)) ?
                            ((NPC.frameCounter < 326D) ?
                            (num237 - 1) : 0) : 0)))));
                    }
                    else
                        emoteDisplayTime2 = 90;

                    if (displayEmote)
                    {
                        NPC nPC = Main.npc[(int)NPC.ai[2]];
                        if (emoteDisplayTime != -1)
                            EmoteBubble.NewBubbleNPC(new WorldUIAnchor(NPC), emoteDisplayTime, new WorldUIAnchor(nPC));

                        if (emoteDisplayTime2 != -1 && nPC.CanTalk)
                            EmoteBubble.NewBubbleNPC(new WorldUIAnchor(nPC), emoteDisplayTime2, new WorldUIAnchor(NPC));
                    }

                    NPC.frame.Y = frameHeight * (displayEmote ? num268 : num269);
                    if (NPC.frameCounter >= 420D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.CanTalk && (NPC.ai[0] == 16f || NPC.ai[0] == 17f))
                {
                    NPC.frameCounter += 1D;
                    int num272 = NPC.frame.Y / frameHeight;
                    int num17 = num237 - num272;
                    if ((uint)(num17 - 1) > 1u && (uint)(num17 - 4) > 1u && num272 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    bool flag13 = NPC.ai[0] == 16f;
                    int num273 = 0;
                    int emoteDisplayTime = -1;
                    if (NPC.frameCounter < 10D)
                        num273 = 0;
                    else if (NPC.frameCounter < 16D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter < 22D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 28D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter < 34D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 40D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter == 40D && Main.netMode != NetmodeID.MultiplayerClient)
                        emoteDisplayTime = 45;
                    else if (NPC.frameCounter < 70D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 76D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter < 82D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 88D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter < 94D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 100D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter == 100D && Main.netMode != NetmodeID.MultiplayerClient)
                        emoteDisplayTime = 45;
                    else if (NPC.frameCounter < 130D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 136D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter < 142D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 148D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter < 154D)
                        num273 = num237 - 4;
                    else if (NPC.frameCounter < 160D)
                        num273 = num237 - 5;
                    else if (NPC.frameCounter != 160D || Main.netMode == NetmodeID.MultiplayerClient)
                        num273 = ((NPC.frameCounter < 220D) ? (num237 - 4) : ((NPC.frameCounter < 226D) ? (num237 - 5) : 0));
                    else
                        emoteDisplayTime = 75;

                    if (flag13 && emoteDisplayTime != -1)
                    {
                        int num275 = (int)NPC.localAI[2];
                        int num276 = (int)NPC.localAI[3];
                        int num277 = (int)Main.npc[(int)NPC.ai[2]].localAI[3];
                        int num278 = (int)Main.npc[(int)NPC.ai[2]].localAI[2];
                        int num279 = 3 - num275 - num276;
                        int num280 = 0;
                        if (NPC.frameCounter == 40D)
                            num280 = 1;

                        if (NPC.frameCounter == 100D)
                            num280 = 2;

                        if (NPC.frameCounter == 160D)
                            num280 = 3;

                        int num281 = 3 - num280;
                        int rockPaperScissorsResultType = -1;
                        int num283 = 0;
                        while (rockPaperScissorsResultType < 0)
                        {
                            num17 = num283 + 1;
                            num283 = num17;
                            if (num17 >= 100)
                                break;

                            rockPaperScissorsResultType = Main.rand.Next(2);
                            if (rockPaperScissorsResultType == 0 && num278 >= num276)
                                rockPaperScissorsResultType = -1;

                            if (rockPaperScissorsResultType == 1 && num277 >= num275)
                                rockPaperScissorsResultType = -1;

                            if (rockPaperScissorsResultType == -1 && num281 <= num279)
                                rockPaperScissorsResultType = 2;
                        }

                        if (rockPaperScissorsResultType == 0)
                        {
                            Main.npc[(int)NPC.ai[2]].localAI[3] += 1f;
                            num277++;
                        }

                        if (rockPaperScissorsResultType == 1)
                        {
                            Main.npc[(int)NPC.ai[2]].localAI[2] += 1f;
                            num278++;
                        }

                        int emoteType = Utils.SelectRandom<int>(Main.rand, EmoteID.RPSPaper, EmoteID.RPSRock, EmoteID.RPSScissors);
                        int emoteType2 = emoteType;
                        switch (rockPaperScissorsResultType)
                        {
                            case 0:
                                switch (emoteType)
                                {
                                    case EmoteID.RPSPaper:
                                        emoteType2 = EmoteID.RPSRock;
                                        break;
                                    case EmoteID.RPSRock:
                                        emoteType2 = EmoteID.RPSScissors;
                                        break;
                                    case EmoteID.RPSScissors:
                                        emoteType2 = EmoteID.RPSPaper;
                                        break;
                                }
                                break;
                            case 1:
                                switch (emoteType)
                                {
                                    case EmoteID.RPSPaper:
                                        emoteType2 = EmoteID.RPSScissors;
                                        break;
                                    case EmoteID.RPSRock:
                                        emoteType2 = EmoteID.RPSPaper;
                                        break;
                                    case EmoteID.RPSScissors:
                                        emoteType2 = EmoteID.RPSRock;
                                        break;
                                }
                                break;
                        }

                        if (num281 == 0)
                        {
                            if (num277 >= 2)
                                emoteType -= 3;

                            if (num278 >= 2)
                                emoteType2 -= 3;
                        }

                        EmoteBubble.NewBubble(emoteType, new WorldUIAnchor(NPC), emoteDisplayTime);
                        EmoteBubble.NewBubble(emoteType2, new WorldUIAnchor(Main.npc[(int)NPC.ai[2]]), emoteDisplayTime);
                    }

                    NPC.frame.Y = frameHeight * (flag13 ? num273 : num273);
                    if (NPC.frameCounter >= 420D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.velocity.X == 0f)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0D;
                }
                else
                {
                    NPC.frameCounter += Math.Abs(NPC.velocity.X) * 2f;
                    NPC.frameCounter += 1D;

                    int num288 = frameHeight * 2;
                    if (NPC.frame.Y < num288)
                        NPC.frame.Y = num288;

                    int num287 = 6;
                    if (NPC.frameCounter > (double)num287)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }

                    if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type] - num236)
                        NPC.frame.Y = num288;
                }

                return;
            }

            NPC.frameCounter = 0D;
            NPC.frame.Y = frameHeight;
        }

        public override void AI()
        {
            if (!CalamityWorld.spawnedCirrus)
                CalamityWorld.spawnedCirrus = true;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas.SupremeCalamitas>()) && Main.zenithWorld)
                return false;

            if (CalamityWorld.spawnedCirrus)
                return true;

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                bool hasVodka = player.InventoryHas(ModContent.ItemType<FabsolsVodka>()) || player.PortableStorageHas(ModContent.ItemType<FabsolsVodka>());
                if (player.active && hasVodka)
                    return Main.hardMode;
            }
            return false;
        }

		public override List<string> SetNPCNameList() => new List<string>() { this.GetLocalizedValue("Name.Cirrus") };

        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            if (Main.zenithWorld)
            {
                player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CirrusSlap" + Main.rand.Next(1, 2 + 1)).Format(player.name)), player.statLife / 2, -player.direction, false, false, -1, false);
                SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, player.Center);
            }

            if (CalamityUtils.AnyBossNPCS())
                return this.GetLocalizedValue("Chat.BossAlive");

            if (NPC.homeless)
                return this.GetLocalizedValue("Chat.Homeless" + Main.rand.Next(1, 2 + 1));

            int wife = NPC.FindFirstNPC(NPCID.Stylist);
            bool wifeIsAround = wife != -1;
            bool beLessDrunk = wifeIsAround && NPC.downedMoonlord;

            if (Main.bloodMoon)
            {
                if (Main.rand.NextBool(4))
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CirrusSlap" + Main.rand.Next(1, 2 + 1)).Format(player.name)), player.statLife / 2, -player.direction, false, false, -1, false); ;
                    SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, player.Center);
                    return this.GetLocalizedValue("Chat.BloodMoonSlap");
                }
                return this.GetLocalizedValue("Chat.BloodMoon" + Main.rand.Next(1, 3 + 1));
            }

            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));
            if (ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.Normal4"));

            int tavernKeep = NPC.FindFirstNPC(NPCID.DD2Bartender);
            if (tavernKeep != -1)
            {
                dialogue.Add(this.GetLocalization("Chat.Tavernkeep1").Format(Main.npc[tavernKeep].GivenName));
                dialogue.Add(this.GetLocalization("Chat.Tavernkeep2").Format(Main.npc[tavernKeep].GivenName));

                if (ChildSafety.Disabled)
                    dialogue.Add(this.GetLocalizedValue("Chat.Tavernkeep3"));
            }

            int permadong = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permadong != -1)
                dialogue.Add(this.GetLocalization("Chat.Archmage").Format(Main.npc[permadong].GivenName));

            int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if (witch != -1)
                dialogue.Add(this.GetLocalization("Chat.BrimstoneWitch").Format(Main.npc[witch].GivenName));

            if (wifeIsAround)
            {
                dialogue.Add(this.GetLocalization("Chat.Stylist1").Format(Main.npc[wife].GivenName));
                if (ChildSafety.Disabled)
                {
                    dialogue.Add(this.GetLocalization("Chat.Stylist2").Format(Main.npc[wife].GivenName));
                    dialogue.Add(this.GetLocalization("Chat.Stylist3").Format(Main.npc[wife].GivenName));
                }
            }

            if (Main.dayTime)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Day1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day3"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day4"));

                if (beLessDrunk)
                {
                    dialogue.Add(this.GetLocalization("Chat.DayStylist1").Format(Main.npc[wife].GivenName));
                    dialogue.Add(this.GetLocalization("Chat.DayStylist2").Format(Main.npc[wife].GivenName));
                }
                else
                {
                    dialogue.Add(this.GetLocalizedValue("Chat.DayDrunk1"));
                    dialogue.Add(this.GetLocalizedValue("Chat.DayDrunk2"));
                }
            }
            else
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Night1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night3"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night4"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night5"));

                if (wifeIsAround)
                    dialogue.Add(this.GetLocalization("Chat.NightStylist").Format(Main.npc[wife].GivenName));
            }

            if (BirthdayParty.PartyIsUp)
                dialogue.Add(this.GetLocalizedValue("Chat.Party"));

            if (AcidRainEvent.AcidRainEventIsOngoing)
                dialogue.Add(this.GetLocalizedValue("Chat.AcidRain"));

            if (Main.invasionType == InvasionID.MartianMadness)
                dialogue.Add(this.GetLocalizedValue("Chat.Martians"));

            if (DownedBossSystem.downedCryogen && ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.CryogenDefeated"));

            if (DownedBossSystem.downedLeviathan)
                dialogue.Add(this.GetLocalizedValue("Chat.LeviathanDefeated"));

            if (NPC.downedMoonlord)
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated"));

            if (DownedBossSystem.downedPolterghast)
                dialogue.Add(this.GetLocalizedValue("Chat.PolterghastDefeated"));

            if (DownedBossSystem.downedDoG)
                dialogue.Add(this.GetLocalizedValue("Chat.DoGDefeated"));

            if (player.Calamity().chibii)
                dialogue.Add(this.GetLocalizedValue("Chat.HasChibii"));

            if (player.Calamity().aquaticHeart && !player.Calamity().aquaticHeartHide && ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.HasAnahitaTrans"));

            if (player.Calamity().fabsolVodka)
                dialogue.Add(this.GetLocalizedValue("Chat.HasVodka"));

            if (player.HasItem(ModContent.ItemType<Fabsol>()))
            {
                dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn1"));
                dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn2"));
                if (ChildSafety.Disabled)
                    dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn3"));
            }

            return dialogue;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var something = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + (BirthdayParty.PartyIsUp ? "Alt" : "")).Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY) - new Vector2(0f, 6f), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, something, 0);
            return false;
        }

        public string Death()
        {
            int deaths = Main.player[Main.myPlayer].numberOfDeathsPVE;

            string text = this.GetLocalization("DeathCount").Format(deaths);

            if (deaths > 10000)
                text += " " + this.GetLocalizedValue("Death10000");
            else if (deaths > 5000)
                text += " " + this.GetLocalizedValue("Death5000");
            else if (deaths > 2500)
                text += " " + this.GetLocalizedValue("Death2500");
            else if (deaths > 1000)
                text += " " + this.GetLocalizedValue("Death1000");
            else if (deaths > 500)
                text += " " + this.GetLocalizedValue("Death500");
            else if (deaths > 250)
                text += " " + this.GetLocalizedValue("Death250");
            else if (deaths > 100)
                text += " " + this.GetLocalizedValue("Death100");

            IList<string> donorList = new List<string>(CalamityLists.donatorList);
            int maxDonorsListed = 25;
            string[] donors = new string[maxDonorsListed];
            for (int i = 0; i < maxDonorsListed; i++)
            {
                donors[i] = donorList[Main.rand.Next(donorList.Count)];
                donorList.Remove(donors[i]);
            }

            text += ("\n\n" + this.GetLocalization("DonorShoutout").Format(donors));

            return text;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = this.GetLocalizedValue("DeathCountButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
            else
            {
                Main.npcChatText = Death();
            }
        }

        public override void AddShops()
        {
            Condition potionSells = new(CalamityUtils.GetText("Condition.PotionConfig"), () => CalamityConfig.Instance.PotionSelling);
            Condition downedAureus = new(CalamityUtils.GetText("Condition.PostAureus"), () => DownedBossSystem.downedAstrumAureus);

            NPCShop shop = new(Type);
            shop.AddWithCustomValue(ItemID.LovePotion, Item.buyPrice(silver: 25), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ModContent.ItemType<GrapeBeer>(), Item.buyPrice(silver: 30))
                .AddWithCustomValue(ModContent.ItemType<RedWine>(), Item.buyPrice(gold: 1))
                .AddWithCustomValue(ModContent.ItemType<Whiskey>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Rum>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Tequila>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Fireball>(), Item.buyPrice(gold: 3))
                .AddWithCustomValue(ModContent.ItemType<FabsolsVodka>(), Item.buyPrice(gold: 4))
                .AddWithCustomValue(ModContent.ItemType<Vodka>(), Item.buyPrice(gold: 2), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<Screwdriver>(), Item.buyPrice(gold: 6), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<WhiteWine>(), Item.buyPrice(gold: 6), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<EvergreenGin>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<CaribbeanRum>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<Margarita>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<OldFashioned>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ItemID.EmpressButterfly, Item.buyPrice(gold: 10), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<Everclear>(), Item.buyPrice(gold: 3), downedAureus)
                .AddWithCustomValue(ModContent.ItemType<BloodyMary>(), Item.buyPrice(gold: 4), downedAureus, Condition.BloodMoon)
                .AddWithCustomValue(ModContent.ItemType<StarBeamRye>(), Item.buyPrice(gold: 6), downedAureus, Condition.TimeNight)
                .AddWithCustomValue(ModContent.ItemType<Moonshine>(), Item.buyPrice(gold: 2), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<MoscowMule>(), Item.buyPrice(gold: 8), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<CinnamonRoll>(), Item.buyPrice(gold: 8), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<TequilaSunrise>(), Item.buyPrice(gold: 10), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.BloodyMoscato, Item.buyPrice(gold: 1), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.BananaDaiquiri, Item.buyPrice(silver: 75), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.PeachSangria, Item.buyPrice(silver: 50), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.PinaColada, Item.buyPrice(gold: 1), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ModContent.ItemType<WeightlessCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<VigorousCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<ResilientCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<SpitefulCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<OddMushroom>(), Item.buyPrice(1))
                .AddWithCustomValue(ItemID.UnicornHorn, Item.buyPrice(0, 2, 50), Condition.HappyEnough, Condition.InHallow)
                .AddWithCustomValue(ItemID.Milkshake, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.InHallow, Condition.NpcIsPresent(NPCID.Stylist))
                .Register();
        }

        // Make this Town NPC teleport to the Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 180;
            randExtraCooldown = 60;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<FabRay>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 11.5f;
        }
    }
}
