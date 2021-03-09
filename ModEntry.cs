using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;
using xTile.Tiles;
using xTile.Layers;



namespace SkullCavernToggle
{
    public class ModEntry
        : Mod
    {

        private ModConfig config;

        public static Shrine Shrine { get; private set; } = new Shrine();

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.Toggle;
            helper.Events.Player.Warped += this.OnWarp;

            this.config = helper.ReadConfig<ModConfig>();
        }

        // Apply shrine tiles when player is in the correct location
        private void OnWarp(object sender, WarpedEventArgs e)
        {
            if(e.NewLocation.NameOrUniqueName == "SkullCave" && ShowShrine() == true)
            {
                Shrine.ApplyTiles(this.Helper);
            }            
        }


        // Are the toggle conditions met?
        private bool ShouldToggle()
        {
            var order = Game1.player.team.completedSpecialOrders;
         
            if(Game1.player.team.SpecialOrderActive("QiChallenge10") == true)
            {
                return false;
            }

            if(this.config.MustCompleteQuest == true)
            {
                foreach (string soid in new List<string>(order.Keys))
                {
                    if (soid.Contains("QiChallenge10") == true)
                    {
                        return true;
                    }
                }
            }

            else
            {
                return true;
            }

            return false;
        }

        // Should the shrine be added to the map?
        private bool ShowShrine()
        {
            // Is shrine toggle being used?
            if(this.config.ShrineToggle == true)
            {
                // Yes, check if the conditions are met

                // Get completed orders
                var order = Game1.player.team.completedSpecialOrders;

                // Must the quest be completed first?
                if (this.config.MustCompleteQuest == true)
                {
                    // Yes, is it complete?

                    // Iterate through completed orders
                    foreach (string soid in new List<string>(order.Keys))
                    {
                        
                        if (soid.Contains("QiChallenge10") == true)
                        {
                            // Yes, order complete, add shrine
                            return true;
                        }

                        else
                        {
                            // No, order not complete, don't add shrine
                            return false;
                        }
                    }
                }

                else
                {
                    // No, quest is not a condition, add the shrine
                    return true;
                }
                
            }

            // No, key toggle is used, shrine should not be placed

            return false;            
        }

        // Toggle difficulty after confirmation
        private void ShrineMenu(int difficulty)
        {
            // Toggle accordingly
            if(difficulty > 0)
            {
                // Normal
                Game1.netWorldState.Value.SkullCavesDifficulty = 0;

            }
            else
            {
                // Dangerous
                Game1.netWorldState.Value.SkullCavesDifficulty = 1;
            }

            // Fix shrine appearance for new difficulty
            Shrine.ApplyTiles(this.Helper);
            // Exit confirmation box
            Game1.exitActiveMenu();
            // Play sound cue
            Game1.playSound("serpentDie");
            // Show message to confirm toggle
            Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled", null));
        }

        // Toggle difficulty using button
        private void Toggle(object sender, ButtonPressedEventArgs e)
        {
            if(this.config.ToggleDifficulty.JustPressed() == true && this.config.ShrineToggle == false)
            {
                // Has correct button been pushed, conditions for toggle been met and world is ready?
                if (ShouldToggle() == true && Context.IsWorldReady == true)
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

                else if (ShouldToggle() == false && Context.IsWorldReady == true)
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

            // Using shrine
            else if(e.Button == SButton.MouseRight && Game1.currentLocation.NameOrUniqueName == "SkullCave" && Game1.player.canMove == true && ShowShrine() == true)
            {
                // If player clicks this location (shrine) display the appropriate response
                if((e.Cursor.GrabTile.X == 2 && e.Cursor.GrabTile.Y == 2) || (e.Cursor.GrabTile.X == 2 && e.Cursor.GrabTile.Y == 3) || (e.Cursor.GrabTile.X == 2 && e.Cursor.GrabTile.Y == 4))
                {
                    if(ShouldToggle() == true)
                    {
                        if(Game1.netWorldState.Value.SkullCavesDifficulty > 0)
                        {
                            Game1.activeClickableMenu = new ConfirmationDialog("Toggle Skull Cavern to normal?", (ConfirmationDialog.behavior)(_ => ShrineMenu(1)), null);
                        }
                        else
                        {
                            Game1.activeClickableMenu = new ConfirmationDialog("Toggle Skull Cavern to dangerous?", (ConfirmationDialog.behavior)(_ => ShrineMenu(0)), null);
                        }                       
                    }

                    else if(ShouldToggle() == false && Game1.player.team.SpecialOrderActive("QiChallenge10") == true)
                    {
                        Game1.activeClickableMenu = new DialogueBox("Looks like Skull Cavern Invasion is active... I can't toggle right now.");
                    }

                    else
                    {
                        Game1.activeClickableMenu = new DialogueBox("You haven't completed Skull Cavern Invasion... I don't think you can handle this yet.");
                    }
                    
                }
            }
            
        }
    }
}
