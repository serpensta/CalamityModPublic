﻿using System.Collections.Generic;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using CalamityMod.Items.SummonItems;
using static CalamityMod.World.CalamityWorld;

namespace CalamityMod.Systems
{
    public class WorldgenManagementSystem : ModSystem
    {
        #region PreWorldGen
        public override void PreWorldGen()
        {
            Abyss.TotalPlacedIslandsSoFar = 0;
            roxShrinePlaced = false;

            // This will only be applied at world-gen time to new worlds.
            // Old worlds will never receive this marker naturally.
            IsWorldAfterDraedonUpdate = true;
        }
        #endregion

        #region ModifyWorldGenTasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            //Evil Floating Island
            int islandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Island Houses"));
            if (islandIndex != -1)
            {
                tasks.Insert(islandIndex + 2, new PassLegacy("Evil Island", (progress, config) =>
                {
                    progress.Message = WorldGen.crimson ? "Adding a putrid floating island" : "Adding a grotesque floating island";
                    WorldEvilIsland.PlaceEvilIsland();
                }));
            }

            //Calamity's biome chests in the dungeon
            int DungeonChestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (DungeonChestIndex != -1)
            {
                tasks.Insert(DungeonChestIndex + 1, new PassLegacy("CalamityDungeonBiomeChests", MiscWorldgenRoutines.GenerateBiomeChests));
            }

            //Larger Jungle Temple
            int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
            {
                progress.Message = "Building a bigger jungle temple";
                CustomTemple.NewJungleTemple();
            });

            //Improved Golem arena
            int JungleTempleIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Temple"));
            tasks[JungleTempleIndex2] = new PassLegacy("Temple", (progress, config) =>
            {
                progress.Message = "Building a better jungle temple";
                Main.tileSolid[162] = false;
                Main.tileSolid[226] = true;
                CustomTemple.NewJungleTemplePart2();
                Main.tileSolid[232] = false;
            });

            //Better Lihzahrd altar (consistency?)
            int LihzahrdAltarIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
            tasks[LihzahrdAltarIndex] = new PassLegacy("Lihzahrd Altars", (progress, config) =>
            {
                progress.Message = "Placing the Lihzahrd altar";
                CustomTemple.NewJungleTempleLihzahrdAltar();
            });

            //Giant beehive
            int giantHiveIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Hives"));
            if (giantHiveIndex != -1)
            {
                tasks.Insert(giantHiveIndex + 1, new PassLegacy("Giant Hive", (progress, config) =>
                {
                    progress.Message = "Building a giant beehive";
                    int attempts = 0;
                    while (attempts < 1000)
                    {
                        attempts++;
                        Point origin = WorldGen.RandomWorldPoint((int)Main.worldSurface + 25, 20, Main.maxTilesY - (int)Main.worldSurface - 125, 20);
                        if (GiantHive.CanPlaceGiantHive(origin, GenVars.structures))
                            break;
                    }
                }));
            }

            //mechanic shed
            int mechanicIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));
            if (mechanicIndex != -1)
            {
                tasks.Insert(mechanicIndex + 1, new PassLegacy("Mechanic Shed", (progress, config) =>
                {
                    progress.Message = "Placing mechanic shed";
                    MechanicShed.PlaceMechanicShed(GenVars.structures);
                }));
            }

            //Vernal pass
            int vernalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Muds Walls In Jungle"));
            if (vernalIndex != -1)
            {
                tasks.Insert(vernalIndex + 1, new PassLegacy("Vernal Pass", (progress, config) =>
                {
                    progress.Message = "Blessing a flourishing jungle grove";
                    VernalPass.PlaceVernalPass(GenVars.structures);
                }));
            }

            //Sunken sea
            int SunkenSeaIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids Again"));
            if (SunkenSeaIndex != -1)
            {
                tasks.Insert(SunkenSeaIndex + 1, new PassLegacy("Sunken Sea", (progress, config) =>
                {
                    progress.Message = "Partially flooding an overblown desert";

                    int sunkenSeaX = GenVars.UndergroundDesertLocation.Left;
                    int sunkenSeaY = Main.maxTilesY - 400;

                    SunkenSea.Place(new Point(sunkenSeaX, sunkenSeaY));
                }));
            }

            //All further tasks occur after vanilla worldgen is completed
            int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalIndex != -1)
            {
                //Reallocate gems so rarity corresponds to depth
                int currentFinalIndex = FinalIndex;
                tasks.Insert(++currentFinalIndex, new PassLegacy("Gem Depth Adjustment", (progress, config) =>
                {
                    progress.Message = "Sensibly shuffling gem depth";
                    MiscWorldgenRoutines.SmartGemGen();
                }));

                //Forsaken Archive structure in the Dungeon
                tasks.Insert(++currentFinalIndex, new PassLegacy("Forsaken Archive", (progress, config) =>
                {
                    progress.Message = "Entombing occult literature";
                    DungeonArchive.PlaceArchive();
                }));

                //Planetoids
                tasks.Insert(++currentFinalIndex, new PassLegacy("Planetoids", Planetoid.GenerateAllBasePlanetoids));

                //Sulphurous Sea (Step 1)
                int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
                if (SulphurIndex != -1)
                {
                    tasks.Insert(SulphurIndex + 1, new PassLegacy("Sulphur Sea", (progress, config) =>
                    {
                        progress.Message = "Polluting one of the oceans";
                        SulphurousSea.PlaceSulphurSea();
                    }));
                }

                //Brimstone Crags
                tasks.Insert(++currentFinalIndex, new PassLegacy("Brimstone Crag", (progress, config) =>
                {
                    progress.Message = "Incinerating Azafure";
                    BrimstoneCrag.GenAllCragsStuff();
                }));

                //Biome shrines
                tasks.Insert(++currentFinalIndex, new PassLegacy("Special Shrines", (progress, config) =>
                {
                    progress.Message = "Hiding forbidden shrines";

                    //Cuts down on worldgen time to process the right one.
                    //TODO -- Possible Both Evils compat whenever
                    if (WorldGen.crimson)
                    {
                        progress.Message = "Slaying a vengeful god";
                        UndergroundShrines.PlaceCrimsonShrine(GenVars.structures);
                    }
                    else
                    {
                        progress.Message = "Rotting a god's grave";
                        UndergroundShrines.PlaceCorruptionShrine(GenVars.structures);
                    }                    

                    progress.Message = "Burrowing an artifact from Osiris";
                    UndergroundShrines.PlaceDesertShrine(GenVars.structures);

                    progress.Message = "Crystallizing a deep blue geode";
                    UndergroundShrines.PlaceGraniteShrine(GenVars.structures);

                    progress.Message = "Hiding a beast tamer's igloo";
                    UndergroundShrines.PlaceIceShrine(GenVars.structures);

                    progress.Message = "Recovering a piece of the Parthenon";
                    UndergroundShrines.PlaceMarbleShrine(GenVars.structures);

                    progress.Message = "Planting a mushroom rich of hyphae";
                    UndergroundShrines.PlaceMushroomShrine(GenVars.structures);

                    progress.Message = "Assembling a shinobi hideout";
                    UndergroundShrines.PlaceSurfaceShrine(GenVars.structures);
                }));

                //aerialite
                //this MUST generate after the evil island, otherwise the ores keep getting painted from the evil island gen
                tasks.Insert(++currentFinalIndex, new PassLegacy("Aerialite", (progress, config) =>
                {
                    progress.Message = "Hiding wyvern's gold in plain sight";
                    AerialiteOreGen.Generate(false);
                }));

                //Draedon Labs
                tasks.Insert(++currentFinalIndex, new PassLegacy("Draedon Structures", (progress, config) =>
                {
                    progress.Message = "Rust and Dust";
                    List<Point> workshopPositions = new List<Point>();

                    // Small: 4, Normal: 7, Large: 9
                    // Tries to scale up reasonably for XL worlds
                    int workshopCount = Main.maxTilesX / 900;

                    // Small: 2, Normal: 4, Large: 5
                    // Tries to scale up reasonably for XL worlds
                    int labCount = Main.maxTilesX / 1500;

                    progress.Message = "Forging with the fires of hell";
                    DraedonStructures.PlaceHellLab(out Point hellPlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(hellPlacementPosition);

                    progress.Message = "Studying marine biology";
                    DraedonStructures.PlaceSunkenSeaLab(out Point sunkenSeaPlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(sunkenSeaPlacementPosition);

                    progress.Message = "Prototyping quantum supercooling";
                    DraedonStructures.PlaceIceLab(out Point icePlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(icePlacementPosition);

                    progress.Message = "Developing abhorrent bioweaponry";
                    DraedonStructures.PlacePlagueLab(out Point plaguePlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(plaguePlacementPosition);

                    progress.Message = "Strip mining for minerals";
                    DraedonStructures.PlaceCavernLab(out Point cavernPlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(cavernPlacementPosition);

                    progress.Message = "Abandoned engineering projects";
                    for (int i = 0; i < workshopCount; i++)
                    {
                        DraedonStructures.PlaceWorkshop(out Point placementPosition, workshopPositions, GenVars.structures);
                        workshopPositions.Add(placementPosition);
                    }
                    progress.Message = "Other minor research projects";
                    for (int i = 0; i < labCount; i++)
                    {
                        DraedonStructures.PlaceResearchFacility(out Point placementPosition, workshopPositions, GenVars.structures);
                        workshopPositions.Add(placementPosition);
                    }
                }));

                //Abyss
                tasks.Insert(++currentFinalIndex, new PassLegacy("Abyss", (progress, config) =>
                {
                    progress.Message = "Disposing of Silva's remains";
                    Abyss.PlaceAbyss();
                }));

                //Sulphurous Sea (Part 2, after Abyss)
                tasks.Insert(++currentFinalIndex, new PassLegacy("Sulphur Sea 2", (progress, config) =>
                {
                    progress.Message = "Irradiating one of the oceans";
                    SulphurousSea.SulphurSeaGenerationAfterAbyss();
                }));

                //Roxcalibur
                tasks.Insert(++currentFinalIndex, new PassLegacy("Roxcalibur", (progress, config) =>
                {
                    progress.Message = "I wanna rock";
                    MiscWorldgenRoutines.PlaceRoxShrine();
                }));
            }
        }

        //An Astral Meteor always falls at the beginning of Hardmode.
        public override void ModifyHardmodeTasks(List<GenPass> tasks)
        {
            int announceIndex = tasks.FindIndex(match => match.Name == "Hardmode Announcement");

            // Insert the Astral biome generation right before the final hardmode announcement.
            tasks.Insert(announceIndex, new PassLegacy("AstralMeteor", (progress, config) =>
            {
                AstralBiome.PlaceAstralMeteor();
            }));
        }
        #endregion

        #region PostWorldGen
        public override void PostWorldGen()
        {
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null)
                {
                    //Checks which sheet a chest belongs to
                    bool isContainer1 = Main.tile[chest.x, chest.y].TileType == TileID.Containers;
                    bool isContainer2 = Main.tile[chest.x, chest.y].TileType == TileID.Containers2;

                    //Pre-1.4 chests
                    bool isBrownChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 0;
                    bool isGoldChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 36 || Main.tile[chest.x, chest.y].TileFrameX == 2*36); //Includes Locked Gold Chests
                    bool isMahoganyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 8 * 36;
                    bool isIvyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;
                    bool isIceChest = isContainer1 &&  Main.tile[chest.x, chest.y].TileFrameX == 11 * 36;
                    bool isMushroomChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 32 * 36;
                    bool isMarniteChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 50 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 51 * 36);

                    //1.4 chests
                    bool isDeadManChest = isContainer2 && Main.tile[chest.x, chest.y].TileFrameX == 4 * 36;
                    bool isSandstoneChest = isContainer2 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;

                    // Replace Suspicious Looking Eyes in Chests with random useful early game potions.
                    if (isBrownChest || isGoldChest || isMahoganyChest || isIvyChest || isIceChest || isMushroomChest || isMarniteChest || isDeadManChest || isSandstoneChest)
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.SuspiciousLookingEye)
                            {
                                // 60% chance of 3-5 Mining Potions
                                // 20% chance of 2-3 Builder's Potions
                                // 20% chance of 5-9 Shine Potions
                                float rng = WorldGen.genRand.NextFloat();
                                if (rng < 0.2f)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.ShinePotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(5, 10);
                                }
                                else if (rng < 0.4f)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.BuilderPotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(2, 4);
                                }
                                else
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.MiningPotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(3, 6);
                                }
                                break;
                            }
                        }
                    }

                    // Adds Desert Medallion to Sandstone Chests at a 20% chance
                    if (isSandstoneChest)
                    {
                        float rng = WorldGen.genRand.NextFloat();
                        if (rng < 0.2f)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (chest.item[inventoryIndex].IsAir)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<DesertMedallion>());
                                    chest.item[inventoryIndex].stack = 1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Save the set of ores that got generated
            OreTypes[0] = (ushort)GenVars.copperBar;
            OreTypes[1] = (ushort)GenVars.ironBar;
            OreTypes[2] = (ushort)GenVars.silverBar;
            OreTypes[3] = (ushort)GenVars.goldBar;
        }
        #endregion
    }
}
