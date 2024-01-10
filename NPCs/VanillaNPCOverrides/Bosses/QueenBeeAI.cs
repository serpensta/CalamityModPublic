using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.Projectiles.Boss;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class QueenBeeAI
    {
        public static bool BuffedQueenBeeAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            bool enrage = true;
            int targetTileX = (int)Main.player[npc.target].Center.X / 16;
            int targetTileY = (int)Main.player[npc.target].Center.Y / 16;

            Tile tile = Framing.GetTileSafely(targetTileX, targetTileY);
            if (tile.WallType == WallID.HiveUnsafe)
                enrage = false;

            float maxEnrageScale = 2f;
            float enrageScale = death ? 0.25f : 0f;
            if (((npc.position.Y / 16f) < Main.worldSurface && enrage) || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 0.5f;
            }
            if (!Main.player[npc.target].ZoneJungle || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 0.5f;
            }

            if (Main.getGoodWorld)
                enrageScale += ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 1f : 0.5f);

            if (bossRush)
                enrageScale = 2f;

            if (enrageScale > maxEnrageScale)
                enrageScale = maxEnrageScale;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Bee spawn limit
            int beeLimit = 20;

            // Queen Bee Bee count
            int totalBees = 0;
            bool beeLimitReached = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC bee = Main.npc[i];
                bool isQueenBeeBee = bee.ai[3] == 1f;
                if (bee.active && (bee.type == NPCID.Bee || bee.type == NPCID.BeeSmall) && isQueenBeeBee)
                {
                    totalBees++;
                    if (totalBees >= beeLimit)
                    {
                        beeLimitReached = true;
                        break;
                    }
                }
            }

            // Phases

            // Become more aggressive and start firing double stingers (triple in death mode) phase
            bool phase2 = lifeRatio < 0.8f;

            // Begin launching beehives instead of bees phase, spawn hornets in death mode
            bool phase3 = lifeRatio < 0.6f;

            // Stop spawning bees from ass, spawn bees while charging and become more aggressive phase
            bool phase4 = lifeRatio < 0.3f;

            // Triple stinger (quintuple in death mode) bombardment phase
            bool phase5 = lifeRatio < 0.1f;

            // Despawn
            float distanceFromTarget = Vector2.Distance(npc.Center, Main.player[npc.target].Center);
            if (npc.ai[0] != 5f)
            {
                if (npc.timeLeft < 60)
                    npc.timeLeft = 60;
                if (distanceFromTarget > 3000f)
                    npc.ai[0] = 4f;
            }
            if (Main.player[npc.target].dead)
                npc.ai[0] = 5f;

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = npc.ai[0] == 0f;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<KamiFlu>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            // Always start in enemy spawning phase
            if (calamityGlobalNPC.newAI[3] == 0f)
            {
                calamityGlobalNPC.newAI[3] = 1f;
                npc.ai[0] = 2f;
                npc.netUpdate = true;
                npc.SyncExtraAI();
            }

            // Despawn phase
            if (npc.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                npc.velocity.Y *= 0.98f;

                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;

                if (npc.position.X < (Main.maxTilesX * 8))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;
                    else
                        npc.localAI[0] = 1f;

                    npc.velocity.X -= 0.08f;
                }
                else
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;
                    else
                        npc.localAI[0] = 1f;

                    npc.velocity.X += 0.08f;
                }

                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            // Pick a random phase
            else if (npc.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int phase;
                    do phase = Main.rand.Next(4);
                    while (phase == npc.ai[1] || phase == 1 || (phase == 2 && phase4));

                    npc.TargetClosest();
                    npc.ai[0] = phase;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }

            // Charging phase
            else if (npc.ai[0] == 0f)
            {
                // Number of charges
                int chargeAmt = (int)Math.Ceiling(2f + enrageScale);
                if (phase3)
                    chargeAmt++;
                if (phase4)
                    chargeAmt++;

                // Switch to a random phase if chargeAmt has been exceeded
                if (npc.ai[1] > (2 * chargeAmt) && npc.ai[1] % 2f == 0f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                    return false;
                }

                // Charge velocity
                float speed = 16f;
                if (phase2)
                    speed += 8f;
                if (phase4)
                    speed += 8f;

                speed += 8f * enrageScale;

                // Line up and initiate charge
                if (npc.ai[1] % 2f == 0f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    // Initiate charge
                    float chargeDistance = 20f;
                    chargeDistance += 20f * enrageScale;
                    if (death)
                        chargeDistance += MathHelper.Lerp(0f, 100f, 1f - lifeRatio);

                    if (Math.Abs(npc.position.Y + (npc.height / 2) - (Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2))) < chargeDistance)
                    {
                        // Set damage
                        npc.damage = npc.defDamage;

                        // Set AI variables and speed
                        npc.localAI[0] = 1f;
                        npc.ai[1] += 1f;
                        npc.ai[2] = 0f;

                        // Get target location
                        Vector2 beeLocation = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float targetXDist = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - beeLocation.X;
                        float targetYDist = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - beeLocation.Y;
                        float targetDistance = (float)Math.Sqrt(targetXDist * targetXDist + targetYDist * targetYDist);
                        targetDistance = speed / targetDistance;
                        npc.velocity.X = targetXDist * targetDistance;
                        npc.velocity.Y = targetYDist * targetDistance;

                        // Face the correct direction and play charge sound
                        float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                        npc.direction = playerLocation < 0 ? 1 : -1;
                        npc.spriteDirection = npc.direction;
                        
                        SoundEngine.PlaySound(SoundID.Zombie125, npc.Center);

                        return false;
                    }

                    // Velocity variables
                    npc.localAI[0] = 0f;
                    float chargeVelocity = 12f;
                    float chargeAcceleration = 0.15f;
                    if (phase2)
                    {
                        chargeVelocity += 3f;
                        chargeAcceleration += 0.125f;
                    }
                    if (phase4)
                    {
                        chargeVelocity += 3f;
                        chargeAcceleration += 0.125f;
                    }
                    chargeVelocity += 3f * enrageScale;
                    chargeAcceleration += 0.5f * enrageScale;

                    // Velocity calculations
                    if (npc.position.Y + (npc.height / 2) < Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2))
                        npc.velocity.Y += chargeAcceleration;
                    else
                        npc.velocity.Y -= chargeAcceleration;

                    if (npc.velocity.Y < -chargeVelocity)
                        npc.velocity.Y = -chargeVelocity;
                    if (npc.velocity.Y > chargeVelocity)
                        npc.velocity.Y = chargeVelocity;

                    if (Math.Abs(npc.position.X + (npc.width / 2) - (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))) > 500f)
                        npc.velocity.X += chargeAcceleration * npc.direction;
                    else if (Math.Abs(npc.position.X + (npc.width / 2) - (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))) < 300f)
                        npc.velocity.X -= chargeAcceleration * npc.direction;
                    else
                        npc.velocity.X *= 0.8f;

                    // Limit velocity
                    if (npc.velocity.X < -chargeVelocity)
                        npc.velocity.X = -chargeVelocity;
                    if (npc.velocity.X > chargeVelocity)
                        npc.velocity.X = chargeVelocity;

                    // Face the correct direction
                    float playerLocation2 = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation2 < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
                else
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    // Face the correct direction
                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    // Charging distance from player
                    int chargingDistance = 400;
                    if (phase4)
                        chargingDistance = 600;
                    else if (phase2)
                        chargingDistance = 500;
                    chargingDistance -= (int)(100f * enrageScale);

                    // Get which side of the player the boss is on
                    int chargeDirection = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))
                        chargeDirection = -1;

                    // If boss is in correct position, slow down, if not, reset
                    bool shouldCharge = false;
                    if (npc.direction == chargeDirection && Math.Abs(npc.position.X + (npc.width / 2) - (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))) > chargingDistance)
                    {
                        npc.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > chargingDistance * 1.5f)
                    {
                        npc.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (enrageScale > 0f && shouldCharge)
                        npc.velocity *= MathHelper.Lerp(0.3f, 1f, 1f - enrageScale / maxEnrageScale);

                    // Keep moving
                    if (npc.ai[2] != 1f)
                    {
                        // Velocity fix if Queen Bee is slowed
                        if (npc.velocity.Length() < speed)
                            npc.velocity.X = speed * npc.direction;

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 90f)
                        {
                            npc.SyncExtraAI();
                            npc.velocity.X *= 1.01f;
                        }

                        // Spawn bees
                        bool spawnBee = phase4 && calamityGlobalNPC.newAI[0] % 20f == 0f;
                        if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && spawnBee)
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit1, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int spawnType = Main.rand.Next(NPCID.Bee, NPCID.BeeSmall + 1);
                                if (Main.zenithWorld)
                                {
                                    if (phase3)
                                        spawnType = Main.rand.NextBool(3) ? ModContent.NPCType<PlagueChargerLarge>() : ModContent.NPCType<PlagueCharger>();
                                    else
                                        spawnType = NPCID.Hellbat;
                                }

                                if (!beeLimitReached)
                                {
                                    int spawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, spawnType);
                                    Main.npc[spawn].velocity = Main.player[npc.target].Center - npc.Center;
                                    Main.npc[spawn].velocity.Normalize();
                                    Main.npc[spawn].velocity *= 5f;
                                    if (!Main.zenithWorld)
                                    {
                                        Main.npc[spawn].ai[2] = enrageScale;
                                        Main.npc[spawn].ai[3] = 1f;
                                        Main.npc[spawn].localAI[0] = 60f;
                                    }
                                    Main.npc[spawn].netUpdate = true;
                                }
                            }
                        }

                        npc.localAI[0] = 1f;
                        return false;
                    }

                    float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    // Slow down
                    npc.localAI[0] = 0f;
                    npc.velocity *= 0.9f;

                    float chargeDeceleration = 0.1f;
                    if (phase2)
                    {
                        npc.velocity *= 0.9f;
                        chargeDeceleration += 0.05f;
                    }
                    if (phase4)
                    {
                        npc.velocity *= 0.8f;
                        chargeDeceleration += 0.1f;
                    }
                    if (enrageScale > 0f)
                        npc.velocity *= MathHelper.Lerp(0.5f, 1f, 1f - enrageScale / maxEnrageScale);

                    if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < chargeDeceleration)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        npc.SyncExtraAI();
                    }

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
            }

            // Fly above target before bee spawning phase
            else if (npc.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                // Get target location
                float beeAttackAccel = death ? 0.125f : 0.1f;
                Vector2 beeAttackPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float beeAttackTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - beeAttackPosition.X;
                float beeAttackTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 200f - beeAttackPosition.Y;
                float beeAttackTargetDist = (float)Math.Sqrt(beeAttackTargetX * beeAttackTargetX + beeAttackTargetY * beeAttackTargetY);

                // Go to bee spawn phase
                calamityGlobalNPC.newAI[0] += 1f;
                if (beeAttackTargetDist < 360f || calamityGlobalNPC.newAI[0] >= 180f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    npc.netUpdate = true;
                    npc.SyncExtraAI();
                    return false;
                }

                // Velocity calculations
                if (npc.velocity.X < beeAttackTargetX)
                {
                    npc.velocity.X += beeAttackAccel;
                    if (npc.velocity.X < 0f && beeAttackTargetX > 0f)
                        npc.velocity.X += beeAttackAccel;
                }
                else if (npc.velocity.X > beeAttackTargetX)
                {
                    npc.velocity.X -= beeAttackAccel;
                    if (npc.velocity.X > 0f && beeAttackTargetX < 0f)
                        npc.velocity.X -= beeAttackAccel;
                }
                if (npc.velocity.Y < beeAttackTargetY)
                {
                    npc.velocity.Y += beeAttackAccel;
                    if (npc.velocity.Y < 0f && beeAttackTargetY > 0f)
                        npc.velocity.Y += beeAttackAccel;
                }
                else if (npc.velocity.Y > beeAttackTargetY)
                {
                    npc.velocity.Y -= beeAttackAccel;
                    if (npc.velocity.Y > 0f && beeAttackTargetY < 0f)
                        npc.velocity.Y -= beeAttackAccel;
                }
            }

            // Bee spawn phase
            else if (npc.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                npc.localAI[0] = 0f;

                // Get target location and spawn bees from ass
                Vector2 beeSpawnLocation = new Vector2(npc.position.X + (npc.width / 2) + (Main.rand.Next(20) * npc.direction), npc.position.Y + npc.height * 0.8f);
                Vector2 queenBeeLocation = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float beeSpawnTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - queenBeeLocation.X;
                float beeSpawnTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - queenBeeLocation.Y;
                float beeSpawnTargetDist = (float)Math.Sqrt(beeSpawnTargetX * beeSpawnTargetX + beeSpawnTargetY * beeSpawnTargetY);

                // Bee spawn timer
                npc.ai[1] += 1f;
                int beeSpawnTimer = 0;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && (npc.Center - Main.player[i].Center).Length() < 1000f)
                        beeSpawnTimer++;
                }
                npc.ai[1] += beeSpawnTimer / 2;
                if (phase2)
                    npc.ai[1] += 1f;

                bool spawnBee = false;
                float beeSpawnCheck = (phase3 ? 60f : 15f) - (phase3 ? 24f : 6f) * enrageScale;
                if (npc.ai[1] > beeSpawnCheck)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    spawnBee = true;
                }

                // Spawn bees
                if (Collision.CanHit(beeSpawnLocation, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && spawnBee && !beeLimitReached)
                {
                    if (!phase3 || Main.zenithWorld)
                        SoundEngine.PlaySound(SoundID.NPCHit1, beeSpawnLocation);
                    else
                        SoundEngine.PlaySound(SoundID.NPCHit18, beeSpawnLocation);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (phase3 && !Main.zenithWorld)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), beeSpawnLocation, (Main.player[npc.target].Center - beeSpawnLocation).SafeNormalize(Vector2.UnitY), ProjectileID.BeeHive, 0, 0f, Main.myPlayer, 0f, 0f, 1f);

                            if (death)
                            {
                                int spawnType = Main.rand.Next(NPCID.Bee, NPCID.BeeSmall + 1);
                                int spawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)beeSpawnLocation.X, (int)beeSpawnLocation.Y, spawnType);
                                Main.npc[spawn].velocity = Main.player[npc.target].Center - npc.Center;
                                Main.npc[spawn].velocity.Normalize();
                                Main.npc[spawn].velocity *= 5f;
                                Main.npc[spawn].ai[2] = enrageScale;
                                Main.npc[spawn].ai[3] = 1f;
                                Main.npc[spawn].localAI[0] = 60f;
                                Main.npc[spawn].netUpdate = true;
                            }
                        }
                        else
                        {
                            int spawnType = Main.rand.Next(NPCID.Bee, NPCID.BeeSmall + 1);
                            if (Main.zenithWorld)
                            {
                                if (phase3)
                                    spawnType = Main.rand.NextBool(3) ? ModContent.NPCType<PlagueChargerLarge>() : ModContent.NPCType<PlagueCharger>();
                                else
                                    spawnType = NPCID.Hellbat;
                            }

                            int spawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)beeSpawnLocation.X, (int)beeSpawnLocation.Y, spawnType);
                            Main.npc[spawn].velocity = Main.player[npc.target].Center - npc.Center;
                            Main.npc[spawn].velocity.Normalize();
                            Main.npc[spawn].velocity *= 5f;
                            if (!Main.zenithWorld)
                            {
                                Main.npc[spawn].ai[2] = enrageScale;
                                Main.npc[spawn].ai[3] = 1f;
                                Main.npc[spawn].localAI[0] = 60f;
                            }
                            Main.npc[spawn].netUpdate = true;
                        }
                    }
                }

                // Velocity calculations if target is too far away
                if (beeSpawnTargetDist > 400f || !Collision.CanHit(new Vector2(beeSpawnLocation.X, beeSpawnLocation.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    float beeAttackHoverSpeed = death ? 17.5f : 14f;
                    float beeAttackHoverAccel = death ? 0.125f : 0.1f;
                    queenBeeLocation = beeSpawnLocation;
                    beeSpawnTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - queenBeeLocation.X;
                    beeSpawnTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - queenBeeLocation.Y;
                    beeSpawnTargetDist = (float)Math.Sqrt(beeSpawnTargetX * beeSpawnTargetX + beeSpawnTargetY * beeSpawnTargetY);
                    beeSpawnTargetDist = beeAttackHoverSpeed / beeSpawnTargetDist;

                    if (npc.velocity.X < beeSpawnTargetX)
                    {
                        npc.velocity.X += beeAttackHoverAccel;
                        if (npc.velocity.X < 0f && beeSpawnTargetX > 0f)
                            npc.velocity.X += beeAttackHoverAccel;
                    }
                    else if (npc.velocity.X > beeSpawnTargetX)
                    {
                        npc.velocity.X -= beeAttackHoverAccel;
                        if (npc.velocity.X > 0f && beeSpawnTargetX < 0f)
                            npc.velocity.X -= beeAttackHoverAccel;
                    }
                    if (npc.velocity.Y < beeSpawnTargetY)
                    {
                        npc.velocity.Y += beeAttackHoverAccel;
                        if (npc.velocity.Y < 0f && beeSpawnTargetY > 0f)
                            npc.velocity.Y += beeAttackHoverAccel;
                    }
                    else if (npc.velocity.Y > beeSpawnTargetY)
                    {
                        npc.velocity.Y -= beeAttackHoverAccel;
                        if (npc.velocity.Y > 0f && beeSpawnTargetY < 0f)
                            npc.velocity.Y -= beeAttackHoverAccel;
                    }
                }
                else
                    npc.velocity *= (death ? 0.85f : 0.9f);

                // Face the correct direction
                float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                // Go to a random phase
                float numSpawns = phase3 ? 2f : 5f;
                if (npc.ai[2] > numSpawns || beeLimitReached)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 2f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Stinger phase
            else if (npc.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Get target location and shoot from ass
                Vector2 stingerSpawnLocation = new Vector2(npc.position.X + (npc.width / 2) + (Main.rand.Next(20) * npc.direction), npc.position.Y + npc.height * 0.8f);
                Vector2 stingerQueenBeeLocation = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float stingerAttackTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - stingerQueenBeeLocation.X;
                float stingerAttackTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - (phase4 ? 400f : phase2 ? 350f : 300f) - stingerQueenBeeLocation.Y;
                float stingerAttackTargetDist = (float)Math.Sqrt(stingerAttackTargetX * stingerAttackTargetX + stingerAttackTargetY * stingerAttackTargetY);

                npc.ai[1] += 1f;
                int stingerAttackTimer = phase5 ? 45 : phase2 ? 30 : 20;
                stingerAttackTimer -= (int)Math.Ceiling((phase5 ? 18f : phase2 ? 12f : 8f) * enrageScale);
                if (stingerAttackTimer < 5)
                    stingerAttackTimer = 5;

                // Fire stingers
                if (npc.ai[1] % stingerAttackTimer == (stingerAttackTimer - 1) && npc.position.Y + npc.height < Main.player[npc.target].position.Y && Collision.CanHit(stingerSpawnLocation, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    SoundEngine.PlaySound(SoundID.Item17, stingerSpawnLocation);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float stingerSpeed = 5f;
                        if (phase3)
                            stingerSpeed += 1f;
                        stingerSpeed += 2f * enrageScale;

                        float stingerTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - stingerSpawnLocation.X;
                        float stingerTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - stingerSpawnLocation.Y;
                        float stingerTargetDist = (float)Math.Sqrt(stingerTargetX * stingerTargetX + stingerTargetY * stingerTargetY);
                        stingerTargetDist = stingerSpeed / stingerTargetDist;
                        stingerTargetX *= stingerTargetDist;
                        stingerTargetY *= stingerTargetDist;
                        Vector2 stingerVelocity = new Vector2(stingerTargetX, stingerTargetY);
                        int type = Main.zenithWorld ? (phase3 ? ModContent.ProjectileType<PlagueStingerGoliathV2>() : ProjectileID.FlamingWood) : ProjectileID.QueenBeeStinger;

                        int projectile = Projectile.NewProjectile(npc.GetSource_FromAI(), stingerSpawnLocation, stingerVelocity, type, Main.zenithWorld ? 25 : npc.GetProjectileDamage(type), 0f, Main.myPlayer, 0f, (Main.zenithWorld && phase3) ? Main.player[npc.target].position.Y : 0f);
                        Main.projectile[projectile].timeLeft = 600;
                        Main.projectile[projectile].extraUpdates = 1;

                        if (phase2)
                        {
                            int numExtraStingers = death ? (phase5 ? 4 : 2) : (phase5 ? 2 : 1);
                            for (int i = 0; i < numExtraStingers; i++)
                            {
                                projectile = Projectile.NewProjectile(npc.GetSource_FromAI(), stingerSpawnLocation + Main.rand.NextVector2CircularEdge(16f, 16f) * (i + 1), stingerVelocity * (0.75f * (i * 0.2f + 1)), type, Main.zenithWorld ? 25 : npc.GetProjectileDamage(type), 0f, Main.myPlayer, 0f, (Main.zenithWorld && phase3) ? Main.player[npc.target].position.Y : 0f);
                                Main.projectile[projectile].timeLeft = 600;
                                Main.projectile[projectile].extraUpdates = 1;
                            }
                        }
                    }
                }

                // Movement calculations
                float stingerAttackAccel = phase5 ? 0.09375f : 0.075f;
                stingerAttackAccel += 0.1f * enrageScale;
                if (!Collision.CanHit(new Vector2(stingerSpawnLocation.X, stingerSpawnLocation.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    stingerAttackAccel = phase5 ? 0.125f : 0.1f;
                    if (enrageScale > 0f)
                        stingerAttackAccel = MathHelper.Lerp(phase5 ? 0.15625f : 0.125f, phase5 ? 0.3125f : 0.25f, enrageScale / maxEnrageScale);

                    stingerQueenBeeLocation = stingerSpawnLocation;
                    stingerAttackTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - stingerQueenBeeLocation.X;
                    stingerAttackTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - stingerQueenBeeLocation.Y;

                    if (npc.velocity.X < stingerAttackTargetX)
                    {
                        npc.velocity.X += stingerAttackAccel;
                        if (npc.velocity.X < 0f && stingerAttackTargetX > 0f)
                            npc.velocity.X += stingerAttackAccel;
                    }
                    else if (npc.velocity.X > stingerAttackTargetX)
                    {
                        npc.velocity.X -= stingerAttackAccel;
                        if (npc.velocity.X > 0f && stingerAttackTargetX < 0f)
                            npc.velocity.X -= stingerAttackAccel;
                    }
                    if (npc.velocity.Y < stingerAttackTargetY)
                    {
                        npc.velocity.Y += stingerAttackAccel;
                        if (npc.velocity.Y < 0f && stingerAttackTargetY > 0f)
                            npc.velocity.Y += stingerAttackAccel;
                    }
                    else if (npc.velocity.Y > stingerAttackTargetY)
                    {
                        npc.velocity.Y -= stingerAttackAccel;
                        if (npc.velocity.Y > 0f && stingerAttackTargetY < 0f)
                            npc.velocity.Y -= stingerAttackAccel;
                    }
                }
                else if (stingerAttackTargetDist > 100f)
                {
                    float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    if (npc.velocity.X < stingerAttackTargetX)
                    {
                        npc.velocity.X += stingerAttackAccel;
                        if (npc.velocity.X < 0f && stingerAttackTargetX > 0f)
                            npc.velocity.X += stingerAttackAccel * 2f;
                    }
                    else if (npc.velocity.X > stingerAttackTargetX)
                    {
                        npc.velocity.X -= stingerAttackAccel;
                        if (npc.velocity.X > 0f && stingerAttackTargetX < 0f)
                            npc.velocity.X -= stingerAttackAccel * 2f;
                    }
                    if (npc.velocity.Y < stingerAttackTargetY)
                    {
                        npc.velocity.Y += stingerAttackAccel;
                        if (npc.velocity.Y < 0f && stingerAttackTargetY > 0f)
                            npc.velocity.Y += stingerAttackAccel * 2f;
                    }
                    else if (npc.velocity.Y > stingerAttackTargetY)
                    {
                        npc.velocity.Y -= stingerAttackAccel;
                        if (npc.velocity.Y > 0f && stingerAttackTargetY < 0f)
                            npc.velocity.Y -= stingerAttackAccel * 2f;
                    }
                }

                // Go to a random phase
                if (npc.ai[1] > stingerAttackTimer * 15f && !phase5)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 3f;
                    npc.netUpdate = true;
                }
            }

            else if (npc.ai[0] == 4f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                npc.localAI[0] = 1f;
                float despawnVelMult = 14f;

                Vector2 despawnTargetDist = Main.player[npc.target].Center - npc.Center;
                despawnTargetDist.Normalize();
                despawnTargetDist *= 14f;

                npc.velocity = (npc.velocity * despawnVelMult + despawnTargetDist) / (despawnVelMult + 1f);
                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;

                if (distanceFromTarget < 2000f)
                {
                    npc.ai[0] = -1f;
                    npc.localAI[0] = 0f;
                }
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);

            npc.netSpam = 5;

            return false;
        }
    }
}
