using HarmonyLib;

using flanne.Core;

namespace BetterUI;

public static class PowerupDisplay
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(PowerupMenuState), "Enter")]
	private static void PowerupMenuStateEnterPrePatch(GameController ___owner)
	{
		if (BetterUI.configPowerupDisplay.Value)
			___owner.powerupListUI.Show();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PowerupMenuState), "EndLevelUpAnimationCR")]
	private static void EndLevelUpAnimationCRPostPatch(GameController ___owner)
	{
		___owner.powerupListUI.Hide();
	}

	[HarmonyPatch(typeof(GameController), "Start")]
	[HarmonyPostfix]
	static void StartPosftix(GameController __instance)
	{
		__instance.powerupListUI.transform.SetSiblingIndex(0);
	}
}
