using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using xTile.ObjectModel;
using xTile.Tiles;
using xTile.Layers;


namespace SkullCavernToggle
{
    public class ModEntry
        : Mod
    {

        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.Toggle;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            this.config = helper.ReadConfig<ModConfig>();
        }


        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            string tilesheetPath = this.Helper.Content.GetActualAssetKey("temp.png", ContentSource.ModFolder);
          
            GameLocation location = Game1.getLocationFromName("SkullCave");
          
            location.removeTile(2, 3, "Front");
            //location.removeTile(2, 4, "Buildings");

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
            layer.Tiles[2, 3] = new StaticTile(layer, tilesheet, BlendMode.Alpha, 0);

            int index = location.map.TileSheets.IndexOf(tilesheet);

            location.setMapTileIndex(2, 3, 0, "Buildings", index);
            location.setMapTile(2, 3, 0, "Buildings", "Dialogue hello", index);
        }


        // Don't toggle if special order hasn't been completed or is active
        private bool ShouldToggle()
        {
            var order = Game1.player.team.completedSpecialOrders;
         
            if(Game1.player.team.SpecialOrderActive("QiChallenge10") == true)
            {
                return false;
            }

            foreach (string soid in new List<string>(order.Keys))
            {
                if (soid.Contains("QiChallenge10") == true)
                {
                    return true;
                }
            }

            return false;
        }

        // Toggle difficulty
        private void Toggle(object sender, ButtonPressedEventArgs e)
        {
            // Has correct button been pushed, conditions for toggle been met and world is ready?
            if (this.config.ToggleDifficulty.JustPressed() == true && ShouldToggle() == true && Context.IsWorldReady == true)
            {
                // Yes, toggle difficulty

                if (Game1.netWorldState.Value.SkullCavesDifficulty > 0)
                {
                    Game1.netWorldState.Value.SkullCavesDifficulty = 0;
                    Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to normal", null));

                }
                else
                {
                    Game1.netWorldState.Value.SkullCavesDifficulty = 1;
                    Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to hard", null));

                }

            }

            else if(this.config.ToggleDifficulty.JustPressed() == true && ShouldToggle() == false && Context.IsWorldReady == true)
            {
                // No, display message to say difficulty can't be toggled

                if (Game1.player.team.SpecialOrderActive("QiChallenge10") == true)
                {
                    Game1.addHUDMessage(new HUDMessage("Skull Cavern Invasion is active", 3));
                }

                else
                {
                    Game1.addHUDMessage(new HUDMessage("Skull Cavern Invasion not completed", 3));
                }               
            }
        }
    }
}
