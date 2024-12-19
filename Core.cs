using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MooWeOweYa;

[BepInPlugin("dev.mooskyfish.MooWeOweYa", "Moosky's We Owe Ya", "1.0.0")]
[BepInProcess("Bug Fables.exe")]
public class CorePlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    public static string Version = MetadataHelper.GetMetadata(typeof(CorePlugin)).Version.ToString();
    private static Harmony s_harmony;

    public void Awake()
    {
        Logger = base.Logger;
        s_harmony = new Harmony("dev.mooskyfish.MooWeOweYa");
        if (Application.version.StartsWith("1.1"))
        {
            Logger.LogError("Unsupported Game Version");
            return;
        }
        try
        {
            s_harmony.PatchAll();
            Logger.LogInfo("Loaded");
        }
        catch
        {
            Logger.LogError("Unloading patches");
            s_harmony.UnpatchSelf();
            throw;
        }
        MooConfig.SetupConfig(Config);
    }
}
