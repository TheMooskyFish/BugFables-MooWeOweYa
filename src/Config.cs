using BepInEx.Configuration;
namespace MooWeOweYa;

public static class MooConfig
{
    private static ConfigEntry<bool> s_hologram;
    internal static ConfigEntry<bool> ToggleMod;

    internal static ConfigEntry<bool> DisableAll;
    internal static ConfigEntry<bool> Maki;
    internal static ConfigEntry<bool> GenEri;
    internal static ConfigEntry<bool> Mun;
    internal static ConfigEntry<bool> Levi;
    internal static ConfigEntry<bool> Celia;
    internal static ConfigEntry<bool> AntCaptain;
    internal static ConfigEntry<bool> Madeleine;
    internal static ConfigEntry<bool> Zasp;
    
    internal static ConfigEntry<bool> MrTester;

    public static bool Hologram
    {
        get
        {
            return !ToggleMod.Value || s_hologram.Value;
        }
    }

    private const string Partners = "Partners Settings";

    internal static void SetupConfig(ConfigFile config)
    {
        s_hologram = config.Bind("!Settings!", "Hologram Toggle", true, "Disables Hologram Effect (Requires Mod Toggle)");
        ToggleMod = config.Bind("!Settings!", "!Mod Toggle", true, "");
        
        DisableAll = config.Bind("!Settings!", "Disable all partners", false, "");
        Maki = config.Bind(Partners, "Maki", true, "");
        GenEri = config.Bind(Partners, "Gen and Eri", true, "");
        Mun = config.Bind(Partners, "Mun", true, "");
        Levi = config.Bind(Partners, "Levi", true, "");
        Celia = config.Bind(Partners, "Celia", true, "");
        AntCaptain = config.Bind(Partners, "Ant Lieutenant", true, "");
        Madeleine = config.Bind(Partners, "Madeleine", true, "");
        Zasp = config.Bind(Partners, "Zasp", true, "");
        
        MrTester = config.Bind("Custom Partners", "Mr Tester", true, "");
    }
}
