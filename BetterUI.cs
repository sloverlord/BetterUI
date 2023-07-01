using BepInEx;
using BepInEx.Configuration;

using UtillI;
using MTDUI;
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

    #region statsCustomConfigEntries
    public static ConfigEntry<bool> statsCustomHasDamage;
    public static ConfigEntry<bool> statsCustomHasReloadTime;
    public static ConfigEntry<bool> statsCustomHasFireRate;
    public static ConfigEntry<bool> statsCustomHasPierce;
    public static ConfigEntry<bool> statsCustomHasBounce;
    public static ConfigEntry<bool> statsCustomHasSpread;
    // public static ConfigEntry<bool> statsCustomHasMaxAmmo;
    public static ConfigEntry<bool> statsCustomHasMoveSpeed;
    public static ConfigEntry<bool> statsCustomHasWalkSpeed;
    public static ConfigEntry<bool> statsCustomHasSummonDamage;
    public static ConfigEntry<bool> statsCustomHasSummonAttackSpeed;
    public static ConfigEntry<bool> statsCustomHasProjectileNumber;
    public static ConfigEntry<bool> statsCustomHasProjectileSpeed;
    public static ConfigEntry<bool> statsCustomHasProjectileSize;
    public static ConfigEntry<bool> statsCustomHasKnockback;
    public static ConfigEntry<bool> statsCustomHasCharacterSize;
    public static ConfigEntry<bool> statsCustomHasPickupRange;
    public static ConfigEntry<bool> statsCustomHasVisionRange;
    public static ConfigEntry<bool> statsCustomHasDodge;
    public static ConfigEntry<bool> statsCustomHasDodgeCap;
    #endregion

    private void Awake()
    {
        configPowerupDisplay = Config.Bind(
            "Display.Powerups",
            "PowerupsDisplayOnLevelUp",
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
            StatSelection.Base,
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

        statsCustomHasDamage = Config.Bind("Display.Stats.CustomSelection", "Damage", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasReloadTime = Config.Bind("Display.Stats.CustomSelection", "ReloadTime", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasFireRate = Config.Bind("Display.Stats.CustomSelection", "FireRate", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasPierce = Config.Bind("Display.Stats.CustomSelection", "Pierce", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasBounce = Config.Bind("Display.Stats.CustomSelection", "Bounce", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasSpread = Config.Bind("Display.Stats.CustomSelection", "Spread", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasMoveSpeed = Config.Bind("Display.Stats.CustomSelection", "MoveSpeed", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasWalkSpeed = Config.Bind("Display.Stats.CustomSelection", "WalkSpeed", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasSummonDamage = Config.Bind("Display.Stats.CustomSelection", "SummonDamage", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasSummonAttackSpeed = Config.Bind("Display.Stats.CustomSelection", "SummonAttackSpeed", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasProjectileNumber = Config.Bind("Display.Stats.CustomSelection", "ProjectileNumber", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasProjectileSpeed = Config.Bind("Display.Stats.CustomSelection", "ProjectileSpeed", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasProjectileSize = Config.Bind("Display.Stats.CustomSelection", "ProjectileSize", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasKnockback = Config.Bind("Display.Stats.CustomSelection", "Knockback", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasCharacterSize = Config.Bind("Display.Stats.CustomSelection", "CharacterSize", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasPickupRange = Config.Bind("Display.Stats.CustomSelection", "PickupRange", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasVisionRange = Config.Bind("Display.Stats.CustomSelection", "VisionRange", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasDodge = Config.Bind("Display.Stats.CustomSelection", "Dodge", true, "Whether or not to show the stat on Custom StatSelection");
        statsCustomHasDodgeCap = Config.Bind("Display.Stats.CustomSelection", "DodgeCap", true, "Whether or not to show the stat on Custom StatSelection");

        ModOptions.Register(configPowerupDisplay);
        ModOptions.Register(configStatsPanel);
        ModOptions.Register(configStatsSelection);

        UtillIRegister.Register(new StatsPanel());
        harmony.PatchAll(typeof(PowerupDisplayPatch));
        harmony.PatchAll(typeof(CloseOnEscapePatch));
        Logger.LogInfo("Better UI loaded.");
    }
}
