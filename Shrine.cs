using StardewModdingAPI;
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
        public void ApplyTiles(IModHelper helper)
        {
            string tilesheetPath = helper.Content.GetActualAssetKey("temp2.png", ContentSource.ModFolder);

            GameLocation location = Game1.getLocationFromName("SkullCave");

            location.removeTile(2, 3, "Buildings");
            location.removeTile(2, 2, "Front");
            location.removeTile(2, 4, "Buildings");

            TileSheet tilesheet = new TileSheet(
                  id: "z_shrine_tilesheet",
                  map: location.map,
                  imageSource: tilesheetPath,
                  sheetSize: new xTile.Dimensions.Size(16, 48),
                  tileSize: new xTile.Dimensions.Size(16, 16)
               );
            location.map.AddTileSheet(tilesheet);
            location.map.LoadTileSheets(Game1.mapDisplayDevice);

            Layer layer = location.map.GetLayer("Front");
            Layer layer2 = location.map.GetLayer("Buildings");

            if(Game1.netWorldState.Value.SkullCavesDifficulty == 0)
            {
                layer.Tiles[2, 2] = new StaticTile(layer, tilesheet, BlendMode.Alpha, 0);
            }
            else
            {
                layer.Tiles[2, 2] = new StaticTile(layer, tilesheet, BlendMode.Alpha, 48);
            }
            
            layer.Tiles[2, 3] = new StaticTile(layer, tilesheet, BlendMode.Alpha, 16);
            layer2.Tiles[2, 4] = new StaticTile(layer2, tilesheet, BlendMode.Alpha, 32);

            int index = location.map.TileSheets.IndexOf(tilesheet);

            location.setMapTileIndex(2, 3, 0, "Buildings", index);
            location.setMapTile(2, 3, 0, "Buildings", "Nothing", index);
        }
    }
}
