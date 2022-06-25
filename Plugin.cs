using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using flanne.Core;

namespace BetterUI
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class BetterUI : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        internal static ManualLogSource Log;
        
        private void Awake()
        {
            Log = base.Logger;

            // Plugin startup logic
            Log.LogInfo("Better UI loaded.");

            Harmony.CreateAndPatchAll(typeof(BetterUI), null);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerupMenuState), "PlayLevelUpAnimationCR")]
        private static bool PlayLevelUpAnimationCRPostPatch(ref GameController ___owner){
            ___owner.powerupListUI.Show();

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerupMenuState), "EndLevelUpAnimationCR")]
        private static void EndLevelUpAnimationCRPostPatch(ref GameController ___owner){
            ___owner.powerupListUI.Hide();
        }
    }
}
