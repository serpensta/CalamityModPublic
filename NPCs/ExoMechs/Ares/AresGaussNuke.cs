using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Ares
{
	//[AutoloadBossHead]
	public class AresGaussNuke : ModNPC
    {
		public enum Phase
		{
			Nothing = 0,
			GaussNuke = 1,
			Reload = 2
		}

		public float AIState
		{
			get => npc.Calamity().newAI[0];
			set => npc.Calamity().newAI[0] = value;
		}

		// Number of frames on the X and Y axis
		private const int maxFramesX = 9;
		private const int maxFramesY = 12;

		// Counters for frames on the X and Y axis
		private int frameX = 0;
		private int frameY = 0;

		// The exact frame the animation is currently on
		private int exactFrame = 0;

		// Frame limit per animation, these are the specific frames where each animation ends
		private const int normalFrameLimit = 11;
		private const int firstStageGaussNukeChargeFrameLimit = 23;
		private const int secondStageGaussNukeChargeFrameLimit = 35;
		private const int finalStageGaussNukeChargeFrameLimit = 47;
		private const int reloadFrameLimit = 107;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Total duration of the gauss nuke telegraph
		private const float gaussNukeTelegraphDuration = 360f;

		// Total duration of the gauss nuke firing phase
		private const float gaussNukeReloadDuration = 600f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XF-09 Ares Gauss Nuke");
		}

        public override void SetDefaults()
        {
			npc.npcSlots = 5f;
			npc.damage = 100;
			npc.width = 170;
            npc.height = 120;
            npc.defense = 80;
			npc.DR_NERD(0.25f);
			npc.LifeMaxNERB(1000000, 1150000, 500000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
			npc.Opacity = 0f;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
			npc.boss = true;
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.chaseable);
            writer.Write(npc.dontTakeDamage);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.chaseable = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			if (CalamityGlobalNPC.draedonExoMechPrime < 0 || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
				npc.active = false;
				return;
			}

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Percent life remaining
			float lifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoWormAlive = false;
			bool exoSpazAlive = false;
			bool exoRetAlive = false;
			if (CalamityGlobalNPC.draedonExoMechWorm != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
				{
					otherExoMechsAlive++;
					exoWormAlive = true;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
				{
					otherExoMechsAlive++;
					exoSpazAlive = true;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechTwinRed != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].active)
				{
					otherExoMechsAlive++;
					exoRetAlive = true;
				}
			}

			// These are 5 by default to avoid triggering passive phases after the other mechs are dead
			float exoWormLifeRatio = defaultLifeRatio;
			float exoSpazLifeRatio = defaultLifeRatio;
			float exoRetLifeRatio = defaultLifeRatio;
			if (exoWormAlive)
				exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
			if (exoSpazAlive)
				exoSpazLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;
			if (exoRetAlive)
				exoRetLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].lifeMax;
			float totalOtherExoMechLifeRatio = exoWormLifeRatio + exoSpazLifeRatio + exoRetLifeRatio;

			// Phases
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);

			// Target variable
			Player player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];

			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			if (npc.life > Main.npc[(int)npc.ai[1]].life)
				npc.life = Main.npc[(int)npc.ai[1]].life;

			// Despawn if target is dead
			bool targetDead = false;
			if (player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (player.dead)
				{
					targetDead = true;

					AIState = (float)Phase.Nothing;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;

					npc.velocity.Y -= 2f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 2f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<AresBody>() || Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() || Main.npc[a].type == ModContent.NPCType<AresLaserCannon>())
								Main.npc[a].active = false;
						}
					}
				}
			}

			CalamityGlobalNPC calamityGlobalNPC_Body = Main.npc[(int)npc.ai[2]].Calamity();

			// Adjust opacity
			bool invisiblePhase = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune;
			npc.dontTakeDamage = invisiblePhase;
			if (!invisiblePhase)
			{
				npc.Opacity += 0.2f;
				if (npc.Opacity > 1f)
					npc.Opacity = 1f;
			}
			else
			{
				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
			}

			// Predictiveness
			float predictionAmt = malice ? 30f : death ? 25f : revenge ? 20f : expertMode ? 15f : 5f;
			Vector2 predictionVector = player.velocity * predictionAmt;
			Vector2 rotationVector = player.Center + predictionVector - npc.Center;

			float rateOfRotation = AIState == (int)Phase.GaussNuke ? 0.08f : 0.04f;
			Vector2 lookAt = player.Center + Vector2.Normalize(rotationVector);

			float rotation = (float)Math.Atan2(lookAt.Y - npc.Center.Y, lookAt.X - npc.Center.X);
			if (npc.spriteDirection == 1)
				rotation += MathHelper.Pi;
			if (rotation < 0f)
				rotation += MathHelper.TwoPi;
			if (rotation > MathHelper.TwoPi)
				rotation -= MathHelper.TwoPi;

			if (npc.rotation < rotation)
			{
				if (rotation - npc.rotation > MathHelper.Pi)
					npc.rotation -= rateOfRotation;
				else
					npc.rotation += rateOfRotation;
			}
			if (npc.rotation > rotation)
			{
				if (npc.rotation - rotation > MathHelper.Pi)
					npc.rotation += rateOfRotation;
				else
					npc.rotation -= rateOfRotation;
			}

			if (npc.rotation > rotation - rateOfRotation && npc.rotation < rotation + rateOfRotation)
				npc.rotation = rotation;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > rotation - rateOfRotation && npc.rotation < rotation + rateOfRotation)
				npc.rotation = rotation;

			// Light
			Lighting.AddLight(npc.Center, 0.2f, 0.25f, 0.05f);

			// Default vector to fly to
			Vector2 destination = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays ? new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + 540f, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + 540f) : new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + 750f, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y);

			// Velocity and acceleration values
			float baseVelocityMult = malice ? 1.3f : death ? 1.2f : revenge ? 1.15f : expertMode ? 1.1f : 1f;
			float baseVelocity = 15f * baseVelocityMult;
			float baseAcceleration = 1f;
			if (berserk)
			{
				baseVelocity *= 1.5f;
				baseAcceleration *= 1.5f;
			}
			Vector2 desiredVelocity = Vector2.Normalize(destination - npc.Center) * baseVelocity;

			// Distance from target
			float distanceFromTarget = Vector2.Distance(npc.Center, player.Center);

			// Gate values
			float gaussNukePhaseGateValue = 600f;

			// Variable to disable deathray firing
			bool doNotFire = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays || calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune;
			if (doNotFire)
			{
				AIState = (float)Phase.Nothing;
				calamityGlobalNPC.newAI[1] = 0f;
				calamityGlobalNPC.newAI[2] = 0f;
			}

			// Attacking phases
			switch ((int)AIState)
			{
				// Do nothing and fly in place
				case (int)Phase.Nothing:

					calamityGlobalNPC.newAI[1] += 1f;
					if (calamityGlobalNPC.newAI[1] >= gaussNukePhaseGateValue)
					{
						AIState = (float)Phase.GaussNuke;
						calamityGlobalNPC.newAI[1] = 0f;
					}

					break;

				// Fire gauss nuke that emits a wave pounder stealth strike-size explosion on death
				case (int)Phase.GaussNuke:

					if (!targetDead)
					{
						calamityGlobalNPC.newAI[2] += 1f;
						if (calamityGlobalNPC.newAI[2] < gaussNukeTelegraphDuration)
						{
							if (calamityGlobalNPC.newAI[2] == 1f)
							{
								// Set frames to gauss nuke charge up frames
								npc.frameCounter = 0D;
								frameX = 1;
								frameY = 0;
								exactFrame = 12;
							}

							// Fire gauss nuke on frame 41
							if (exactFrame == 41 && calamityGlobalNPC.newAI[1] == 0f)
							{
								calamityGlobalNPC.newAI[1] = 1f;

								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), npc.Center);
									float projectileVelocity = 12f;
									Vector2 gaussNukeVelocity = Vector2.Normalize(rotationVector) * projectileVelocity;
									int type = ModContent.ProjectileType<AresGaussNukeProjectile>();
									int damage = npc.GetProjectileDamage(type);
									float offset = 40f;
									Projectile.NewProjectile(npc.Center + Vector2.Normalize(gaussNukeVelocity) * offset, gaussNukeVelocity, type, damage, 0f, Main.myPlayer, 0f, 0f);

									// Recoil
									npc.velocity -= gaussNukeVelocity;
								}
							}
						}
						else
						{
							AIState = (float)Phase.Reload;
							calamityGlobalNPC.newAI[1] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;

							// Set frames to gauss nuke reload frames
							npc.frameCounter = 0D;
							frameX = 4;
							frameY = 0;
							exactFrame = 48;
						}
					}

					break;

				case (int)Phase.Reload:

					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= gaussNukeReloadDuration)
					{
						AIState = (float)Phase.Nothing;
						calamityGlobalNPC.newAI[2] = 0f;
						npc.TargetClosest();
					}

					break;
			}

			// Movement
			if (!targetDead)
				npc.SimpleFlyMovement(desiredVelocity, baseAcceleration);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

		public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames when using gauss nuke
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.Nothing)
			{
				if (npc.frameCounter >= 10D)
				{
					npc.frameCounter = 0D;
					frameY++;
					exactFrame++;
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}
					if (exactFrame > normalFrameLimit)
						frameX = frameY = exactFrame = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 10D)
				{
					npc.frameCounter = 0D;
					frameY++;
					exactFrame++;
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}
					if (exactFrame > reloadFrameLimit)
						frameX = frameY = exactFrame = 0;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 vector = new Vector2(npc.width / 2, npc.height / 2);
			Vector2 center = npc.Center - Main.screenPosition;
			spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Ares/AresGaussNukeGlow");
			spriteBatch.Draw(texture, center, frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

			return false;
		}

		public override bool PreNPCLoot() => false;

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
				Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1f);

			if (npc.life <= 0)
			{
				for (int num193 = 0; num193 < 2; num193++)
				{
					Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
				}
				for (int num194 = 0; num194 < 20; num194++)
				{
					int num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
					Main.dust[num195].noGravity = true;
					Main.dust[num195].velocity *= 3f;
					num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
					Main.dust[num195].velocity *= 2f;
					Main.dust[num195].noGravity = true;
				}

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresGaussNuke1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresGaussNuke2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresGaussNuke3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresHandBase1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresHandBase2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresHandBase3"), 1f);
			}
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
    }
}
