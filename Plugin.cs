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

        public static ManualLogSource Log;
        
        public static ConfigEntry<bool> configPowerupDisplay;
        public static ConfigEntry<bool> configInfoDisplay;

        // config entries for info display colors
        public static ConfigEntry<string> configheaderColor;
        public static ConfigEntry<string> configLabelColor;

        private void Awake()
        {
            configPowerupDisplay = Config.Bind("Display.Powerups", "DisplayPowerups", true, "Whether or not to show selected powerups when leveling up.");
            configInfoDisplay = Config.Bind("Display.Info", "DisplayInfo", true, "Whether or not to show current stats in pause menu.");
            configheaderColor = Config.Bind("Header.Color", "HeaderColor", "orange", "What color to make the header for the stat display.");
            configLabelColor = Config.Bind("Label.Color", "LabelColor", "white", "What color to make the labels for the stat display.");

            Log = base.Logger;

            // Plugin startup logic
            Log.LogInfo("Better UI loaded.");

            Harmony.CreateAndPatchAll(typeof(InfoDisplay), null);
            Harmony.CreateAndPatchAll(typeof(PowerupDisplay), null);
        }
    }
}
