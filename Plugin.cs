using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;

namespace BetterUI
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class BetterUI : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        internal static ManualLogSource Log;
        
        public static ConfigEntry<bool> configPowerupDisplay;
        public static ConfigEntry<bool> configDarknessDisplay;

        private void Awake()
        {
            configPowerupDisplay = Config.Bind("Display.Powerups", "DisplayPowerups", true, "Whether or not to show selected powerups when leveling up.");
            configDarknessDisplay = Config.Bind("Display.Darkness", "DisplayDarkness", true, "Whether or not to show the current darkness level.");

            Log = base.Logger;

            // Plugin startup logic
            Log.LogInfo("Better UI loaded.");

            Harmony.CreateAndPatchAll(typeof(DarknessDisplay), null);
            Harmony.CreateAndPatchAll(typeof(PowerupDisplay), null);
        }
    }
}
