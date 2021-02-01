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
        private readonly NetWorldState state = new NetWorldState();

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
            if (e.Button == this.config.ToggleDifficulty && ShouldToggle() == true && Context.IsWorldReady == true)
            {
                if(state.SkullCavesDifficulty == 1)
                {
                    state.SkullCavesDifficulty = 0;
                    this.Monitor.Log($"Difficulty:{state.skullCavesDifficulty}");
                    Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to normal", null));
                }
                else
                {
                    state.SkullCavesDifficulty = 1;
                    this.Monitor.Log($"Difficulty:{state.skullCavesDifficulty}");
                    Game1.addHUDMessage(new HUDMessage("Skull Cavern toggled to hard", null));
                }

                
                Game1.netWorldState.Set(state);
            }
            else if(e.Button == SButton.Z && ShouldToggle() == false && Context.IsWorldReady == true)
            {
                Game1.addHUDMessage(new HUDMessage("Skull Cavern Difficulty can't be toggled now", 3));
            }
        }
    }
}
