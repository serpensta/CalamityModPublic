using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class DestroyerAI
    {
        public static bool BuffedDestroyerAI(NPC npc, Mod mod)
        {
            int mechdusaCurvedSpineSegmentIndex = 0;
            int mechdusaCurvedSpineSegments = 10;
            if (NPC.IsMechQueenUp && npc.type != NPCID.TheDestroyer)
            {
                int mechdusaIndex = (int)npc.ai[1];
                while (mechdusaIndex > 0 && mechdusaIndex < Main.maxNPCs)
                {
                    if (Main.npc[mechdusaIndex].active && Main.npc[mechdusaIndex].type >= NPCID.TheDestroyer && Main.npc[mechdusaIndex].type <= NPCID.TheDestroyerTail)
                    {
                        mechdusaCurvedSpineSegmentIndex++;
                        if (Main.npc[mechdusaIndex].type == NPCID.TheDestroyer)
                            break;

                        if (mechdusaCurvedSpineSegmentIndex >= mechdusaCurvedSpineSegments)
                        {
                            mechdusaCurvedSpineSegmentIndex = 0;
                            break;
                        }

                        mechdusaIndex = (int)Main.npc[mechdusaIndex].ai[1];
                        continue;
                    }

                    mechdusaCurvedSpineSegmentIndex = 0;
                    break;
                }
            }

            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // 10 seconds of resistance to prevent spawn killing
            if (calamityGlobalNPC.newAI[1] < 600f)
                calamityGlobalNPC.newAI[1] += 1f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = calamityGlobalNPC.newAI[1] < 600f;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases based on life percentage
            bool phase2 = lifeRatio < 0.85f;
            bool phase3 = lifeRatio < 0.7f;
            bool startFlightPhase = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < (death ? 0.4f : 0.25f);
            bool phase5 = lifeRatio < (death ? 0.2f : 0.1f);

            // Flight timer
            float newAISet = phase5 ? 900f : phase4 ? 450f : 0f;
            calamityGlobalNPC.newAI[3] += 1f;
            if (calamityGlobalNPC.newAI[3] >= 1800f)
            {
                calamityGlobalNPC.newAI[3] = newAISet;
                npc.TargetClosest();
            }

            // Set worm variable for worms
            if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = 4f;
            float minimalDamageVelocity = 8f;
            if (npc.type == NPCID.TheDestroyer)
            {
                if (npc.velocity.Length() <= minimalContactDamageVelocity)
                {
                    npc.damage = (int)(npc.defDamage * 0.5f);
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((npc.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp(npc.defDamage * 0.5f, npc.defDamage, velocityDamageScalar);
                }
            }
            else
            {
                float bodyAndTailVelocity = (npc.position - npc.oldPosition).Length();
                if (bodyAndTailVelocity <= minimalContactDamageVelocity)
                {
                    npc.damage = 0;
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp(0f, npc.defDamage, velocityDamageScalar);
                }
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            bool increaseSpeed = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles;
            bool increaseSpeedMore = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;

            // Get a new target if current target is too far away
            if (increaseSpeedMore && npc.type == NPCID.TheDestroyer)
                npc.TargetClosest();

            float enrageScale = bossRush ? 1f : 0f;
            if (Main.dayTime || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Phase for flying at the player
            bool flyAtTarget = (calamityGlobalNPC.newAI[3] >= 900f && startFlightPhase) || (calamityGlobalNPC.newAI[1] < 600f && calamityGlobalNPC.newAI[1] > 60f);

            // Dust on spawn and alpha effects
            if (npc.type == NPCID.TheDestroyer || (npc.type != NPCID.TheDestroyer && Main.npc[(int)npc.ai[1]].alpha < 128))
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int spawnDust = Dust.NewDust(npc.position, npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                        Main.dust[spawnDust].noGravity = true;
                        Main.dust[spawnDust].noLight = true;
                    }
                }
                npc.alpha -= 42;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            // Check if other segments are still alive, if not, die
            if (npc.type > NPCID.TheDestroyer)
            {
                bool shouldDespawn = true;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.TheDestroyer)
                    {
                        shouldDespawn = false;
                        break;
                    }
                }
                if (!shouldDespawn)
                {
                    if (npc.ai[1] <= 0f)
                        shouldDespawn = true;
                    else if (Main.npc[(int)npc.ai[1]].life <= 0)
                        shouldDespawn = true;
                }
                if (shouldDespawn)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.checkDead();
                    npc.active = false;
                }
            }

            if (npc.type == NPCID.TheDestroyerBody)
            {
                // Enrage, fire more cyan lasers
                if (enrageScale > 0f && !bossRush)
                {
                    if (calamityGlobalNPC.newAI[2] < 480f)
                        calamityGlobalNPC.newAI[2] += 1f;
                }
                else
                {
                    if (calamityGlobalNPC.newAI[2] > 0f)
                        calamityGlobalNPC.newAI[2] -= 1f;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.type == NPCID.TheDestroyer)
                {
                    // Spawn segments from head
                    if (npc.ai[0] == 0f)
                    {
                        npc.ai[3] = npc.whoAmI;
                        npc.realLife = npc.whoAmI;
                        int index = npc.whoAmI;
                        int totalSegments = Main.getGoodWorld ? 100 : 80;
                        for (int j = 0; j <= totalSegments; j++)
                        {
                            int type = NPCID.TheDestroyerBody;
                            if (j == totalSegments)
                                type = NPCID.TheDestroyerTail;

                            int segment = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)(npc.position.Y + npc.height), type, npc.whoAmI);
                            Main.npc[segment].ai[3] = npc.whoAmI;
                            Main.npc[segment].realLife = npc.whoAmI;
                            Main.npc[segment].ai[1] = index;
                            Main.npc[index].ai[0] = segment;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment, 0f, 0f, 0f, 0, 0, 0);
                            index = segment;
                        }
                    }

                    // Laser breath in Death Mode
                    if (death)
                    {
                        if (calamityGlobalNPC.newAI[0] < 600f)
                            calamityGlobalNPC.newAI[0] += 1f;

                        if (npc.SafeDirectionTo(player.Center).AngleBetween((npc.rotation - MathHelper.PiOver2).ToRotationVector2()) < MathHelper.ToRadians(18f) &&
                            calamityGlobalNPC.newAI[0] >= 600f && Vector2.Distance(npc.Center, player.Center) > 480f &&
                            Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
                            if (calamityGlobalNPC.newAI[0] % 30f == 0f)
                            {
                                float velocity = bossRush ? 6f : death ? 5.333f : 5f;
                                int type = ProjectileID.DeathLaser;
                                int damage = npc.GetProjectileDamage(type);
                                Vector2 projectileVelocity = (player.Center - npc.Center).SafeNormalize(Vector2.UnitY) * velocity;
                                int numProj = calamityGlobalNPC.newAI[0] % 60f == 0f ? 7 : 4;
                                int spread = 54;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 5f, perturbedSpeed, type, damage, 0f, Main.myPlayer, 1f, 0f);
                                    Main.projectile[proj].timeLeft = 900;
                                }
                            }

                            calamityGlobalNPC.newAI[0] += 1f;
                            if (calamityGlobalNPC.newAI[0] > 660f)
                                calamityGlobalNPC.newAI[0] = 0f;
                        }
                    }
                }

                // Fire lasers
                if (npc.type == NPCID.TheDestroyerBody)
                {
                    // Laser rate of fire
                    calamityGlobalNPC.newAI[0] += 1f;
                    float shootProjectile = death ? 180 : 300;
                    float timer = npc.ai[0] * 30f;
                    float shootProjectileGateValue = timer + shootProjectile;

                    // Shoot lasers
                    // 50% chance to shoot harmless scrap if probe has been launched
                    bool probeLaunched = npc.ai[2] == 1f;
                    if (calamityGlobalNPC.newAI[0] >= shootProjectileGateValue)
                    {
                        calamityGlobalNPC.newAI[0] = 0f;
                        npc.TargetClosest();
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
                            // Laser speed
                            float projectileSpeed = 3.5f + Main.rand.NextFloat() * 1.5f;
                            projectileSpeed += enrageScale;

                            // Set projectile damage and type
                            int projectileType = ProjectileID.DeathLaser;
                            float laserSpawnDistance = 10f;
                            int random = phase3 ? 4 : phase2 ? 3 : 2;
                            switch (Main.rand.Next(random))
                            {
                                case 0:
                                case 1:
                                    break;
                                case 2:
                                    projectileType = ModContent.ProjectileType<DestroyerCursedLaser>();
                                    break;
                                case 3:
                                    projectileType = ModContent.ProjectileType<DestroyerElectricLaser>();
                                    break;
                            }

                            if (calamityGlobalNPC.newAI[2] > 0f || bossRush)
                            {
                                projectileType = ModContent.ProjectileType<DestroyerElectricLaser>();
                                laserSpawnDistance = 20f;
                            }

                            bool weakLaser = false;
                            if (probeLaunched)
                            {
                                weakLaser = true;
                                projectileType = ProjectileID.EyeLaser;
                                laserSpawnDistance = 0f;
                            }

                            // Get target vector
                            Vector2 projectileVelocity = (player.Center - npc.Center).SafeNormalize(Vector2.UnitY) * projectileSpeed;
                            Vector2 projectileSpawn = npc.Center + projectileVelocity * laserSpawnDistance;

                            // Shoot projectile
                            int damage = npc.GetProjectileDamage(projectileType);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, damage, 0f, Main.myPlayer, weakLaser ? 0f : 1f, 0f);
                            Main.projectile[proj].timeLeft = weakLaser ? 600 : 900;

                            npc.netUpdate = true;
                        }
                    }
                }
            }

            int tilePosX = (int)(npc.position.X / 16f) - 1;
            int tileWidthPosX = (int)((npc.position.X + npc.width) / 16f) + 2;
            int tilePosY = (int)(npc.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((npc.position.Y + npc.height) / 16f) + 2;

            if (tilePosX < 0)
                tilePosX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePosY < 0)
                tilePosY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            // Fly or not
            bool shouldFly = flyAtTarget;
            if (!shouldFly)
            {
                for (int k = tilePosX; k < tileWidthPosX; k++)
                {
                    for (int l = tilePosY; l < tileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[Main.tile[k, l].TileType] || (Main.tileSolidTop[Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 tileConvertedPosition;
                            tileConvertedPosition.X = k * 16;
                            tileConvertedPosition.Y = l * 16;
                            if (npc.position.X + npc.width > tileConvertedPosition.X && npc.position.X < tileConvertedPosition.X + 16f && npc.position.Y + npc.height > tileConvertedPosition.Y && npc.position.Y < tileConvertedPosition.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Start flying if target is not within a certain distance
            if (!shouldFly)
            {
                npc.localAI[1] = 1f;

                if (npc.type == NPCID.TheDestroyer)
                {
                    Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                    int noFlyZone = 1000;
                    int heightReduction = death ? 400 : (int)(400f * (1f - lifeRatio));
                    int height = 1800 - heightReduction;
                    bool outsideNoFlyZone = true;

                    if (npc.position.Y > player.position.Y)
                    {
                        for (int m = 0; m < Main.maxPlayers; m++)
                        {
                            if (Main.player[m].active)
                            {
                                Rectangle noFlyRectangle = new Rectangle((int)Main.player[m].position.X - noFlyZone, (int)Main.player[m].position.Y - noFlyZone, noFlyZone * 2, height);
                                if (rectangle.Intersects(noFlyRectangle))
                                {
                                    outsideNoFlyZone = false;
                                    break;
                                }
                            }
                        }
                        if (outsideNoFlyZone)
                            shouldFly = true;
                    }
                }
            }
            else
                npc.localAI[1] = 0f;

            // Despawn
            float fallSpeed = 16f;
            if (player.dead)
            {
                shouldFly = false;
                npc.velocity.Y += 2f;

                if (npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y += 2f;
                    fallSpeed = 32f;
                }

                if (npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        if (Main.npc[n].aiStyle == npc.aiStyle)
                            Main.npc[n].active = false;
                    }
                }
            }

            float fallSpeedBoost = death ? 6.5f * (1f - lifeRatio) : 5f * (1f - lifeRatio);
            fallSpeed += fallSpeedBoost;
            fallSpeed += 4f * enrageScale;

            // Speed and movement
            float speedBoost = death ? (0.14f * (1f - lifeRatio)) : (0.1f * (1f - lifeRatio));
            float turnSpeedBoost = death ? (0.19f * (1f - lifeRatio)) : (0.15f * (1f - lifeRatio));
            float speed = 0.1f + speedBoost;
            float turnSpeed = 0.15f + turnSpeedBoost;
            speed += 0.04f * enrageScale;
            turnSpeed += 0.06f * enrageScale;

            if (flyAtTarget)
            {
                float speedMultiplier = phase5 ? 1.8f : phase4 ? 1.65f : 1.5f;
                speed *= speedMultiplier;
            }

            speed *= increaseSpeedMore ? 2f : increaseSpeed ? 1.5f : 1f;
            turnSpeed *= increaseSpeedMore ? 2f : increaseSpeed ? 1.5f : 1f;

            if (Main.getGoodWorld)
            {
                speed *= 1.2f;
                turnSpeed *= 1.2f;
            }

            Vector2 npcCenter = npc.Center;
            float targetTilePosX = player.position.X + (player.width / 2);
            float targetTilePosY = player.position.Y + (player.height / 2);
            targetTilePosX = (int)(targetTilePosX / 16f) * 16;
            targetTilePosY = (int)(targetTilePosY / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16;
            npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            targetTilePosX -= npcCenter.X;
            targetTilePosY -= npcCenter.Y;
            float targetTileDist = (float)Math.Sqrt(targetTilePosX * targetTilePosX + targetTilePosY * targetTilePosY);

            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                int mechdusaSegmentScale = (int)(44f * npc.scale);
                try
                {
                    npcCenter = npc.Center;
                    targetTilePosX = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - npcCenter.X;
                    targetTilePosY = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - npcCenter.Y;
                }
                catch
                {
                }

                if (mechdusaCurvedSpineSegmentIndex > 0)
                {
                    float absoluteTilePosX = (float)mechdusaSegmentScale - (float)mechdusaSegmentScale * (((float)mechdusaCurvedSpineSegmentIndex - 1f) * 0.1f);
                    if (absoluteTilePosX < 0f)
                        absoluteTilePosX = 0f;

                    if (absoluteTilePosX > (float)mechdusaSegmentScale)
                        absoluteTilePosX = mechdusaSegmentScale;

                    targetTilePosY = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) + absoluteTilePosX - npcCenter.Y;
                }

                npc.rotation = (float)Math.Atan2(targetTilePosY, targetTilePosX) + MathHelper.PiOver2;
                targetTileDist = (float)Math.Sqrt(targetTilePosX * targetTilePosX + targetTilePosY * targetTilePosY);
                if (mechdusaCurvedSpineSegmentIndex > 0)
                    mechdusaSegmentScale = mechdusaSegmentScale / mechdusaCurvedSpineSegments * mechdusaCurvedSpineSegmentIndex;

                targetTileDist = (targetTileDist - mechdusaSegmentScale) / targetTileDist;
                targetTilePosX *= targetTileDist;
                targetTilePosY *= targetTileDist;
                npc.velocity = Vector2.Zero;
                npc.position.X += targetTilePosX;
                npc.position.Y += targetTilePosY;
            }
            else
            {
                if (!shouldFly)
                {
                    npc.velocity.Y += 0.15f;
                    if (npc.velocity.Y > fallSpeed)
                        npc.velocity.Y = fallSpeed;

                    // This bool exists to stop the strange wiggle behavior when worms are falling down
                    bool slowXVelocity = Math.Abs(npc.velocity.X) > speed;
                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.4)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X -= speed * 1.1f;
                        else
                            npc.velocity.X += speed * 1.1f;
                    }
                    else if (npc.velocity.Y == fallSpeed)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < targetTilePosX)
                                npc.velocity.X += speed;
                            else if (npc.velocity.X > targetTilePosX)
                                npc.velocity.X -= speed;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X += speed * 0.9f;
                            else
                                npc.velocity.X -= speed * 0.9f;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                }
                else
                {
                    if (npc.soundDelay == 0)
                    {
                        float soundDelay = targetTileDist / 40f;
                        if (soundDelay < 10f)
                            soundDelay = 10f;
                        if (soundDelay > 20f)
                            soundDelay = 20f;

                        npc.soundDelay = (int)soundDelay;
                        SoundEngine.PlaySound(SoundID.WormDig, npc.position);
                    }

                    targetTileDist = (float)Math.Sqrt(targetTilePosX * targetTilePosX + targetTilePosY * targetTilePosY);
                    float absoluteTilePosX = Math.Abs(targetTilePosX);
                    float absoluteTilePosY = Math.Abs(targetTilePosY);
                    float tileToReachTarget = fallSpeed / targetTileDist;
                    targetTilePosX *= tileToReachTarget;
                    targetTilePosY *= tileToReachTarget;

                    bool flyWyvernMovement = false;
                    if (flyAtTarget)
                    {
                        if (((npc.velocity.X > 0f && targetTilePosX < 0f) || (npc.velocity.X < 0f && targetTilePosX > 0f) || (npc.velocity.Y > 0f && targetTilePosY < 0f) || (npc.velocity.Y < 0f && targetTilePosY > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > speed / 2f && targetTileDist < 400f)
                        {
                            flyWyvernMovement = true;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < fallSpeed)
                                npc.velocity *= 1.1f;
                        }

                        if (npc.position.Y > player.position.Y)
                        {
                            flyWyvernMovement = true;

                            if (Math.Abs(npc.velocity.X) < fallSpeed / 2f)
                            {
                                if (npc.velocity.X == 0f)
                                    npc.velocity.X -= npc.direction;

                                npc.velocity.X *= 1.1f;
                            }
                            else if (npc.velocity.Y > -fallSpeed)
                                npc.velocity.Y -= speed;
                        }
                    }

                    if (!flyWyvernMovement)
                    {
                        if (!flyAtTarget)
                        {
                            if (((npc.velocity.X > 0f && targetTilePosX > 0f) || (npc.velocity.X < 0f && targetTilePosX < 0f)) && ((npc.velocity.Y > 0f && targetTilePosY > 0f) || (npc.velocity.Y < 0f && targetTilePosY < 0f)))
                            {
                                if (npc.velocity.X < targetTilePosX)
                                    npc.velocity.X += turnSpeed;
                                else if (npc.velocity.X > targetTilePosX)
                                    npc.velocity.X -= turnSpeed;
                                if (npc.velocity.Y < targetTilePosY)
                                    npc.velocity.Y += turnSpeed;
                                else if (npc.velocity.Y > targetTilePosY)
                                    npc.velocity.Y -= turnSpeed;
                            }
                        }

                        if ((npc.velocity.X > 0f && targetTilePosX > 0f) || (npc.velocity.X < 0f && targetTilePosX < 0f) || (npc.velocity.Y > 0f && targetTilePosY > 0f) || (npc.velocity.Y < 0f && targetTilePosY < 0f))
                        {
                            if (npc.velocity.X < targetTilePosX)
                                npc.velocity.X += speed;
                            else if (npc.velocity.X > targetTilePosX)
                                npc.velocity.X -= speed;
                            if (npc.velocity.Y < targetTilePosY)
                                npc.velocity.Y += speed;
                            else if (npc.velocity.Y > targetTilePosY)
                                npc.velocity.Y -= speed;

                            if (Math.Abs(targetTilePosY) < fallSpeed * 0.2 && ((npc.velocity.X > 0f && targetTilePosX < 0f) || (npc.velocity.X < 0f && targetTilePosX > 0f)))
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += speed * 2f;
                                else
                                    npc.velocity.Y -= speed * 2f;
                            }
                            if (Math.Abs(targetTilePosX) < fallSpeed * 0.2 && ((npc.velocity.Y > 0f && targetTilePosY < 0f) || (npc.velocity.Y < 0f && targetTilePosY > 0f)))
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += speed * 2f;
                                else
                                    npc.velocity.X -= speed * 2f;
                            }
                        }
                        else if (absoluteTilePosX > absoluteTilePosY)
                        {
                            if (npc.velocity.X < targetTilePosX)
                                npc.velocity.X += speed * 1.1f;
                            else if (npc.velocity.X > targetTilePosX)
                                npc.velocity.X -= speed * 1.1f;

                            if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.5)
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += speed;
                                else
                                    npc.velocity.Y -= speed;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < targetTilePosY)
                                npc.velocity.Y += speed * 1.1f;
                            else if (npc.velocity.Y > targetTilePosY)
                                npc.velocity.Y -= speed * 1.1f;

                            if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.5)
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += speed;
                                else
                                    npc.velocity.X -= speed;
                            }
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

                if (npc.type == NPCID.TheDestroyer)
                {
                    if (shouldFly)
                    {
                        if (npc.localAI[0] != 1f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 1f;
                    }
                    else
                    {
                        if (npc.localAI[0] != 0f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 0f;
                    }

                    if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                        npc.netUpdate = true;
                }
            }

            if (NPC.IsMechQueenUp && npc.type == NPCID.TheDestroyer)
            {
                NPC nPC = Main.npc[NPC.mechQueen];
                Vector2 mechQueenCenter = nPC.GetMechQueenCenter();
                Vector2 mechdusaSpinningVector = new Vector2(0f, 100f);
                Vector2 spinningpoint = mechQueenCenter + mechdusaSpinningVector;
                float mechdusaRotation = nPC.velocity.X * 0.025f;
                spinningpoint = spinningpoint.RotatedBy(mechdusaRotation, mechQueenCenter);
                npc.position = spinningpoint - npc.Size / 2f + nPC.velocity;
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                npc.rotation = mechdusaRotation * 0.75f + (float)Math.PI;
            }

            return false;
        }

        public static bool VanillaDestroyerAI(NPC npc, Mod mod)
        {
            int num = 0;
            int num2 = 10;
            if (NPC.IsMechQueenUp && npc.type != NPCID.TheDestroyer)
            {
                int num3 = (int)npc.ai[1];
                while (num3 > 0 && num3 < Main.maxNPCs)
                {
                    if (Main.npc[num3].active && Main.npc[num3].type >= NPCID.TheDestroyer && Main.npc[num3].type <= NPCID.TheDestroyerTail)
                    {
                        num++;
                        if (Main.npc[num3].type == NPCID.TheDestroyer)
                            break;

                        if (num >= num2)
                        {
                            num = 0;
                            break;
                        }

                        num3 = (int)Main.npc[num3].ai[1];
                        continue;
                    }

                    num = 0;
                    break;
                }
            }

            if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = 4f;
            float minimalDamageVelocity = 8f;
            if (npc.type == NPCID.TheDestroyer)
            {
                if (npc.velocity.Length() <= minimalContactDamageVelocity)
                {
                    npc.damage = (int)(npc.defDamage * 0.5f);
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((npc.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp(npc.defDamage * 0.5f, npc.defDamage, velocityDamageScalar);
                }
            }
            else
            {
                float bodyAndTailVelocity = (npc.position - npc.oldPosition).Length();
                if (bodyAndTailVelocity <= minimalContactDamageVelocity)
                {
                    npc.damage = 0;
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp(0f, npc.defDamage, velocityDamageScalar);
                }
            }

            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead)
                npc.TargetClosest();

            if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail)
            {
                if (npc.type == NPCID.TheDestroyer || (npc.type != NPCID.TheDestroyer && Main.npc[(int)npc.ai[1]].alpha < 128))
                {
                    if (npc.alpha != 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int num4 = Dust.NewDust(npc.position, npc.width, npc.height, 182, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num4].noGravity = true;
                            Main.dust[num4].noLight = true;
                        }
                    }

                    npc.alpha -= 42;
                    if (npc.alpha < 0)
                        npc.alpha = 0;
                }
            }

            if (npc.type > NPCID.TheDestroyer)
            {
                bool flag = false;
                if (npc.ai[1] <= 0f)
                    flag = true;
                else if (Main.npc[(int)npc.ai[1]].life <= 0)
                    flag = true;

                if (flag)
                {
                    npc.life = 0;
                    npc.HitEffect();
                    npc.checkDead();
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f && npc.type == NPCID.TheDestroyer)
                {
                    npc.ai[3] = npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int num5 = 0;
                    int num6 = npc.whoAmI;
                    int destroyerSegmentsCount = NPC.GetDestroyerSegmentsCount();
                    for (int j = 0; j <= destroyerSegmentsCount; j++)
                    {
                        int num7 = NPCID.TheDestroyerBody;
                        if (j == destroyerSegmentsCount)
                            num7 = NPCID.TheDestroyerTail;

                        num5 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X), (int)(npc.position.Y + (float)npc.height), num7, npc.whoAmI);
                        Main.npc[num5].ai[3] = npc.whoAmI;
                        Main.npc[num5].realLife = npc.whoAmI;
                        Main.npc[num5].ai[1] = num6;
                        Main.npc[num6].ai[0] = num5;
                        NetMessage.SendData(23, -1, -1, null, num5);
                        num6 = num5;
                    }
                }

                if (npc.type == NPCID.TheDestroyerBody)
                {
                    npc.localAI[0] += Main.rand.Next(4);
                    int chanceToFire = Main.rand.Next(1400, 26000);
                    if (Main.expertMode)
                        chanceToFire = (int)(chanceToFire * MathHelper.Lerp(Main.masterMode ? 0.5f : 0.7f, 1f, npc.life / (float)npc.lifeMax));

                    if (npc.localAI[0] >= (float)chanceToFire)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest();
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            float laserVelocity = Main.masterMode ? 9f : Main.expertMode ? 8.5f : 8f;
                            Vector2 vector = npc.Center;
                            float num8 = Main.player[npc.target].Center.X - vector.X + (float)Main.rand.Next(-2, 3);
                            float num9 = Main.player[npc.target].Center.Y - vector.Y + (float)Main.rand.Next(-2, 3);
                            float num10 = (float)Math.Sqrt(num8 * num8 + num9 * num9);
                            num10 = laserVelocity / num10;
                            num8 *= num10;
                            num9 *= num10;
                            num8 += (float)Main.rand.Next(-2, 3) * 0.05f;
                            num9 += (float)Main.rand.Next(-2, 3) * 0.05f;
                            int attackDamage_ForProjectiles = npc.GetAttackDamage_ForProjectiles(22f, 18f);
                            int num11 = ProjectileID.DeathLaser;
                            vector.X += num8 * 5f;
                            vector.Y += num9 * 5f;
                            int num12 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector.X, vector.Y, num8, num9, num11, attackDamage_ForProjectiles, 0f, Main.myPlayer);
                            Main.projectile[num12].timeLeft = 300;
                            npc.netUpdate = true;
                        }
                    }
                }
            }

            int num13 = (int)(npc.position.X / 16f) - 1;
            int num14 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num15 = (int)(npc.position.Y / 16f) - 1;
            int num16 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num13 < 0)
                num13 = 0;

            if (num14 > Main.maxTilesX)
                num14 = Main.maxTilesX;

            if (num15 < 0)
                num15 = 0;

            if (num16 > Main.maxTilesY)
                num16 = Main.maxTilesY;

            bool flag2 = false;
            if (!flag2)
            {
                Vector2 vector2 = default(Vector2);
                for (int k = num13; k < num14; k++)
                {
                    for (int l = num15; l < num16; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[Main.tile[k, l].TileType] || (Main.tileSolidTop[Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            vector2.X = k * 16;
                            vector2.Y = l * 16;
                            if (npc.position.X + (float)npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + (float)npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!flag2)
            {
                if (npc.type != NPCID.TheDestroyerBody || npc.ai[2] != 1f)
                    Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0.3f, 0.1f, 0.05f);

                npc.localAI[1] = 1f;
                if (npc.type == NPCID.TheDestroyer)
                {
                    Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                    int num17 = 1000;
                    int height = (num17 * 2) - (Main.masterMode ? 700 : Main.expertMode ? 400 : 0);
                    bool flag3 = true;
                    if (npc.position.Y > Main.player[npc.target].position.Y)
                    {
                        for (int m = 0; m < Main.maxPlayers; m++)
                        {
                            if (Main.player[m].active)
                            {
                                Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num17, (int)Main.player[m].position.Y - num17, num17 * 2, height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    flag3 = false;
                                    break;
                                }
                            }
                        }

                        if (flag3)
                            flag2 = true;
                    }
                }
            }
            else
                npc.localAI[1] = 0f;

            float num18 = Main.masterMode ? 24f : Main.expertMode ? 20f : 16f;
            if (Main.IsItDay() || Main.player[npc.target].dead)
            {
                flag2 = false;
                npc.velocity.Y += 1f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y += 1f;
                    num18 *= 2f;
                }

                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        if (Main.npc[n].aiStyle == npc.aiStyle)
                            Main.npc[n].active = false;
                    }
                }
            }

            float num19 = 0.1f;
            float num20 = 0.15f;
            if (Main.expertMode)
            {
                num19 = Main.masterMode ? 0.15f : 0.125f;
                num20 = Main.masterMode ? 0.225f : 0.1875f;
            }

            if (Main.getGoodWorld)
            {
                num19 *= 1.2f;
                num20 *= 1.2f;
            }

            Vector2 vector3 = npc.Center;
            float num21 = Main.player[npc.target].Center.X;
            float num22 = Main.player[npc.target].Center.Y;
            num21 = (int)(num21 / 16f) * 16;
            num22 = (int)(num22 / 16f) * 16;
            vector3.X = (int)(vector3.X / 16f) * 16;
            vector3.Y = (int)(vector3.Y / 16f) * 16;
            num21 -= vector3.X;
            num22 -= vector3.Y;
            float num23 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                int num24 = (int)(44f * npc.scale);
                try
                {
                    vector3 = npc.Center;
                    num21 = Main.npc[(int)npc.ai[1]].Center.X - vector3.X;
                    num22 = Main.npc[(int)npc.ai[1]].Center.Y - vector3.Y;
                }
                catch
                {
                }

                if (num > 0)
                {
                    float num25 = (float)num24 - (float)num24 * (((float)num - 1f) * 0.1f);
                    if (num25 < 0f)
                        num25 = 0f;

                    if (num25 > (float)num24)
                        num25 = num24;

                    num22 = Main.npc[(int)npc.ai[1]].Center.Y + num25 - vector3.Y;
                }

                npc.rotation = (float)Math.Atan2(num22, num21) + MathHelper.PiOver2;
                num23 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
                if (num > 0)
                    num24 = num24 / num2 * num;

                num23 = (num23 - (float)num24) / num23;
                num21 *= num23;
                num22 *= num23;
                npc.velocity = Vector2.Zero;
                npc.position.X += num21;
                npc.position.Y += num22;
                num21 = Main.npc[(int)npc.ai[1]].Center.X - vector3.X;
                num22 = Main.npc[(int)npc.ai[1]].Center.Y - vector3.Y;
                npc.rotation = (float)Math.Atan2(num22, num21) + MathHelper.PiOver2;
            }
            else
            {
                if (!flag2)
                {
                    npc.TargetClosest();
                    npc.velocity.Y += 0.15f;
                    if (Main.masterMode && npc.velocity.Y > 0f)
                        npc.velocity.Y += 0.05f;

                    if (npc.velocity.Y > num18)
                        npc.velocity.Y = num18;

                    // This bool exists to stop the strange wiggle behavior when worms are falling down
                    bool slowXVelocity = Math.Abs(npc.velocity.X) > num19;
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num18 * 0.4)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X -= num19 * 1.1f;
                        else
                            npc.velocity.X += num19 * 1.1f;
                    }
                    else if (npc.velocity.Y == num18)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < num21)
                                npc.velocity.X += num19;
                            else if (npc.velocity.X > num21)
                                npc.velocity.X -= num19;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X += num19 * 0.9f;
                            else
                                npc.velocity.X -= num19 * 0.9f;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                }
                else
                {
                    if (npc.soundDelay == 0)
                    {
                        float num26 = num23 / 40f;
                        if (num26 < 10f)
                            num26 = 10f;

                        if (num26 > 20f)
                            num26 = 20f;

                        npc.soundDelay = (int)num26;
                        SoundEngine.PlaySound(SoundID.WormDig, npc.Center);
                    }

                    num23 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
                    float num27 = Math.Abs(num21);
                    float num28 = Math.Abs(num22);
                    float num29 = num18 / num23;
                    num21 *= num29;
                    num22 *= num29;
                    if (((npc.velocity.X > 0f && num21 > 0f) || (npc.velocity.X < 0f && num21 < 0f)) && ((npc.velocity.Y > 0f && num22 > 0f) || (npc.velocity.Y < 0f && num22 < 0f)))
                    {
                        if (npc.velocity.X < num21)
                            npc.velocity.X += num20;
                        else if (npc.velocity.X > num21)
                            npc.velocity.X -= num20;

                        if (npc.velocity.Y < num22)
                            npc.velocity.Y += num20;
                        else if (npc.velocity.Y > num22)
                            npc.velocity.Y -= num20;
                    }

                    if ((npc.velocity.X > 0f && num21 > 0f) || (npc.velocity.X < 0f && num21 < 0f) || (npc.velocity.Y > 0f && num22 > 0f) || (npc.velocity.Y < 0f && num22 < 0f))
                    {
                        if (npc.velocity.X < num21)
                            npc.velocity.X += num19;
                        else if (npc.velocity.X > num21)
                            npc.velocity.X -= num19;

                        if (npc.velocity.Y < num22)
                            npc.velocity.Y += num19;
                        else if (npc.velocity.Y > num22)
                            npc.velocity.Y -= num19;

                        if ((double)Math.Abs(num22) < (double)num18 * 0.2 && ((npc.velocity.X > 0f && num21 < 0f) || (npc.velocity.X < 0f && num21 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += num19 * 2f;
                            else
                                npc.velocity.Y -= num19 * 2f;
                        }

                        if ((double)Math.Abs(num21) < (double)num18 * 0.2 && ((npc.velocity.Y > 0f && num22 < 0f) || (npc.velocity.Y < 0f && num22 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += num19 * 2f;
                            else
                                npc.velocity.X -= num19 * 2f;
                        }
                    }
                    else if (num27 > num28)
                    {
                        if (npc.velocity.X < num21)
                            npc.velocity.X += num19 * 1.1f;
                        else if (npc.velocity.X > num21)
                            npc.velocity.X -= num19 * 1.1f;

                        if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num18 * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += num19;
                            else
                                npc.velocity.Y -= num19;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num22)
                            npc.velocity.Y += num19 * 1.1f;
                        else if (npc.velocity.Y > num22)
                            npc.velocity.Y -= num19 * 1.1f;

                        if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num18 * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += num19;
                            else
                                npc.velocity.X -= num19;
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
                if (npc.type == NPCID.TheDestroyer)
                {
                    if (flag2)
                    {
                        if (npc.localAI[0] != 1f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 1f;
                    }
                    else
                    {
                        if (npc.localAI[0] != 0f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 0f;
                    }

                    if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                        npc.netUpdate = true;
                }
            }

            if (NPC.IsMechQueenUp && npc.type == NPCID.TheDestroyer)
            {
                NPC nPC = Main.npc[NPC.mechQueen];
                Vector2 mechQueenCenter = nPC.GetMechQueenCenter();
                Vector2 vector4 = new Vector2(0f, 100f);
                Vector2 spinningpoint = mechQueenCenter + vector4;
                float num30 = nPC.velocity.X * 0.025f;
                spinningpoint = spinningpoint.RotatedBy(num30, mechQueenCenter);
                npc.position = spinningpoint - npc.Size / 2f + nPC.velocity;
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                npc.rotation = num30 * 0.75f + (float)Math.PI;
            }

            return false;
        }

        public static bool BuffedProbeAI(NPC npc, Mod mod)
        {
            bool bossRush = BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            NPCAimedTarget targetData = npc.GetTargetData();
            bool targetDead = false;
            if (targetData.Type == NPCTargetType.Player)
                targetDead = Main.player[npc.target].dead;

            float velocity = bossRush ? 8f : 6f;
            float acceleration = bossRush ? 0.1f : 0.05f;

            Vector2 probeCenter = npc.Center;
            float probeTargetX = targetData.Center.X;
            float probeTargetY = targetData.Center.Y;
            probeTargetX = (int)(probeTargetX / 8f) * 8;
            probeTargetY = (int)(probeTargetY / 8f) * 8;
            probeCenter.X = (int)(probeCenter.X / 8f) * 8;
            probeCenter.Y = (int)(probeCenter.Y / 8f) * 8;
            probeTargetX -= probeCenter.X;
            probeTargetY -= probeCenter.Y;
            float distanceFromTarget = (float)Math.Sqrt(probeTargetX * probeTargetX + probeTargetY * probeTargetY);
            float distance2 = distanceFromTarget;

            bool farAwayFromTarget = false;
            if (distanceFromTarget > 600f)
                farAwayFromTarget = true;

            if (distanceFromTarget == 0f)
            {
                probeTargetX = npc.velocity.X;
                probeTargetY = npc.velocity.Y;
            }
            else
            {
                distanceFromTarget = velocity / distanceFromTarget;
                probeTargetX *= distanceFromTarget;
                probeTargetY *= distanceFromTarget;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type)
                {
                    Vector2 otherProbeDist = Main.npc[i].Center - npc.Center;
                    if (otherProbeDist.Length() < (npc.width + npc.height))
                    {
                        otherProbeDist = otherProbeDist.SafeNormalize(Vector2.UnitY);
                        otherProbeDist *= -0.1f;
                        npc.velocity += otherProbeDist;
                        Main.npc[i].velocity -= otherProbeDist;
                    }
                }
            }

            if (distance2 > 100f)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                    npc.velocity.Y += 0.023f;
                else
                    npc.velocity.Y -= 0.023f;

                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                    npc.velocity.X += 0.023f;
                else
                    npc.velocity.X -= 0.023f;

                if (npc.ai[0] > 200f)
                    npc.ai[0] = -200f;
            }

            if (targetDead)
            {
                probeTargetX = npc.direction * velocity / 2f;
                probeTargetY = -velocity / 2f;
            }

            if (npc.ai[3] != 0f)
            {
                if (NPC.IsMechQueenUp)
                {
                    NPC nPC = Main.npc[NPC.mechQueen];
                    Vector2 tileConvertedPosition = new Vector2(26f * npc.ai[3], 0f);
                    int mechdusaProbe = (int)npc.ai[2];
                    if (mechdusaProbe < 0 || mechdusaProbe >= Main.maxNPCs)
                    {
                        mechdusaProbe = NPC.FindFirstNPC(NPCID.TheDestroyer);
                        npc.ai[2] = mechdusaProbe;
                        npc.netUpdate = true;
                    }

                    if (mechdusaProbe > -1)
                    {
                        NPC nPC2 = Main.npc[mechdusaProbe];
                        if (!nPC2.active || nPC2.type != NPCID.TheDestroyer)
                        {
                            npc.dontTakeDamage = false;
                            if (npc.ai[3] > 0f)
                                npc.netUpdate = true;

                            npc.ai[3] = 0f;
                        }
                        else
                        {
                            Vector2 spinningpoint = nPC2.Center + tileConvertedPosition;
                            spinningpoint = spinningpoint.RotatedBy(nPC2.rotation, nPC2.Center);
                            npc.Center = spinningpoint;
                            npc.velocity = nPC.velocity;
                            npc.dontTakeDamage = true;
                        }
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        if (npc.ai[3] > 0f)
                            npc.netUpdate = true;

                        npc.ai[3] = 0f;
                    }
                }
                else
                {
                    npc.dontTakeDamage = false;
                    if (npc.ai[3] > 0f)
                        npc.netUpdate = true;

                    npc.ai[3] = 0f;
                }
            }
            else
            {
                npc.dontTakeDamage = false;

                if (npc.velocity.X < probeTargetX)
                    npc.velocity.X += acceleration;
                else if (npc.velocity.X > probeTargetX)
                    npc.velocity.X -= acceleration;

                if (npc.velocity.Y < probeTargetY)
                    npc.velocity.Y += acceleration;
                else if (npc.velocity.Y > probeTargetY)
                    npc.velocity.Y -= acceleration;
            }

            npc.localAI[0] += 1f;
            if (npc.justHit)
                npc.localAI[0] = 0f;

            float laserGateValue = NPC.IsMechQueenUp ? 360f : bossRush ? 150f : 240f;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= laserGateValue)
            {
                npc.localAI[0] = 0f;
                if (targetData.Type != 0 && Collision.CanHit(npc.position, npc.width, npc.height, targetData.Position, targetData.Width, targetData.Height))
                {
                    int type = ProjectileID.PinkLaser;
                    int damage = npc.GetProjectileDamage(type);
                    int totalProjectiles = (CalamityWorld.death || bossRush) ? 3 : 1;
                    Vector2 npcCenter = new Vector2(probeTargetX, probeTargetY);
                    if (NPC.IsMechQueenUp)
                    {
                        Vector2 v = targetData.Center - npc.Center - targetData.Velocity * 20f;
                        float projectileVelocity = 8f;
                        npcCenter = v.SafeNormalize(Vector2.UnitY) * projectileVelocity;
                    }
                    for (int i = 0; i < totalProjectiles; i++)
                    {
                        float velocityMultiplier = 1f;
                        switch (i)
                        {
                            case 0:
                                break;
                            case 1:
                                velocityMultiplier = 0.9f;
                                break;
                            case 2:
                                velocityMultiplier = 0.8f;
                                break;
                        }
                        Projectile.NewProjectile(npc.GetSource_FromAI(), probeCenter, npcCenter * velocityMultiplier, type, damage, 0f, Main.myPlayer);
                    }

                    npc.netUpdate = true;
                }
            }

            if (probeTargetX > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(probeTargetY, probeTargetX);
            }
            if (probeTargetX < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(probeTargetY, probeTargetX) + MathHelper.Pi;
            }

            float tilePosX = -0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * tilePosX;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    npc.velocity.X = 2f;
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    npc.velocity.X = -2f;
            }

            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * tilePosX;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5)
                    npc.velocity.Y = 2f;
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5)
                    npc.velocity.Y = -2f;
            }

            if (farAwayFromTarget)
            {
                if ((npc.velocity.X > 0f && probeTargetX > 0f) || (npc.velocity.X < 0f && probeTargetX < 0f))
                {
                    if (Math.Abs(npc.velocity.X) < (NPC.IsMechQueenUp ? 5f : 12f))
                        npc.velocity.X *= 1.05f;
                }
                else
                    npc.velocity.X *= 0.9f;
            }

            if (NPC.IsMechQueenUp && npc.ai[2] == 0f)
            {
                Vector2 center = npc.GetTargetData().Center;
                Vector2 v2 = center - npc.Center;
                if (v2.Length() < 120f)
                    npc.Center = center - v2.SafeNormalize(Vector2.UnitY) * 120;
            }

            if (targetDead)
            {
                npc.velocity.Y -= acceleration * 2f;
                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }

            return false;
        }
    }
}
