using StardewModdingAPI.Utilities;

namespace SkullCavernToggle
{
    internal class ModConfig
    {      
        public bool ShrineToggle { get; set; } = true;
        public KeybindList ToggleDifficulty { get; set; } = KeybindList.Parse("Z");
        public bool MustCompleteQuest { get; set; } = true;
    }
}
