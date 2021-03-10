﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
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
