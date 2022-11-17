using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

namespace BetterUI;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class BetterUI: BaseUnityPlugin
{
	public static readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

	public static Harmony menuCloserHarmony = new Harmony(PluginInfo.PLUGIN_GUID);

	public static ManualLogSource Log;

	public static ConfigEntry<bool> configPowerupDisplay;
	public static ConfigEntry<bool> configInfoDisplay;

	// config entries for info display colors
	public static ConfigEntry<string> configheaderColor;
	public static ConfigEntry<string> configLabelColor;

	public static ConfigEntry<int> configLastDarkness;

	private void Awake()
	{
		configPowerupDisplay = Config.Bind("Display.Powerups", "DisplayPowerups", true, "Whether or not to show selected powerups when leveling up.");
		configInfoDisplay = Config.Bind("Display.Info", "DisplayInfo", true, "Whether or not to show current stats in pause menu.");
		configheaderColor = Config.Bind("Header.Color", "HeaderColor", "orange", "What color to make the header for the stat display.");
		configLabelColor = Config.Bind("Label.Color", "LabelColor", "white", "What color to make the labels for the stat display.");

		configLastDarkness = Config.Bind("Last.Darkness", "LastDarkness", 1, "What the darkness level should be on return to character select. (-1 to disable)");

		Log = base.Logger;

		// Plugin startup logic
		Log.LogInfo("Better UI loaded.");

		Harmony.CreateAndPatchAll(typeof(InfoDisplay), null);
		Harmony.CreateAndPatchAll(typeof(PowerupDisplay), null);
		Harmony.CreateAndPatchAll(typeof(LastDarkness), null);
		Harmony.CreateAndPatchAll(typeof(CustomInputs), null);
	}
}
