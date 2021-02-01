using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Network;
using StardewValley;
using System.Collections.Generic;

namespace SkullCavernToggle
{
    public class ModEntry
        : Mod
    {
        //private readonly NetWorldState state = new NetWorldState();

        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.Toggle;
            this.config = helper.ReadConfig<ModConfig>();
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
            if (e.Button == this.config.ToggleDifficulty && ShouldToggle() == true && Context.IsWorldReady == true)
            {
                // Yes, toggle difficulty

                if (Game1.netWorldState.Value.SkullCavesDifficulty == 1)
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

            else if(e.Button == SButton.Z && ShouldToggle() == false && Context.IsWorldReady == true)
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
