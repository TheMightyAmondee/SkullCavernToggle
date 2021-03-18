using System;
using StardewModdingAPI;
using StardewValley;
using xTile.Layers;
using xTile.Dimensions;
using Harmony;
using xTile.Tiles;

namespace SkullCavernToggle
{
    public class Shrine 
    {
        private static IMonitor monitor;
        private static IModHelper helper;
        private static IManifest manifest;

        public static void GetHelpers(IMonitor monitor, IModHelper helper, IManifest manifest)
        {
            Shrine.monitor = monitor;
            Shrine.helper = helper;
            Shrine.manifest = manifest;
        }
        public static void Hook(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                postfix: new HarmonyMethod(typeof(Shrine), nameof(Shrine.performAction_PostFix))
                );
        }
        // Apply shrine tiles to map
        public static void ApplyTiles(IModHelper helper, bool multiplayerpatch = false)
        {
            // Get tilesheet pathway
            string tilesheetPath = helper.Content.GetActualAssetKey("assets\\snake_shrine.png", ContentSource.ModFolder);

            // Get skullcave location
            GameLocation location = Game1.getLocationFromName("SkullCave");

            // Get tilesheet from pathway
            TileSheet tilesheet = new TileSheet(
                  id: "z_shrine_tilesheet",
                  map: location.map,
                  imageSource: tilesheetPath,
                  sheetSize: new xTile.Dimensions.Size(16, 48),
                  tileSize: new xTile.Dimensions.Size(16, 16)
               );

            // Load tilesheet
            location.map.AddTileSheet(tilesheet);
            location.map.LoadTileSheets(Game1.mapDisplayDevice);

            // Get required layers
            Layer frontlayer = location.map.GetLayer("Front");
            Layer buildingslayer = location.map.GetLayer("Buildings");

            // Which snake head to use
            if (Game1.netWorldState.Value.SkullCavesDifficulty == 0)
            {
                if (multiplayerpatch == true)
                {
                    // Dangerous state, red eyes
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 48);
                }
                else
                {
                    // Normal state, yellow eyes
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 0);
                }

            }
            else
            {
                if (multiplayerpatch == true)
                {
                    // Normal state, yellow eyes
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 0);
                }
                else
                {
                    // Dangerous state, red eyes 
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 48);
                }
            }
            
            // Apply other tiles for shrine
            frontlayer.Tiles[2, 3] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 16);
            buildingslayer.Tiles[2, 4] = new StaticTile(buildingslayer, tilesheet, BlendMode.Alpha, 32);

            // Apply properties to middle tile, these don't do anything but show shrine can be interacted with
            int index = location.map.TileSheets.IndexOf(tilesheet);

            location.setMapTileIndex(2, 3, 0, "Buildings", index);
            location.setMapTile(2, 3, 0, "Buildings", "SnakeShrine", index);
        }

        public static void performAction_PostFix(GameLocation __instance, string action, Farmer who, Location tileLocation)
        {
            try
            {
                if (action != null && who.IsLocalPlayer && action == "SnakeShrine")
                {
                    GameLocation location = Game1.currentLocation;
                    if (Game1.netWorldState.Value.SkullCavesDifficulty > 0)
                    {                        
                        location.createQuestionDialogue("--Shrine Of Greater Challenge--^Summon an ancient magi-seal protection, returning the Skull Cavern to it's original state?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                // Normal
                                Game1.netWorldState.Value.SkullCavesDifficulty = 0;
                                Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to normal", null));
                                Game1.playSound("serpentDie");
                                Shrine.ApplyTiles(helper);

                                // Log new difficulty, difficulty will update after the clock ticks in multiplayer (10 in-game minutes)
                                monitor.Log("Skull Cavern Difficulty: " + Game1.netWorldState.Value.SkullCavesDifficulty, LogLevel.Trace);
                                Multiplayer message = new Multiplayer();
                                helper.Multiplayer.SendMessage(message, "Toggled", modIDs: new[] { manifest.UniqueID });
                            }
                        });
                    }

                    else
                    {
                        location.createQuestionDialogue("--Shrine Of Greater Challenge--^Dispel the ancient magi-seal of protection, allowing powerful monsters to enter the cavern?", location.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                // Dangerous
                                Game1.netWorldState.Value.SkullCavesDifficulty = 1;
                                Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to dangerous", null));
                                Game1.playSound("serpentDie");
                                Shrine.ApplyTiles(helper);

                                // Log new difficulty, difficulty will update after the clock ticks in multiplayer (10 in-game minutes)
                                monitor.Log("Skull Cavern Difficulty: " + Game1.netWorldState.Value.SkullCavesDifficulty, LogLevel.Trace);
                                Multiplayer message = new Multiplayer();
                                helper.Multiplayer.SendMessage(message, "Toggled", modIDs: new[] { manifest.UniqueID });
                            }
                        });
                    }
                }
            }
            catch(Exception e)
            {
                monitor.Log("Failed to patch", LogLevel.Error);
                throw new System.Exception("Failed to patch method", e);
            }
           
        }
    }
}
