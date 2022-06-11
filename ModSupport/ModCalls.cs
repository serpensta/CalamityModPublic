﻿using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Particles;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod
{
    public class ModCalls
    {
        #region Boss / Event Downed
        /// <summary>
        /// Returns whether the Calamity boss or event corresponding to the given string has been defeated.
        /// </summary>
        /// <param name="boss">The boss or event name to check. Many aliases are accepted.</param>
        /// <returns>Whether the boss or event has been defeated.</returns>
        public static bool GetBossDowned(string boss)
        {
            switch (boss.ToLower())
            {
                default:
                    return false;

                // Because this value will always be true if Acid Rain at any point in the game is beaten.
                case "acid rain":
                case "acidrain":

                case "acid rain 1":
                case "acidrain 1":
                case "acidrain1":
                case "acid rain eoc":
                case "acidrain eoc":
                case "acidraineoc":
                    return DownedBossSystem.downedEoCAcidRain;

                case "desertscourge":
                case "desert scourge":
                    return DownedBossSystem.downedDesertScourge;

                case "clam":
                case "giantclam":
                case "giant clam":
                    return DownedBossSystem.downedCLAM;

                case "crabulon":
                    return DownedBossSystem.downedCrabulon;

                case "hivemind":
                case "hive mind":
                case "thehivemind":
                case "the hive mind":
                    return DownedBossSystem.downedHiveMind;

                case "perforator":
                case "perforators":
                case "theperforators":
                case "the perforators":
                case "perforatorhive":
                case "perforator hive":
                case "theperforatorhive":
                case "the perforator hive":
                    return DownedBossSystem.downedPerforator;

                case "slimegod":
                case "slime god":
                case "theslimegod":
                case "the slime god":
                    return DownedBossSystem.downedSlimeGod;

                case "hmclam":
                case "clamhm":
                case "hm clam":
                case "clam hm":
                case "hmgiantclam":
                case "giantclamhm":
                case "hm giant clam":
                case "giant clam hm":
                case "hardmodeclam":
                case "hardmode clam":
                case "hardmodegiantclam":
                case "hardmode giant clam":
                    return DownedBossSystem.downedCLAMHardMode;

                case "cryogen":
                    return DownedBossSystem.downedCryogen;

                case "acid rain 2":
                case "acidrain 2":
                case "acidrain2":
                case "acid rain scourge":
                case "acid rain aquatic scourge":
                case "acid rain aquaticscourge":
                case "acidrain scourge":
                case "acidrain aquatic scourge":
                case "acidrain aquaticscourge":
                case "acidrainscourge":
                case "acidrainaquaticscourge":
                    return DownedBossSystem.downedAquaticScourgeAcidRain;

                case "aquaticscourge":
                case "aquatic scourge":
                    return DownedBossSystem.downedAquaticScourge;

                case "brimstoneelemental":
                case "brimstone elemental":
                    return DownedBossSystem.downedBrimstoneElemental;

                case "calamitas":
                case "clone":
                case "calamitasclone":
                case "calamitas clone":
                case "clonelamitas":
                case "calamitasdoppelganger":
                case "calamitas doppelganger":
                    return DownedBossSystem.downedCalamitas;

                case "gss":
                case "greatsandshark":
                case "great sand shark":
                    return DownedBossSystem.downedGSS;

                // Don't remove the old references to "Siren" here to avoid breaking other mods
                case "sirenleviathan":
                case "siren leviathan":
                case "sirenandleviathan":
                case "siren and leviathan":
                case "the siren and the leviathan":
                case "siren":
                case "thesiren":
                case "the siren":
                case "anahita":
                case "leviathan":
                case "theleviathan":
                case "the leviathan":
                case "anahitaleviathan":
                case "anahita leviathan":
                case "anahitaandleviathan":
                case "anahita and leviathan":
                case "anahita and the leviathan":
                    return DownedBossSystem.downedLeviathan;

                case "aureus":
                case "astrumaureus":
                case "astrum aureus":
                    return DownedBossSystem.downedAstrumAureus;

                case "pbg":
                case "plaguebringer":
                case "plaguebringergoliath":
                case "plaguebringer goliath":
                case "theplaguebringergoliath":
                case "the plaguebringer goliath":
                    return DownedBossSystem.downedPlaguebringer;

                case "scavenger": // backwards compatibility
                case "ravager":
                    return DownedBossSystem.downedRavager;

                case "stargod": // backwards compatibility
                case "star god": // backwards compatibility
                case "astrumdeus":
                case "astrum deus":
                    return DownedBossSystem.downedAstrumDeus;

                case "guardians":
                case "donuts":
                case "profanedguardians":
                case "profaned guardians":
                case "theprofanedguardians":
                case "the profaned guardians":
                    return DownedBossSystem.downedGuardians;

                case "dragonfolly":
                case "the dragonfolly":
                case "bumblebirb":
                case "bumblefuck":
                    return DownedBossSystem.downedDragonfolly;

                case "providence":
                case "providencetheprofanedgoddess":
                case "providence the profaned goddess":
                case "providence, the profaned goddess":
                    return DownedBossSystem.downedProvidence;

                case "polterghast":
                case "necroghast":
                case "necroplasm":
                    return DownedBossSystem.downedPolterghast;

                // Old Duke is also Acid Rain tier 3, so he gets those names too
                case "oldduke":
                case "old duke":
                case "theoldduke":
                case "the old duke":
                case "boomerduke":
                case "boomer duke":
                case "sulphurduke":
                case "sulphur duke":
                case "sulfurduke":
                case "sulfur duke":
                case "acid rain 3":
                case "acidrain 3":
                case "acidrain3":
                case "acid rain duke":
                case "acidrain duke":
                case "acidrainduke":
                    return DownedBossSystem.downedBoomerDuke;

                case "sentinel1": // backwards compatibility
                case "void":
                case "ceaselessvoid":
                case "ceaseless void":
                    return DownedBossSystem.downedCeaselessVoid;

                case "sentinel2": // backwards compatibility
                case "stormweaver":
                case "storm weaver":
                    return DownedBossSystem.downedStormWeaver;

                case "sentinel3": // backwards compatibility
                case "cosmicwraith":
                case "cosmic wraith":
                case "signus":
                case "signusenvoyofthedevourer":
                case "signus envoy of the devourer":
                case "signus, envoy of the devourer":
                    return DownedBossSystem.downedSignus;

                case "sentinelany": // backwards compatibility
                case "anysentinel":
                case "any sentinel":
                case "onesentinel":
                case "one sentinel":
                case "sentinel":
                    return DownedBossSystem.downedCeaselessVoid || DownedBossSystem.downedStormWeaver || DownedBossSystem.downedSignus;

                case "sentinelall": // backwards compatibility
                case "sentinels":
                case "allsentinel":
                case "allsentinels":
                case "all sentinels":
                    return DownedBossSystem.downedCeaselessVoid && DownedBossSystem.downedStormWeaver && DownedBossSystem.downedSignus;

                case "dog":
                case "devourerofgods":
                case "devourer of gods":
                case "thedevourerofgods":
                case "the devourer of gods":
                    return DownedBossSystem.downedDoG;

                case "yharon":
                case "jungledragonyharon":
                case "jungle dragon yharon":
                case "jungle dragon, yharon":
                    return DownedBossSystem.downedYharon;

                case "draedon":
                case "exomechs":
                case "exo mechs":
                case "the exo mechs":
                case "apollo":
                case "artemis":
                case "thanatos":
                case "ares":
                    return DownedBossSystem.downedExoMechs;

                case "scal":
                case "supremecalamitas":
                case "supreme calamitas":
                    return DownedBossSystem.downedSCal;

                case "adulteidolonwyrm":
                case "adult eidolon wyrm":
                case "adultwyrm":
                case "adult wyrm":
                case "adulteidolon":
                case "adult eidolon":
                    return DownedBossSystem.downedAdultEidolonWyrm;
            }
        }
        #endregion

        #region Player in Zone / Area
        /// <summary>
        /// Returns whether the specified player is in the Calamity biome or area corresponding to the given string.
        /// </summary>
        /// <param name="p">The player whose locale is being questioned.</param>
        /// <param name="zone">The zone or area name to check. Many aliases are accepted.</param>
        /// <returns>Whether the player is currently in the zone.</returns>
        public static bool GetInZone(Player p, string zone)
        {
            CalamityPlayer mp = p.Calamity();
            switch (zone.ToLower())
            {
                default:
                    return false;

                case "calamity": // backwards compatibility
                case "calamitybiome":
                case "calamity biome":
                case "crag":
                case "crags":

                case "profanedcrag": // remove these four when the actual profaned biome is added
                case "profaned crag":
                case "profanedcrags":
                case "profaned crags":

                case "brimstone":
                case "brimstonecrag":
                case "brimstone crag":
                case "brimstonecrags":
                case "brimstone crags":
                    return mp.ZoneCalamity;

                case "astral":
                case "astralbiome":
                case "astral biome":
                case "astralinfection":
                case "astral infection":
                    return mp.ZoneAstral;

                case "sunkensea":
                case "sunken sea":
                case "thesunkensea":
                case "the sunken sea":
                    return mp.ZoneSunkenSea;

                case "sulfur":
                case "sulphur":
                case "sulfursea":
                case "sulfur sea":
                case "sulphursea":
                case "sulphur sea":
                case "sulfuroussea":
                case "sulfurous sea":
                case "sulphuroussea":
                case "sulphurous sea":
                    return mp.ZoneSulphur;

                case "abyss":
                case "theabyss":
                case "the abyss":
                case "anyabyss":
                case "any abyss":
                case "abyssany":
                case "any abyss layer":
                    return mp.ZoneAbyss;

                case "abyss1":
                case "abyss 1":
                case "abyss_1":
                case "layer1":
                case "layer 1":
                case "layer_1":
                case "abysslayer1":
                case "abyss layer 1":
                    return mp.ZoneAbyssLayer1;

                case "abyss2":
                case "abyss 2":
                case "abyss_2":
                case "layer2":
                case "layer 2":
                case "layer_2":
                case "abysslayer2":
                case "abyss layer 2":
                    return mp.ZoneAbyssLayer2;

                case "abyss3":
                case "abyss 3":
                case "abyss_3":
                case "layer3":
                case "layer 3":
                case "layer_3":
                case "abysslayer3":
                case "abyss layer 3":
                    return mp.ZoneAbyssLayer3;

                case "abyss4":
                case "abyss 4":
                case "abyss_4":
                case "layer4":
                case "layer 4":
                case "layer_4":
                case "abysslayer4":
                case "abyss layer 4":
                    return mp.ZoneAbyssLayer4;
            }
        }
        #endregion

        #region Difficulty Modes
        /// <summary>
        /// Returns whether the Calamity difficulty modifier corresponding to the given string is currently active.
        /// </summary>
        /// <param name="difficulty">The difficulty modifier to check for.</param>
        /// <returns>Whether the difficulty is currently active.</returns>
        public static bool GetDifficultyActive(string difficulty)
        {
            switch (difficulty.ToLower())
            {
                default:
                    return false;

                case "revengeance":
                case "rev":
                case "revengeancemode":
                case "revengeance mode":
                    return CalamityWorld.revenge;

                case "death":
                case "deathmode":
                case "death mode":
                    return CalamityWorld.death;

                case "malice":
                case "malicemode":
                case "malice mode":
                    return CalamityWorld.malice;

                case "br":
                case "bossrush":
                case "boss rush":
                case "bossrushactive":
                case "boss rush active":
                    return BossRushEvent.BossRushActive;

                case "armageddon":
                case "arma":
                case "instakill":
                case "instagib":
                case "armageddonmode":
                case "armageddon mode":
                    return CalamityWorld.armageddon;
            }
        }

        /// <summary>
        /// Either enables or disables the Calamity difficulty modifier corresponding to the given string.<br></br>
        /// Unlike the in-game mode changing items, this has no ancillary effects such as failing if a boss is alive or instantly killing players.
        /// </summary>
        /// <param name="difficulty">The difficulty modifier to edit.</param>
        /// <param name="enabled">Whether to enable or disable the difficulty.</param>
        /// <returns></returns>
        public static bool SetDifficultyActive(string difficulty, bool enabled)
        {
            switch (difficulty.ToLower())
            {
                default:
                    return false;

                case "revengeance":
                case "rev":
                case "revengeancemode":
                case "revengeance mode":
                    return CalamityWorld.revenge = enabled;

                case "death":
                case "deathmode":
                case "death mode":
                    return CalamityWorld.death = enabled;

                case "malice":
                case "malicemode":
                case "malice mode":
                    return CalamityWorld.malice = enabled;

                case "br":
                case "bossrush":
                case "boss rush":
                case "bossrushactive":
                case "boss rush active":
                    return BossRushEvent.BossRushActive = enabled;

                case "armageddon":
                case "arma":
                case "instakill":
                case "instagib":
                case "armageddonmode":
                case "armageddon mode":
                    return CalamityWorld.armageddon = enabled;
            }
        }
        #endregion

        #region Rogue Class
        /// <summary>
        /// Gets a player's current rogue projectile velocity multiplier.
        /// </summary>
        /// <param name="p">The player whose rogue velocity is being queried.</param>
        /// <returns>Current rogue projectile velocity multiplier. 1f is no bonus, 2f doubles projectile speed.</returns>
        public static float GetRogueVelocity(Player p) => p?.Calamity()?.rogueVelocity ?? 1f;

        /// <summary>
        /// Adds a flat amount of rogue velocity stat to a player. This amount can be negative.
        /// </summary>
        /// <param name="p">The player whose rogue velocity is being modified.</param>
        /// <param name="add">The amount of rogue velocity to add or subtract (if negative).</param>
        /// <returns>The player's new rogue velocity stat.</returns>
        public static float AddRogueVelocity(Player p, float add) => p is null ? 1f : (p.Calamity().rogueVelocity += add);

        public static float GetCurrentStealth(Player p) => p?.Calamity()?.rogueStealth ?? 0f;

        public static float GetMaxStealth(Player p) => p?.Calamity()?.rogueStealthMax ?? 0f;

        public static float AddMaxStealth(Player p, float add) => p is null ? 0f : (p.Calamity().rogueStealthMax += add);
        #endregion

        #region Player Armor Set Bonuses
        /// <summary>
        /// Returns whether the specified player has the set bonus corresponding to the given string.
        /// </summary>
        /// <param name="p">The player whose set bonuses are being questioned.</param>
        /// <param name="setBonus">The set bonus to check for.</param>
        /// <returns>Whether the player currently has the set bonus.</returns>
        public static bool GetSetBonus(Player p, string setBonus)
        {
            CalamityPlayer mp = p.Calamity();

            setBonus = setBonus.ToLower();

            // LATER -- no summon set bonuses are written well. all use two bools, neither of which actually controls the function

            // Desert Prowler
            if (setBonus == "desertprowler" || setBonus == "desert prowler")
                return mp.desertProwler;

            // Snow Ruffian
            if (setBonus == "snowruffian" || setBonus == "snow ruffian")
                return mp.snowRuffianSet;

            // Sulphurous
            if (setBonus == "sulfur" || setBonus == "sulphur" || setBonus == "sulfurous" || setBonus == "sulphurous")
                return mp.sulfurSet;

            // Victide
            if (setBonus == "victide_summon" || setBonus == "victide summon")
                return mp.victideSummoner; // the bool set directly by VictideHelmet.UpdateArmorSet
            else if (setBonus == "victide" || setBonus.StartsWith("victide_") || setBonus.StartsWith("victide "))
                return mp.victideSet;

            // Aerospec
            if (setBonus == "aerospec_summon" || setBonus == "aerospec summon")
                return mp.valkyrie; // the bool set directly by AerospecHelmet.UpdateArmorSet
            else if (setBonus == "aerospec" || setBonus.StartsWith("aerospec_") || setBonus.StartsWith("aerospec "))
                return mp.aeroSet;

            // Statigel
            if (setBonus == "statigel_summon" || setBonus == "statigel summon")
                return mp.slimeGod; // the bool set directly by StatigelHood.UpdateArmorSet
            if (setBonus == "statigel" || setBonus.StartsWith("statigel_") || setBonus.StartsWith("statigel "))
                return mp.statigelSet;

            // Mollusk
            if (setBonus == "mollusk")
                return mp.molluskSet;

            // Titan Heart
            if (setBonus == "titanheart" || setBonus == "titan heart")
                return mp.titanHeartSet;

            // Forbidden Circlet
            if (setBonus == "forbidden_circlet" || setBonus == "forbidden circlet")
                return mp.forbiddenCirclet;

            // Daedalus
            switch (setBonus)
            {
                default:
                    break;
                case "daedalus":
                    return mp.daedalusReflect || mp.daedalusShard || mp.daedalusAbsorb || mp.daedalusCrystal || mp.daedalusSplit;
                case "daedalus_melee":
                case "daedalus melee":
                    return mp.daedalusReflect;
                case "daedalus_ranged":
                case "daedalus ranged":
                    return mp.daedalusShard;
                case "daedalus_magic":
                case "daedalus magic":
                    return mp.daedalusAbsorb;
                case "daedalus_summon":
                case "daedalus summon":
                    return mp.daedalusCrystal;
                case "daedalus_rogue":
                case "daedalus rogue":
                    return mp.daedalusSplit;
            }

            // Reaver
            switch (setBonus)
            {
                default:
                    break;
                case "reaver":
                    return mp.reaverSpeed || mp.reaverExplore || mp.reaverDefense;
                case "reaver_speed":
                case "reaver speed":
                    return mp.reaverSpeed;
                case "reaver_explore":
                case "reaver explore":
                case "reaver_exploration":
                case "reaver exploration":
                    return mp.reaverExplore;
                case "reaver_defense":
                case "reaver defense":
                case "reaver_tank":
                case "reaver tank":
                    return mp.reaverDefense;
            }

            // Fathom Swarmer
            if (setBonus == "fathomswarmer" || setBonus == "fathom swarmer")
                return mp.fathomSwarmer;

            // Brimflame
            if (setBonus == "brimflame")
                return mp.brimflameSet;

            // Umbraphile
            if (setBonus == "umbraphile")
                return mp.umbraphileSet;

            // Hydrothermic (Ataxia is legacy name)
            switch (setBonus)
            {
                default:
                    break;
                case "ataxia":
                case "hydrothermic":
                case "hydrothermal":
                    return mp.ataxiaBlaze;
                case "ataxia_melee":
                case "ataxia melee":
                case "hydrothermic_melee":
                case "hydrothermic melee":
                case "hydrothermal_melee":
                case "hydrothermal melee":
                    return mp.ataxiaGeyser;
                case "ataxia_ranged":
                case "ataxia ranged":
                case "hydrothermic_ranged":
                case "hydrothermic ranged":
                case "hydrothermal_ranged":
                case "hydrothermal ranged":
                    return mp.ataxiaBolt;
                case "ataxia_magic":
                case "ataxia magic":
                case "hydrothermic_magic":
                case "hydrothermic magic":
                case "hydrothermal_magic":
                case "hydrothermal magic":
                    return mp.ataxiaMage;
                case "ataxia_summon":
                case "ataxia summon":
                case "hydrothermic_summon":
                case "hydrothermic summon":
                case "hydrothermal_summon":
                case "hydrothermal summon":
                    return mp.chaosSpirit;
                case "ataxia_rogue":
                case "ataxia rogue":
                case "hydrothermic_rogue":
                case "hydrothermic rogue":
                case "hydrothermal_rogue":
                case "hydrothermal rogue":
                    return mp.ataxiaVolley;
            }

            // Plague Reaper
            if (setBonus == "plaguereaper" || setBonus == "plague reaper")
                return mp.plagueReaper;

            // Plaguebringer
            if (setBonus == "plaguebringer" || setBonus == "plaguebringerpatron" || setBonus == "plaguebringer patron")
                return mp.plaguebringerPatronSet;

            // Astral
            if (setBonus == "astral")
                return mp.astralStarRain;

            // Empyrean (formerly Xeroc)
            if (setBonus == "empyrean" || setBonus == "xeroc")
                return mp.xerocSet;

            // Tarragon
            switch (setBonus)
            {
                default:
                    break;
                case "tarragon":
                    return mp.tarraSet;
                case "tarragon_melee":
                case "tarragon melee":
                    return mp.tarraMelee;
                case "tarragon_ranged":
                case "tarragon ranged":
                    return mp.tarraRanged;
                case "tarragon_magic":
                case "tarragon magic":
                    return mp.tarraMage;
                case "tarragon_summon":
                case "tarragon summon":
                    return mp.tarraSummon;
                case "tarragon_rogue":
                case "tarragon rogue":
                    return mp.tarraThrowing;
            }

            // Prismatic
            if (setBonus == "prismatic" || setBonus == "prism")
                return mp.prismaticSet;

            // Bloodflare
            switch (setBonus)
            {
                default:
                    break;
                case "bloodflare":
                    return mp.bloodflareSet;
                case "bloodflare_melee":
                case "bloodflare melee":
                    return mp.bloodflareMelee;
                case "bloodflare_ranged":
                case "bloodflare ranged":
                    return mp.bloodflareRanged;
                case "bloodflare_magic":
                case "bloodflare magic":
                    return mp.bloodflareMage;
                case "bloodflare_summon":
                case "bloodflare summon":
                    return mp.bloodflareSummon;
                case "bloodflare_rogue":
                case "bloodflare rogue":
                    return mp.bloodflareThrowing;
            }

            // Omega Blue
            if (setBonus == "omegablue" || setBonus == "omega blue")
                return mp.omegaBlueSet;

            // God Slayer
            switch (setBonus)
            {
                default:
                    break;
                case "godslayer":
                case "god slayer":
                    return mp.godSlayer;
                case "godslayer_melee":
                case "godslayer melee":
                case "god slayer melee":
                    return mp.godSlayerDamage; // melee helm's unique damage reducing property
                case "godslayer_ranged":
                case "godslayer ranged":
                case "god slayer ranged":
                    return mp.godSlayerRanged;
                // God Slayer Mage was removed in the Draedon Update
                case "godslayer_magic":
                case "godslayer magic":
                case "god slayer magic":
                    return false; // mp.godSlayerMage;
                // God Slayer Summon was removed in the Draedon Update
                case "godslayer_summon":
                case "godslayer summon":
                case "god slayer summon":
                    return false; // mp.godSlayerSummon;

                case "godslayer_rogue":
                case "godslayer rogue":
                case "god slayer rogue":
                    return mp.godSlayerThrowing;
            }

            // Fearmonger
            if (setBonus == "fearmonger")
                return mp.fearmongerSet;

            // Silva
            switch (setBonus)
            {
                default:
                    break;
                case "silva":
                    return mp.silvaSet;

                // Silva Melee was removed in the Draedon Update
                case "silva_melee":
                case "silva melee":
                    return false; // mp.silvaMelee;
                // Silva Ranged was removed in the Draedon Update
                case "silva_ranged":
                case "silva ranged":
                    return false; // mp.silvaRanged;
                case "silva_magic":
                case "silva magic":
                    return mp.silvaMage;
                case "silva_summon":
                case "silva summon":
                    return mp.silvaSummon;
                // Silva Rogue was removed in the Draedon Update
                case "silva_rogue":
                case "silva rogue":
                    return false; // mp.silvaThrowing;
            }

            // Auric Tesla
            if (setBonus == "auric" || setBonus == "aurictesla" || setBonus == "auric tesla")
                return mp.auricSet;

            // Demonshade
            if (setBonus == "demonshade")
                return mp.dsSetBonus;

            return false;
        }

        /// <summary>
        /// Turns the set bonus corresponding to the given string on or off for the specified player.
        /// </summary>
        /// <param name="p">The player whose set bonuses are being toggled.</param>
        /// <param name="setBonus">The set bonus to check for.</param>
        /// <param name="enabled">Whether the set bonus should be enabled (true) or disabled (false).</param>
        /// <returns>Whether any set bonus was adjusted.</returns>
        public static bool SetSetBonus(Player p, string setBonus, bool enabled)
        {
            CalamityPlayer mp = p.Calamity();
            setBonus = setBonus.ToLower();

            // Desert Prowler
            if (setBonus == "desertprowler" || setBonus == "desert prowler")
            {
                mp.desertProwler = enabled;
                return true;
            }

            // Snow Ruffian
            if (setBonus == "snowruffian" || setBonus == "snow ruffian")
            {
                mp.snowRuffianSet = enabled;
                return true;
            }

            // Sulphurous
            if (setBonus == "sulfur" || setBonus == "sulphur" || setBonus == "sulfurous" || setBonus == "sulphurous")
            {
                mp.sulfurSet = enabled;
                mp.sulfurJump = enabled;
                return true;
            }

            // Victide
            if (setBonus == "victide_summon" || setBonus == "victide summon")
            {
                mp.victideSet = enabled;
                mp.victideSummoner = enabled; 
                return true;
            }
            else if (setBonus == "victide" || setBonus.StartsWith("victide_") || setBonus.StartsWith("victide "))
            {
                mp.victideSet = true;
                return true;
            }

            // Aerospec
            if (setBonus == "aerospec_summon" || setBonus == "aerospec summon")
            {
                mp.aeroSet = enabled;
                mp.valkyrie = enabled; // LATER -- remove this when player.valkyrie actually controls aerospec summoner
                return true;
            }
            else if (setBonus == "aerospec" || setBonus.StartsWith("aerospec_") || setBonus.StartsWith("aerospec "))
            {
                mp.aeroSet = enabled;
                return true;
            }

            // Statigel
            if (setBonus == "statigel_summon" || setBonus == "statigel summon")
            {
                mp.statigelSet = enabled;
                mp.slimeGod = enabled; // LATER -- remove this when player.slimeGod actually controls statigel summoner
                return true;
            }
            else if (setBonus == "statigel" || setBonus.StartsWith("statigel_") || setBonus.StartsWith("statigel "))
            {
                mp.statigelSet = enabled;
                return true;
            }

            // Mollusk
            if (setBonus == "mollusk")
            {
                mp.molluskSet = enabled;
                return true;
            }

            // Titan Heart
            if (setBonus == "titanheart" || setBonus == "titan heart")
            {
                mp.titanHeartMask = enabled;
                mp.titanHeartMantle = enabled;
                mp.titanHeartBoots = enabled;
                mp.titanHeartSet = enabled;
                return true;
            }

            // Forbidden Circlet
            if (setBonus == "forbidden_circlet" || setBonus == "forbidden circlet")
            {
                mp.forbiddenCirclet = enabled;
                return true;
            }

            // Daedalus
            switch (setBonus)
            {
                default:
                    break;
                case "daedalus_melee":
                case "daedalus melee":
                    mp.daedalusReflect = enabled;
                    return true;
                case "daedalus_ranged":
                case "daedalus ranged":
                    mp.daedalusShard = enabled;
                    return true;
                case "daedalus_magic":
                case "daedalus magic":
                    mp.daedalusAbsorb = enabled;
                    return true;
                case "daedalus_summon":
                case "daedalus summon":
                    mp.daedalusCrystal = enabled; // LATER -- remove this when player.daedalusCrystal actually controls daedalus summoner
                    return true;
                case "daedalus_rogue":
                case "daedalus rogue":
                    mp.daedalusSplit = enabled;
                    return true;
            }

            // Reaver
            switch (setBonus)
            {
                default:
                    break;
                case "reaver_speed":
                case "reaver speed":
                    mp.reaverSpeed = enabled;
                    return true;
                case "reaver_explore":
                case "reaver explore":
                case "reaver_exploration":
                case "reaver exploration":
                    mp.reaverExplore = enabled;
                    return true;
                case "reaver_defense":
                case "reaver defense":
                case "reaver_tank":
                case "reaver tank":
                    mp.reaverDefense = enabled;
                    return true;
            }

            // Fathom Swarmer
            if (setBonus == "fathomswarmer" || setBonus == "fathom swarmer")
            {
                mp.fathomSwarmer = enabled;
                return true;
            }

            // Brimflame
            if (setBonus == "brimflame")
            {
                mp.brimflameSet = enabled;
                return true;
            }

            // Umbraphile
            if (setBonus == "umbraphile")
            {
                mp.umbraphileSet = enabled;
                return true;
            }

            // Hydrothermic (Ataxia as legacy name)
            switch (setBonus)
            {
                default:
                    break;
                case "ataxia":
                case "hydrothermic":
                case "hydrothermal":
                    mp.ataxiaBlaze = enabled;
                    return true;
                case "ataxia_melee":
                case "ataxia melee":
                case "hydrothermic_melee":
                case "hydrothermic melee":
                case "hydrothermal_melee":
                case "hydrothermal melee":
                    mp.ataxiaBlaze = enabled;
                    mp.ataxiaGeyser = enabled;
                    return true;
                case "ataxia_ranged":
                case "ataxia ranged":
                case "hydrothermic_ranged":
                case "hydrothermic ranged":
                case "hydrothermal_ranged":
                case "hydrothermal ranged":
                    mp.ataxiaBlaze = enabled;
                    mp.ataxiaBolt = enabled;
                    return true;
                case "ataxia_magic":
                case "ataxia magic":
                case "hydrothermic_magic":
                case "hydrothermic magic":
                case "hydrothermal_magic":
                case "hydrothermal magic":
                    mp.ataxiaBlaze = enabled;
                    mp.ataxiaMage = enabled;
                    return true;
                case "ataxia_summon":
                case "ataxia summon":
                case "hydrothermic_summon":
                case "hydrothermic summon":
                case "hydrothermal_summon":
                case "hydrothermal summon":
                    mp.ataxiaBlaze = enabled;
                    mp.chaosSpirit = enabled; // LATER -- remove this when player.chaosSpirit actually controls ataxia summoner
                    return true;
                case "ataxia_rogue":
                case "ataxia rogue":
                case "hydrothermic_rogue":
                case "hydrothermic rogue":
                case "hydrothermal_rogue":
                case "hydrothermal rogue":
                    mp.ataxiaBlaze = enabled;
                    mp.ataxiaVolley = enabled;
                    return true;
            }

            // Plague Reaper
            if (setBonus == "plaguereaper" || setBonus == "plague reaper")
            {
                mp.plagueReaper = enabled;
                return true;
            }

            // Plaguebringer
            if (setBonus == "plaguebringer" || setBonus == "plaguebringerpatron" || setBonus == "plaguebringer patron")
            {
                mp.plaguebringerPatronSet = enabled;
                return true;
            }

            // Astral
            if (setBonus == "astral")
            {
                mp.astralStarRain = enabled;
                return true;
            }

            // Xeroc
            if (setBonus == "empyrean" || setBonus == "xeroc")
            {
                mp.xerocSet = enabled;
                return true;
            }

            // Tarragon
            switch (setBonus)
            {
                default:
                    break;
                case "tarragon":
                    mp.tarraSet = enabled;
                    return true;
                case "tarragon_melee":
                case "tarragon melee":
                    mp.tarraSet = enabled;
                    mp.tarraMelee = enabled;
                    return true;
                case "tarragon_ranged":
                case "tarragon ranged":
                    mp.tarraSet = enabled;
                    mp.tarraRanged = enabled;
                    return true;
                case "tarragon_magic":
                case "tarragon magic":
                    mp.tarraSet = enabled;
                    mp.tarraMage = enabled;
                    return true;
                case "tarragon_summon":
                case "tarragon summon":
                    mp.tarraSet = enabled;
                    mp.tarraSummon = enabled; // LATER -- remove this when player.tarraSummon actually controls life aura
                    return true;
                case "tarragon_rogue":
                case "tarragon rogue":
                    mp.tarraSet = enabled;
                    mp.tarraThrowing = enabled;
                    return true;
            }

            // Prismatic
            if (setBonus == "prismatic" || setBonus == "prism")
            {
                mp.prismaticSet = enabled;
                return true;
            }

            // Bloodflare
            switch (setBonus)
            {
                default:
                    break;
                case "bloodflare":
                    mp.bloodflareSet = enabled;
                    return true;
                case "bloodflare_melee":
                case "bloodflare melee":
                    mp.bloodflareSet = enabled;
                    mp.bloodflareMelee = enabled;
                    return true;
                case "bloodflare_ranged":
                case "bloodflare ranged":
                    mp.bloodflareSet = enabled;
                    mp.bloodflareRanged = enabled;
                    return true;
                case "bloodflare_magic":
                case "bloodflare magic":
                    mp.bloodflareSet = enabled;
                    mp.bloodflareMage = enabled;
                    return true;
                case "bloodflare_summon":
                case "bloodflare summon":
                    mp.bloodflareSet = enabled;
                    mp.bloodflareSummon = enabled; // LATER -- remove this when player.bloodflareSummon actually controls bloodflare orbs
                    return true;
                case "bloodflare_rogue":
                case "bloodflare rogue":
                    mp.bloodflareSet = enabled;
                    mp.bloodflareThrowing = enabled;
                    return true;
            }

            // Omega Blue
            if (setBonus == "omegablue" || setBonus == "omega blue")
            {
                mp.omegaBlueSet = enabled;
                return true;
            }

            // God Slayer
            switch (setBonus)
            {
                default:
                    break;
                case "godslayer":
                case "god slayer":
                    mp.godSlayer = enabled;
                    return true;
                case "godslayer_melee":
                case "godslayer melee":
                case "god slayer melee":
                    mp.godSlayer = enabled;
                    mp.godSlayerDamage = enabled; // melee helm's unique damage reducing property
                    return true;
                case "godslayer_ranged":
                case "godslayer ranged":
                case "god slayer ranged":
                    mp.godSlayer = enabled;
                    mp.godSlayerRanged = enabled;
                    return true;
                // God Slayer Mage and Summon were removed in the Draedon Update
                /*
                case "godslayer_magic":
                case "godslayer magic":
                case "god slayer magic":
                    mp.godSlayer = enabled;
                    mp.godSlayerMage = enabled;
                    return true;
                case "godslayer_summon":
                case "godslayer summon":
                case "god slayer summon":
                    mp.godSlayer = enabled;
                    mp.godSlayerSummon = enabled;
                    return true;
                */
                case "godslayer_rogue":
                case "godslayer rogue":
                case "god slayer rogue":
                    mp.godSlayer = enabled;
                    mp.godSlayerThrowing = enabled;
                    return true;
            }

            // Fearmonger
            if (setBonus == "fearmonger")
            {
                mp.fearmongerSet = enabled;
                return true;
            }

            // Silva
            switch (setBonus)
            {
                default:
                    break;
                case "silva":
                    mp.silvaSet = enabled;
                    return true;
                case "silva_magic":
                case "silva magic":
                    mp.silvaSet = enabled;
                    mp.silvaMage = enabled;
                    return true;
                case "silva_summon":
                case "silva summon":
                    mp.silvaSet = enabled;
                    mp.silvaSummon = enabled; // LATER -- remove this when player.silvaSummon actually controls silva crystal
                    return true;
                // Silva Melee, Ranged and Rogue were removed in the Draedon Update
                /*
                case "silva_melee":
                case "silva melee":
                    mp.silvaSet = enabled;
                    mp.silvaMelee = enabled;
                    return true;
                case "silva_ranged":
                case "silva ranged":
                    mp.silvaSet = enabled;
                    mp.silvaRanged = enabled;
                    return true;
                case "silva_rogue":
                case "silva rogue":
                    mp.silvaSet = enabled;
                    mp.silvaThrowing = enabled;
                    return true;
                */
            }

            // Auric Tesla (includes all components)
            switch (setBonus)
            {
                default:
                    break;
                case "auric":
                case "aurictesla":
                case "auric tesla":
                    mp.tarraSet = enabled;
                    mp.bloodflareSet = enabled;
                    mp.godSlayer = enabled;
                    mp.silvaSet = enabled;
                    mp.auricSet = enabled;
                    return true;
                case "auric_melee":
                case "auric melee":
                case "aurictesla_melee":
                case "aurictesla melee":
                case "auric tesla melee":
                    mp.tarraSet = enabled;
                    mp.tarraMelee = enabled;
                    mp.bloodflareSet = enabled;
                    mp.bloodflareMelee = enabled;
                    mp.godSlayer = enabled;
                    mp.godSlayerDamage = enabled;
                    mp.silvaSet = enabled;
                    // mp.silvaMelee = enabled;
                    mp.auricSet = enabled;
                    return true;
                case "auric_ranged":
                case "auric ranged":
                case "aurictesla_ranged":
                case "aurictesla ranged":
                case "auric tesla ranged":
                    mp.tarraSet = enabled;
                    mp.tarraRanged = enabled;
                    mp.bloodflareSet = enabled;
                    mp.bloodflareRanged = enabled;
                    mp.godSlayer = enabled;
                    mp.godSlayerRanged = enabled;
                    mp.silvaSet = enabled;
                    // mp.silvaRanged = enabled;
                    mp.auricSet = enabled;
                    return true;
                case "auric_magic":
                case "auric magic":
                case "aurictesla_magic":
                case "aurictesla magic":
                case "auric tesla magic":
                    mp.tarraSet = enabled;
                    mp.tarraMage = enabled;
                    mp.bloodflareSet = enabled;
                    mp.bloodflareMage = enabled;
                    mp.godSlayer = enabled;
                    // mp.godSlayerMage = enabled;
                    mp.silvaSet = enabled;
                    mp.silvaMage = enabled;
                    mp.auricSet = enabled;
                    return true;
                case "auric_summon":
                case "auric summon":
                case "aurictesla_summon":
                case "aurictesla summon":
                case "auric tesla summon":
                    mp.tarraSet = enabled;
                    mp.tarraSummon = enabled;
                    mp.bloodflareSet = enabled;
                    mp.bloodflareSummon = enabled;
                    mp.godSlayer = enabled;
                    // mp.godSlayerSummon = enabled;
                    mp.silvaSet = enabled;
                    mp.silvaSummon = enabled;
                    mp.auricSet = enabled;
                    return true;
                case "auric_rogue":
                case "auric rogue":
                case "aurictesla_rogue":
                case "aurictesla rogue":
                case "auric tesla rogue":
                    mp.tarraSet = enabled;
                    mp.tarraThrowing = enabled;
                    mp.bloodflareSet = enabled;
                    mp.bloodflareThrowing = enabled;
                    mp.godSlayer = enabled;
                    mp.godSlayerThrowing = enabled;
                    mp.silvaSet = enabled;
                    // mp.silvaThrowing = enabled;
                    mp.auricSet = enabled;
                    return true;
            }

            // Demonshade
            if (setBonus == "demonshade")
            {
                mp.dsSetBonus = enabled;
                mp.rDevil = enabled; // LATER -- remove this when player.rDevil controls demonshade summoned minion
                return true;
            }

            return false;
        }
        #endregion

        #region Other Player Stats
        public static int GetLightStrength(Player p) => p?.GetCurrentAbyssLightLevel() ?? 0;

        public static void AddAbyssLightStrength(Player p, int add)
        {
            if (p != null)
                p.Calamity().externalAbyssLight += add;
        }

        public static bool MakeColdImmune(Player p) => p is null ? false : (p.Calamity().externalColdImmunity = true);
        public static bool MakeHeatImmune(Player p) => p is null ? false : (p.Calamity().externalHeatImmunity = true);
        #endregion

        #region Set Damage Reduction
        public static float SetDamageReduction(int npcID, float dr)
        {
            CalamityMod.DRValues.TryGetValue(npcID, out float oldDR);
            CalamityMod.DRValues.Remove(npcID);
            CalamityMod.DRValues.Add(npcID, dr);
            return oldDR;
        }
        #endregion

        #region Boss Health Bars
        public static bool BossHealthBarVisible() => Main.LocalPlayer.Calamity().drawBossHPBar;

        public static bool SetBossHealthBarVisible(bool visible) => Main.LocalPlayer.Calamity().drawBossHPBar = visible;
        #endregion

        #region Dodge Disabling
        public static bool AreDodgesDisabled() => Main.LocalPlayer.Calamity().disableAllDodges;

        public static bool DisableAllDodges(bool disable) => Main.LocalPlayer.Calamity().disableAllDodges = disable;
        #endregion

        #region Item Rarity
        public static int GetCalamityRarity(Item item)
        {
            CalamityGlobalItem cgi = item.Calamity();
            CalamityRarity calrare = cgi.customRarity;
            if (calrare == CalamityRarity.NoEffect)
                return item.rare;
            return (int)calrare;
        }

        public static bool SetCalamityRarity(Item item, int rarityNum)
        {
            CalamityGlobalItem cgi = item.Calamity();
            cgi.customRarity = (CalamityRarity)rarityNum;
            return cgi.customRarity != CalamityRarity.NoEffect;
        }
        #endregion

        #region Summoner Cross Class Nerf Disabling
        public static bool SetSummonerNerfDisabledByMinion(int type, bool disableNerf)
        {
            if (disableNerf && !CalamityLists.DisabledSummonerNerfMinions.Contains(type))
            {
                CalamityLists.DisabledSummonerNerfMinions.Add(type);
                return true;
            }
            else if (!disableNerf)
            {
                return CalamityLists.DisabledSummonerNerfMinions.Remove(type);
            }

            return false;
        }
        public static bool SetSummonerNerfDisabledByItem(int type, bool disableNerf)
        {
            if (disableNerf && !CalamityLists.DisabledSummonerNerfItems.Contains(type))
            {
                CalamityLists.DisabledSummonerNerfItems.Add(type);
                return true;
            }
            else if (!disableNerf)
            {
                return CalamityLists.DisabledSummonerNerfItems.Remove(type);
            }

            return false;
        }

        public static bool GetSummonerNerfDisabledByMinion(int type) => CalamityLists.DisabledSummonerNerfMinions.Contains(type);
        public static bool GetSummonerNerfDisabledByItem(int type) => CalamityLists.DisabledSummonerNerfItems.Contains(type);
        #endregion

        #region Call

        public static object Call(params object[] args)
        {
            bool isValidPlayerArg(object o) => o is int || o is Player;
            bool isValidItemArg(object o) => o is int || o is Item;
            bool isValidProjectileArg(object o) => o is int || o is Projectile;

            Player castPlayer(object o)
            {
                if (o is int i)
                    return Main.player[i];
                else if (o is Player p)
                    return p;
                return null;
            }

            Item castItem(object o)
            {
                if (o is int i)
                    return Main.item[i];
                else if (o is Item it)
                    return it;
                return null;
            }

            Projectile castProjectile(object o)
            {
                if (o is int i)
                    return Main.projectile[i];
                else if (o is Projectile p)
                    return p;
                return null;
            }

            // Certain IDs in vanilla's files are shorts instead of ints for some reason.
            // Instead of expecting developers to have to manually cast these IDs, this function is used
            // to handle IDs that are either ints OR shorts, without the worry of missing a cast and wondering why
            // the Mod Call did nothing.
            bool castID(object o, out int id)
            {
                id = -1;
                if (!(o is int) && !(o is short))
                    return false;

                if (o is short shortID)
                    id = shortID;
                if (o is int intID)
                    id = intID;

                return true;
            }

            if (args is null || args.Length <= 0)
                return new ArgumentNullException("ERROR: No function name specified. First argument must be a function name.");
            if (!(args[0] is string))
                return new ArgumentException("ERROR: First argument must be a string function name.");

            string methodName = args[0].ToString();
            switch (methodName)
            {
                case "Downed":
                case "GetDowned":
                case "BossDowned":
                case "GetBossDowned":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a boss or event name as a string.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The argument to \"Downed\" must be a string.");
                    return GetBossDowned(args[1].ToString());

                case "Zone":
                case "GetZone":
                case "InZone":
                case "GetInZone":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and a zone name as a string.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify a zone name as a string.");
                    if (!(args[2] is string))
                        return new ArgumentException("ERROR: The second argument to \"InZone\" must be a string.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"InZone\" must be a Player or an int.");
                    return GetInZone(castPlayer(args[1]), args[2].ToString());

                case "Difficulty":
                case "GetDifficulty":
                case "DifficultyActive":
                case "GetDifficultyActive":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a difficulty mode name as a string.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The argument to \"Difficulty\" must be a string.");
                    return GetDifficultyActive(args[1].ToString());

                case "SetDifficulty":
                case "SetDifficultyActive":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a difficulty mode name as a string and a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify a bool.");
                    if (!(args[2] is bool enabled))
                        return new ArgumentException("ERROR: The second argument to \"SetDifficulty\" must be a bool.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The first argument to \"SetDifficulty\" must be a string.");
                    return SetDifficultyActive(args[1].ToString(), enabled);

                case "GetLight":
                case "GetLightLevel":
                case "GetLightStrength":
                case "GetAbyssLight":
                case "GetAbyssLightLevel":
                case "GetAbyssLightStrength":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if(!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The argument to \"GetLightStrength\" must be a Player or an int.");
                    return GetLightStrength(castPlayer(args[1]));

                case "AddLight":
                case "AddLightLevel":
                case "AddLightStrength":
                case "AddAbyssLight":
                case "AddAbyssLightLevel":
                case "AddAbyssLightStrength":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and light strength change as an int.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify light strength change as an int.");
                    if (!(args[2] is int light))
                        return new ArgumentException("ERROR: The second argument to \"AddLightStrength\" must be an int.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddLightStrength\" must be a Player or an int.");
                    AddAbyssLightStrength(castPlayer(args[1]), light);
                    return null;

                case "GetRogueVelocity":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The argument to \"GetRogueVelocity\" must be a Player or an int.");
                    return GetRogueVelocity(castPlayer(args[1]));

                case "AddRogueVelocity":
                case "ModifyRogueVelocity":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue velocity change as a float.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify rogue velocity change as a float.");
                    if (!(args[2] is float velocity))
                        return new ArgumentException("ERROR: The second argument to \"AddRogueVelocity\" must be a float.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddRogueVelocity\" must be a Player or an int.");
                    return AddRogueVelocity(castPlayer(args[1]), velocity);

                case "GetStealth":
                case "GetCurrentStealth":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetStealth\" must be a Player or an int.");
                    return GetCurrentStealth(castPlayer(args[1]));

                case "GetMaxStealth":
                case "GetStealthCap":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetMaxStealth\" must be a Player or an int.");
                    return GetMaxStealth(castPlayer(args[1]));

                case "AddMaxStealth":
                case "ModifyMaxStealth":
                case "ModifyStealthCap":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue max stealth as a float.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify rogue max stealth as a float.");
                    if (!(args[2] is float maxStealth))
                        return new ArgumentException("ERROR: The second argument to \"AddMaxStealth\" must be a float.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddMaxStealth\" must be a Player or an int.");
                    return AddMaxStealth(castPlayer(args[1]), maxStealth);

                case "DR":
                case "DamageReduction":
                case "SetDR":
                case "SetDamageReduction":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both NPC ID as an int and damage reduction as a float or double.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify damage reduction as a float or double.");
                    if (!(args[2] is float) && !(args[2] is double))
                        return new ArgumentException("ERROR: The second argument to \"SetDamageReduction\" must be a float or a double.");
                    if (!castID(args[1], out int npcID))
                        return new ArgumentException("ERROR: The first argument to \"SetDamageReduction\" must be an int or short ID.");

                    float DR = (float)args[2];
                    return SetDamageReduction(npcID, DR);

                case "BossHealthBarVisible":
                case "BossHealthBarsVisible":
                case "GetBossHealthBarVisible":
                case "GetBossHealthBarsVisible":
                    return BossHealthBarVisible();

                case "SetBossHealthBarVisible":
                case "SetBossHealthBarsVisible":
                    if (args.Length < 2 || !(args[1] is bool bossBarEnabled))
                        return new ArgumentNullException("ERROR: Must specify a bool.");
                    return SetBossHealthBarVisible(bossBarEnabled);

                case "NoDodges":
                case "DodgesDisabled":
                case "GetDodgesDisabled":
                    return AreDodgesDisabled();

                case "DisableDodges":
                case "DisableAllDodges":
                case "SetDodgesDisabled":
                    if (args.Length < 2 || !(args[1] is bool disableDodges))
                        return new ArgumentNullException("ERROR: Must specify a bool.");
                    return DisableAllDodges(disableDodges);

                case "GetRarity":
                case "GetItemRarity":
                case "GetCalamityRarity":
                case "GetPostMLRarity":
                case "GetPostMoonLordRarity":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify an Item.");
                    if (!(args[1] is Item))
                        return new ArgumentException("ERROR: The first argument to \"GetCalamityRarity\" must be an Item.");
                    Item itemToGet = (Item)args[1];
                    return GetCalamityRarity(itemToGet);

                case "SetRarity":
                case "SetItemRarity":
                case "SetCalamityRarity":
                case "SetPostMLRarity":
                case "SetPostMoonLordRarity":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both an Item and desired rarity as an int or short ID.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify desired rarity as an int or short ID.");
                    if (!castID(args[2], out int rarity))
                        return new ArgumentException("ERROR: The second argument to \"SetCalamityRarity\" must be an int or short ID.");
                    if (!(args[1] is Item))
                        return new ArgumentException("ERROR: The first argument to \"SetCalamityRarity\" must be an Item.");

                    Item itemToSet = (Item)args[1];
                    return SetCalamityRarity(itemToSet, rarity);

                case "AcidRainActive":
                case "IsAcidRainActive":
                case "GetAcidRainActive":
                    return AcidRainEvent.AcidRainEventIsOngoing;

                case "StartAcidRain":
                    AcidRainEvent.TryStartEvent(true);
                    CalamityNetcode.SyncWorld();
                    return true;

                case "StopAcidRain":
                    if (AcidRainEvent.AcidRainEventIsOngoing)
                    {
                        AcidRainEvent.AccumulatedKillPoints = 0;
                        AcidRainEvent.HasTriedToSummonOldDuke = false;
                        AcidRainEvent.UpdateInvasion(false);
                    }
                    return true;

                // This is intentionally separate from the above because it will stop other events when they are added.
                case "AbominationnClearEvents":
                    bool eventActive = AcidRainEvent.AcidRainEventIsOngoing;
                    bool canClear = Convert.ToBoolean(args[1]); //This is to indicate whether abomm is able to clear the event due to a cooldown
                    if (eventActive && canClear) //adjust based on other events when added.
                    {
                        AcidRainEvent.AccumulatedKillPoints = 0;
                        AcidRainEvent.HasTriedToSummonOldDuke = false;
                        AcidRainEvent.UpdateInvasion(false);
                    }
                    return eventActive;

                case "ExcludeMinionsFromResurrection":
                    // This assumes all arguments after the calling command name are projectile types.
                    IEnumerable<object> secondaryArguments = args.Skip(1);
                    if (secondaryArguments.Any(argument => !castID(argument, out _)))
                        return new ArgumentException("ERROR: All arguments after the calling command to \"ExcludeMinionsFromResurrection\" must be int or short IDs.");

                    CalamityLists.MinionsToNotResurrectList.AddRange(secondaryArguments.Select(argument =>
                    {
                        castID(argument, out int id);
                        return id;
                    }));
                    return null;

                case "CreateEnchantment":
                case "RegisterEnchantment":
                    EnchantmentManager.ConstructFromModcall(args.Skip(1));
                    return null;

                case "MakeItemExhumable":
                    if (args.Length != 3)
                        return new ArgumentNullException("ERROR: Must specify two Item types as an int.");
                    if (!castID(args[1], out int toExhume))
                        return new ArgumentException("ERROR: The first argument to \"MakeItemExhumable\" must be an int or short ID.");
                    if (!castID(args[2], out int result))
                        return new ArgumentException("ERROR: The second argument to \"MakeItemExhumable\" must be an int or short ID.");
                    EnchantmentManager.ItemUpgradeRelationship[toExhume] = result;
                    return null;

                case "DeclareMiniboss":
                case "DeclareMinibossForHealthBar":
                    if (args.Length != 2)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type as an int.");
                    if (!castID(args[1], out int npcType))
                        return new ArgumentException("ERROR: The first argument to \"DeclareMiniboss\" must be an int or short ID.");

                    BossHealthBarManager.MinibossHPBarList.Add(npcType);
                    return null;

                case "ExcludeBossFromHealthBar":
                    if (args.Length != 2)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type as an int.");
                    if (!castID(args[1], out int npcType2))
                        return new ArgumentException("ERROR: The first argument to \"ExcludeBossFromHealthBar\" must be an int or short ID.");

                    BossHealthBarManager.BossExclusionList.Add(npcType2);
                    return null;

                case "DeclareOneToManyRelationshipForHealthBar":
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type as an int for the first argument and the other NPC types in the relationship as ints for the rest of the arguments.");
                    if (!args.Skip(1).All(a => castID(a, out _)))
                        return new ArgumentException("ERROR: All secondary and onward arguments to \"DeclareOneToManyRelationshipForHealthBar\" must be int or short IDs.");

                    castID(args[1], out int npcType3);

                    int[] npcsInRelationship = args.Skip(2).Select(a => (int)a).ToArray();
                    BossHealthBarManager.OneToMany[npcType3] = npcsInRelationship;
                    return null;

                // For context, the boolean argument in the second delegate refers to whether the function should be accumulating max life (true) or just life (false), and the returned long should be the accumulated health.
                case "DeclareSpecialHPCalculationDecisionForHealthBar":
                    if (args.Length != 3)
                        return new ArgumentNullException("ERROR: Must specify both a usage requirement as a Func<NPC, bool> and a health calculator function as a Func<NPC, bool, long>.");
                    if (!(args[1] is Func<NPC, bool> usageRequirement))
                        return new ArgumentException("ERROR: The first argument to \"DeclareSpecialHPCalculationDecisionForHealthBar\" must be a Func<NPC, bool>.");
                    if (!(args[2] is Func<NPC, bool, long> healthCalculatorFunction))
                        return new ArgumentException("ERROR: The first argument to \"DeclareSpecialHPCalculationDecisionForHealthBar\" must be a Func<NPC, bool, long>.");

                    BossHealthBarManager.SpecialHPRequirements[new BossHealthBarManager.NPCSpecialHPGetRequirement(usageRequirement)] = new BossHealthBarManager.NPCSpecialHPGetFunction(healthCalculatorFunction);
                    return null;

                case "CreateNameExtensionHandlerForHealthBar":
                    if (args.Length < 4)
                        return new ArgumentNullException("ERROR: Must specify a extension name as a string, the main NPC type as an int, and the other NPC types to check for as ints the rest of the arguments.");
                    if (!(args[1] is string name))
                        return new ArgumentException("ERROR: The first argument to \"CreateNameExtensionHandlerForHealthBar\" must be a string.");
                    if (!castID(args[1], out int npcType4))
                        return new ArgumentException("ERROR: The second argument to \"CreateNameExtensionHandlerForHealthBar\" must be an int or short ID.");
                    if (!args.Skip(3).All(a => a is int))
                        return new ArgumentException("ERROR: All ternary and onward arguments to \"CreateNameExtensionHandlerForHealthBar\" must be ints.");

                    int[] npcsToCheckFor = args.Skip(3).Select(a => (int)a).ToArray();
                    BossHealthBarManager.EntityExtensionHandler[npcType4] = new BossHealthBarManager.BossEntityExtension(name, npcsToCheckFor);
                    return null;

                // In the following two mod calls, the first argument is the NPC type, the second is the time change context (-1 being night, 0 being nothing, and 1 being day),
                // the third being the boss spawning function, the fourth being the overriding countdown to use, the fifth being whether the boss uses a special sound on spawning,
                // the sixth being the dimness factor that Boss Rush should become once the boss is currently present, the seventh being the array of NPCs present in the battle that
                // should not be deleted by the Boss Rush itself, and the eight being the potential NPCs that will end up killing, assuming the initial boss isn't
                // that (such as P1 Hive Mind turning into its second form and you being expected to kill that).
                case "GetBossRushEntries":
                    var entries = new List<(int, int, Action<int>, int, bool, float, int[], int[])>();
                    foreach (BossRushEvent.Boss boss in BossRushEvent.Bosses)
                    {
                        int[] deathEntries = BossRushEvent.BossIDsAfterDeath.ContainsKey(boss.EntityID) ? BossRushEvent.BossIDsAfterDeath[boss.EntityID] : null;
                        entries.Add((boss.EntityID, (int)boss.ToChangeTimeTo, new Action<int>(boss.SpawnContext), boss.SpecialSpawnCountdown, boss.UsesSpecialSound, boss.DimnessFactor, boss.HostileNPCsToNotDelete.ToArray(), deathEntries));
                    }

                    return entries;

                case "SetBossRushEntries":
                    if (args.Length != 2)
                        return new ArgumentNullException("ERROR: Must specify a list of bosses as a List<(int, int, Action<int>, int, bool, int[], int[])>.");
                    if (!(args[1] is List<(int, int, Action<int>, int, bool, float, int[], int[])> entries2))
                        return new ArgumentException("ERROR: The first argument to \"SetBossRushEntries\" must be a List<(int, int, Action<int>, int, bool, int[], int[])>.");

                    BossRushEvent.Bosses.Clear();
                    BossRushEvent.BossIDsAfterDeath.Clear();
                    foreach (var entry in entries2)
                    {
                        if (entry.Item8 != null)
                            BossRushEvent.BossIDsAfterDeath[entry.Item1] = entry.Item8;
                        BossRushEvent.Bosses.Add(new BossRushEvent.Boss(entry.Item1, (BossRushEvent.TimeChangeContext)entry.Item2, new BossRushEvent.Boss.OnSpawnContext(entry.Item3), entry.Item4, entry.Item5, entry.Item6, entry.Item7));
                    }

                    return null;

                case "CreateCustomDeathEffectForBossRush":
                    if (args.Length != 3)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type and an Action<NPC> that determines what happens when the NPC is killed.");
                    if (!castID(args[1], out int npcType5))
                        return new ArgumentException("ERROR: The first argument to \"CreateCustomDeathEffectForBossRush\" must be an int or short ID.");
                    if (!(args[2] is Action<NPC> deathEffect))
                        return new ArgumentException("ERROR: The first argument to \"CreateCustomDeathEffectForBossRush\" must be an Action<NPC>.");

                    BossRushEvent.BossDeathEffects[npcType5] = deathEffect;
                    return null;

                case "LoadParticleInstances":
                    if (args.Length != 2 || !(args[1] is Mod))
                        return new ArgumentNullException("ERROR: Must specify a Mod instance to load particles from.");

                    GeneralParticleHandler.LoadModParticleInstances(args[1] as Mod);
                    return null;

                case "RegisterModCooldowns":
                    if (args.Length != 2 || !(args[1] is Mod))
                        return new ArgumentNullException("ERROR: Must specify a Mod instance to register cooldowns from.");

                    CooldownRegistry.RegisterModCooldowns(args[1] as Mod);
                    return null;

                case "GetSummonerNerfDisabledByItem":
                    if (args.Length != 2 || !isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: Must specify a valid item to check status of.");
                    return GetSummonerNerfDisabledByItem(castItem(args[1]).type);

                case "GetSummonerNerfDisabledByMinion":
                    if (args.Length != 2 || !isValidProjectileArg(args[1]))
                        return new ArgumentException("ERROR: Must specify a valid projectile to check status of.");
                    return GetSummonerNerfDisabledByMinion(castProjectile(args[1]).type);

                case "SetSummonerNerfDisabledByItem":
                    if (args.Length < 2 || !isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: Must specify a valid item to set the status of.");
                    if (args.Length != 3 || args[2] is not bool disableNerf)
                        return new ArgumentException("ERROR: Must specify a bool that determines whether the summoner nerf is disabled.");
                    return SetSummonerNerfDisabledByItem(castItem(args[1]).type, disableNerf);

                case "SetSummonerNerfDisabledByMinion":
                    if (args.Length < 2 || !isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: Must specify a valid projectile to set the status of.");
                    if (args.Length != 3 || args[2] is not bool disableNerf2)
                        return new ArgumentException("ERROR: Must specify a bool that determines whether the summoner nerf is disabled.");
                    return SetSummonerNerfDisabledByItem(castItem(args[1]).type, disableNerf2);

                default:
                    return new ArgumentException("ERROR: Invalid method name.");
            }
        }
        #endregion
    }
}
