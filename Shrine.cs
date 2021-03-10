﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;
using xTile.Tiles;
using xTile.Layers;

namespace SkullCavernToggle
{
    public class Shrine
    {
        // Apply shrine tiles to map
        public void ApplyTiles(IModHelper helper)
        {
            // Get tilesheet pathway
            string tilesheetPath = helper.Content.GetActualAssetKey("assets\\temp2.png", ContentSource.ModFolder);

            // Get skullcave location
            GameLocation location = Game1.getLocationFromName("SkullCave");

            // Remove tiles where shrine will go
            location.removeTile(2, 3, "Buildings");
            location.removeTile(2, 2, "Front");
            location.removeTile(2, 4, "Buildings");

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
                // Normal state, yellow eyes
                frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 0);
            }
            else
            {
                // Dangerous state, red eyes
                frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 48);
            }
            
            // Apply other tiles for shrine
            frontlayer.Tiles[2, 3] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 16);
            buildingslayer.Tiles[2, 4] = new StaticTile(buildingslayer, tilesheet, BlendMode.Alpha, 32);

            // Apply properties to middle tile, these don't do anything but show shrine can be interacted with
            int index = location.map.TileSheets.IndexOf(tilesheet);

            location.setMapTileIndex(2, 3, 0, "Buildings", index);
            location.setMapTile(2, 3, 0, "Buildings", "Nothing", index);
        }
    }
}