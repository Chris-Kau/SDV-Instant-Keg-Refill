using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public sealed class ModConfig
{
    public KeybindList InstantKegKeybind { get; set; }

    public ModConfig()
    {
        this.InstantKegKeybind = KeybindList.Parse("LeftControl + Z");
    }
}