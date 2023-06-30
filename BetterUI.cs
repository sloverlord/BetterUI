using BepInEx;
using BepInEx.Configuration;

using UtillI;
using HarmonyLib;

namespace BetterUI;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class BetterUI : BaseUnityPlugin
{
    private static readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

    public static ConfigEntry<bool> configPowerupDisplay;
    public static ConfigEntry<DisplayRule> configStatsPanel;
    public static ConfigEntry<StatSelection> configStatsSelection;

    // config entries for info display colors
    public static ConfigEntry<string> configHeaderColor;
    public static ConfigEntry<string> configLabelColor;

    private void Awake()
    {
        configPowerupDisplay = Config.Bind(
            "Display.Powerups",
            "DisplayPowerups",
            true,
            "Whether or not to show selected powerups when leveling up.");

        configStatsPanel = Config.Bind(
            "Display.Stats",
            "StatsPanelDisplayRule",
            DisplayRule.PauseOnly,
            "If and when to show the current stats");

        configStatsSelection = Config.Bind(
            "Display.Stats",
            "StatsSelection",
            StatSelection.Default,
            "Which stats to show in the panel");

        configHeaderColor = Config.Bind(
            "Display.Stats.Color",
            "HeaderColor",
            "orange",
            "What color to make the header for the stat display.");

        configLabelColor = Config.Bind(
            "Display.Stats.Color",
            "LabelColor",
            "white",
            "What color to make the labels for the stat display.");

        UtillIRegister.Register(new StatsPanel());
        harmony.PatchAll(typeof(PowerupDisplayPatch));
        harmony.PatchAll(typeof(CloseOnEscapePatch));
        Logger.LogInfo("Better UI loaded.");
    }
}
