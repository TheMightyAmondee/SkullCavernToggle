using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;
using xTile.Tiles;
using xTile.Layers;


namespace SkullCavernToggle
{
    internal class ShrineMenu
        : GameLocation
    {
        public void Generatemenu()
        {			
            base.createQuestionDialogue(Game1.netWorldState.Value.SkullCavesDifficulty > 0 ? Game1.content.LoadString("Strings\\Locations:ChallengeShrine_AlreadyHard") : Game1.content.LoadString("Strings\\Locations:ChallengeShrine_NotYetHard"), base.createYesNoResponses(), "It is done");
		}
    }
}
