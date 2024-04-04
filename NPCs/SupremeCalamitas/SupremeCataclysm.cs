using System;
using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [AutoloadBossHead]
    public class SupremeCataclysm : ModNPC
    {
        public int VerticalOffset = 375;

        public int CurrentFrame;

        public bool PunchingFromRight;

        public const int HorizontalOffset = 750;

        public const int PunchCounterLimit = 50;

        public const int DartBurstCounterLimit = 300;

        public const int PreBigAttackPause = 50;
        public const float NormalBrothersDR = 0.25f;
        public int BigAttackLimit = 4;
        public bool targetSide = false;

        public Player Target => Main.player[NPC.target];
        public ref float PunchCounter => ref NPC.ai[1];
        public ref float DartBurstCounter => ref NPC.ai[2];
        public ref float ElapsedVerticalDistance => ref NPC.ai[3];
        public ref float AttackDelayTimer => ref NPC.localAI[0];
        public ref float BigAttackTimer => ref NPC.localAI[1];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.3f,
                PortraitPositionYOverride = 36f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.damage = 50;
            NPC.npcSlots = 5f;
            NPC.width = 120;
            NPC.height = 120;
            NPC.defense = 80;
            NPC.DR_NERD(NormalBrothersDR);
            NPC.lifeMax = 138000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SupremeCalamitas.BrotherHit;
            NPC.DeathSound = SupremeCalamitas.BrotherDeath;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.localAI[1] = 300;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<SupremeCalamitas>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SupremeCataclysm")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(VerticalOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            VerticalOffset = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
            float punchInterpolant = Utils.GetLerpValue(10f, PunchCounterLimit * 2f, PunchCounter + (PunchingFromRight ? 0f : PunchCounterLimit), true);
            if (AttackDelayTimer < 120f)
            {
                NPC.frameCounter += 0.15f;
                if (NPC.frameCounter >= 1f)
                {
                    CurrentFrame = (CurrentFrame + 1) % 12;
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                CurrentFrame = (int)Math.Round(MathHelper.Lerp(12f, 21f, punchInterpolant));
            }

            int xFrame = CurrentFrame / Main.npcFrameCount[NPC.type];
            int yFrame = CurrentFrame % Main.npcFrameCount[NPC.type];

            NPC.frame.Width = 212;
            NPC.frame.Height = 208;
            NPC.frame.X = xFrame * NPC.frame.Width;
            NPC.frame.Y = yFrame * NPC.frame.Height;
        }

        public override void AI()
        {
            bool isBroDead = NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>()) == false ? true : false;

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            NPC.direction = NPC.spriteDirection;

            if (BigAttackTimer > 0)
                BigAttackTimer--;

            // Set the whoAmI variable.
            CalamityGlobalNPC.SCalCataclysm = NPC.whoAmI;

            // Disappear if Supreme Calamitas is not present.
            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Increase DR if the target leaves SCal's arena.
            NPC.Calamity().DR = SupremeCataclysm.NormalBrothersDR;
            if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().IsTargetOutsideOfArena)
                NPC.Calamity().DR = SupremeCalamitas.enragedDR;

            float totalLifeRatio = NPC.life / (float)NPC.lifeMax;
            if (CalamityGlobalNPC.SCalCatastrophe != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCalCatastrophe].active)
                    totalLifeRatio += Main.npc[CalamityGlobalNPC.SCalCatastrophe].life / (float)Main.npc[CalamityGlobalNPC.SCalCatastrophe].lifeMax;
            }
            totalLifeRatio *= 0.5f;

            // Get a target if no valid one has been found.
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Target.dead || !Target.active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away.
            if (!NPC.WithinRange(Target.Center, CalamityGlobalNPC.CatchUpDistance200Tiles))
                NPC.TargetClosest();

            float acceleration = Utils.Remap(AttackDelayTimer, 0f, 120f, 2f, 4f);
            int verticalSpeed = (int)Math.Round(MathHelper.Lerp(2f, 6.5f, 1f - totalLifeRatio));

            // Buffs for the big attack when the other brother dies
            if (isBroDead && BigAttackTimer > 400)
                BigAttackTimer = 400;

            if (isBroDead && BigAttackTimer > 0 && BigAttackLimit < 19)
                BigAttackLimit = 19;

            if (BigAttackTimer > PreBigAttackPause)
            {
                // Move up.
                if (ElapsedVerticalDistance < HorizontalOffset)
                {
                    ElapsedVerticalDistance += verticalSpeed;
                    VerticalOffset -= verticalSpeed;
                }

                // Move down.
                else if (ElapsedVerticalDistance < HorizontalOffset * 2)
                {
                    ElapsedVerticalDistance += verticalSpeed;
                    VerticalOffset += verticalSpeed;
                }

                // Reset the vertical distance once a single period has concluded.
                else
                    ElapsedVerticalDistance = 0f;

                // Hover to the side of the target.
                Vector2 idealVelocity = NPC.SafeDirectionTo(Target.Center + new Vector2(HorizontalOffset * (targetSide ? -1 : 1), VerticalOffset)) * Utils.Remap(AttackDelayTimer, 60f, 120f, 0f, 50f);
                if (PunchCounter <= PunchCounterLimit * 0.3f)
                {
                    if (AttackDelayTimer == 120 && Main.rand.NextBool())
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Dust cataclysmdust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height) - NPC.velocity * 0.5f, 66, -NPC.velocity * Main.rand.NextFloat(0.2f, 1.2f));
                            cataclysmdust.noGravity = true;
                            cataclysmdust.scale = Main.rand.NextFloat(0.5f, 0.7f);
                            cataclysmdust.color = Color.Red;
                        }
                    }
                    NPC.SimpleFlyMovement(idealVelocity, acceleration);
                }
                else
                    NPC.velocity *= 0.85f;
            }
            else
                NPC.velocity *= 0.9f;

            // Reset rotation to zero.
            NPC.rotation = 0f;

            // Set direction.
            NPC.spriteDirection = (Target.Center.X > NPC.Center.X).ToDirectionInt();

            // Have a small delay prior to shooting projectiles.
            if (AttackDelayTimer < 120f)
                AttackDelayTimer++;

            // Handle projectile shots.
            else if (BigAttackTimer > PreBigAttackPause)
            {
                // Shoot fists.
                float fireVelocity = isBroDead ? -8.8f : -8;
                float fireRate = BossRushEvent.BossRushActive ? 2f : MathHelper.Lerp(1f, 2.5f, 1f - totalLifeRatio) * (isBroDead ? 1.2f : 1);
                
                PunchCounter += fireRate;
                if (PunchCounter >= PunchCounterLimit)
                {
                    PunchCounter = 0f;
                    SoundEngine.PlaySound(SupremeCalamitas.HellblastSound, NPC.Center);
                    int type = ModContent.ProjectileType<SupremeCataclysmFist>();
                    int damage = NPC.GetProjectileDamage(type);
                    if (bossRush)
                        damage /= 2;
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 fistSpawnPosition = NPC.Center + Vector2.UnitX * 74f * NPC.direction;
                        if (isBroDead)
                        {
                            if (PunchingFromRight)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fistSpawnPosition, NPC.DirectionTo(Target.Center) * 16f, type, damage, 0f, Main.myPlayer, 0f, PunchingFromRight.ToInt(), 2);
                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fistSpawnPosition, NPC.DirectionTo(Target.Center).RotatedBy(0.55) * 16f, type, damage, 0f, Main.myPlayer, 0f, PunchingFromRight.ToInt(), 2);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fistSpawnPosition, NPC.DirectionTo(Target.Center).RotatedBy(-0.55) * 16f, type, damage, 0f, Main.myPlayer, 0f, PunchingFromRight.ToInt(), 2);
                            }
                        }
                        else
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), fistSpawnPosition, NPC.DirectionTo(Target.Center) * 16f, type, damage, 0f, Main.myPlayer, 0f, PunchingFromRight.ToInt(), 2);
                    }
                    PunchingFromRight = !PunchingFromRight;
                    CurrentFrame = 0;
                }
            }
            // Pause before attacking
            else if (BigAttackTimer > 0)
            {
                if (BigAttackTimer == PreBigAttackPause)
                {
                    SoundStyle charge = new("CalamityMod/Sounds/Custom/Ravager/RavagerPillarSummon");
                    SoundEngine.PlaySound(charge with { Volume = 0.85f, Pitch = 0.6f }, NPC.Center);
                }
                for (int i = 0; i < 7; i++)
                {
                    Vector2 vel = new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                    Dust cataclysmdust = Dust.NewDustPerfect(NPC.Center + vel * 2, 279, vel);
                    cataclysmdust.noGravity = true;
                    cataclysmdust.scale = Main.rand.NextFloat(1.2f, 1.8f);
                    cataclysmdust.color = Color.Red;
                }

                BigAttackTimer--;
            }
            // Big attack
            else
            {
                // Shoot fists.
                float fireRate = (BossRushEvent.BossRushActive ? 3.5f : MathHelper.Lerp(2f, 5f, 1f - totalLifeRatio) * (isBroDead ? 1.2f : 1)) + (isBroDead ? 19 - BigAttackLimit * 0.1f : 0);
                if (isBroDead && BigAttackLimit == 0)
                    fireRate = 1f;
                PunchCounter += fireRate;
                if (PunchCounter >= PunchCounterLimit)
                {
                    PunchCounter = 0f;
                    SoundEngine.PlaySound(SupremeCalamitas.BrimstoneShotSound with { Volume = 1.2f, Pitch = 0.4f }, NPC.Center);

                    int type = ModContent.ProjectileType<SupremeCataclysmFist>();
                    int damage = NPC.GetProjectileDamage(type);
                    if (bossRush)
                        damage /= 2;

                    if (BigAttackLimit == 0 && isBroDead)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 fistSpawnPosition = NPC.Center + Vector2.UnitX * 74f * NPC.direction;
                            SoundStyle charge = new("CalamityMod/Sounds/NPCKilled/RavagerLimbLoss", 3);
                            SoundEngine.PlaySound(charge with { Volume = 0.65f, Pitch = -0.7f }, NPC.Center);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), fistSpawnPosition, NPC.DirectionTo(Target.Center) * 8f, type, damage, 0f, Main.myPlayer, 0f, PunchingFromRight.ToInt(), 3);
                        }
                    }
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 fistSpawnPosition = NPC.Center;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fistSpawnPosition, NPC.DirectionTo(Target.Center) * 8f, type, damage, 0f, Main.myPlayer, 0f, PunchingFromRight.ToInt(), 1);
                    }
                    PunchingFromRight = !PunchingFromRight;
                    CurrentFrame = 0;
                    if (BigAttackLimit > 0)
                        BigAttackLimit--;
                    else
                    {
                        BigAttackTimer = 600;
                        AttackDelayTimer = 0;
                        BigAttackLimit = 4;
                        if (isBroDead)
                            targetSide = !targetSide;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = NPC.frame.Size() * 0.5f;
            int afterimageCount = 4;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageCount; i += 2)
                {
                    Color afterimageColor = NPC.GetAlpha(Color.Lerp(drawColor, Color.White, 0.5f)) * ((afterimageCount - i) / 15f);
                    Vector2 drawPosition = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                    spriteBatch.Draw(texture, drawPosition, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 mainDrawPosition = NPC.Center - screenPos;
            spriteBatch.Draw(texture, mainDrawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeCataclysmGlow").Value;
            Color primarycolor = Main.zenithWorld ? Color.Blue : Color.Red; // why? because blue fire is awesome!!
            Color baseGlowmaskColor = NPC.IsABestiaryIconDummy ? Color.White : Color.Lerp(Color.White, primarycolor, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageCount; i++)
                {
                    Color afterimageColor = Color.Lerp(baseGlowmaskColor, Color.White, 0.5f) * ((afterimageCount - i) / 15f);
                    Vector2 drawPosition = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                    spriteBatch.Draw(texture, drawPosition, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, mainDrawPosition, NPC.frame, baseGlowmaskColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<SupremeCataclysmTrophy>(), 10);

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int brimDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[brimDust].scale = 0.5f;
                        Main.dust[brimDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int brimDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[brimDust2].noGravity = true;
                    Main.dust[brimDust2].velocity *= 5f;
                    brimDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust2].velocity *= 2f;
                }

                // Turn into dust on death.
                if (NPC.life <= 0)
                    DeathAshParticle.CreateAshesFromNPC(NPC);
            }
        }
    }
}
