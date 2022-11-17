using System.Reflection;

using HarmonyLib;

using UnityEngine;

using flanne;
using flanne.Core;
using flanne.UI;

namespace BetterUI;

class LastDarkness: MonoBehaviour
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(DifficultyController), "Init")]
	private static void InitPostFix(DifficultyController __instance)
	{
		MethodInfo difficultyMethod = __instance.GetType().GetMethod("SetDifficulty", BindingFlags.NonPublic | BindingFlags.Instance);
		difficultyMethod.Invoke(__instance, new object[] { BetterUI.configLastDarkness.Value });
	}

	// updates difficulty on change in menu
	[HarmonyPostfix]
	[HarmonyPatch(typeof(DifficultyController), "IncreaseDifficulty")]
	private static void IncreaseDifficultyPostFix(DifficultyController __instance)
	{
		BetterUI.configLastDarkness.Value = __instance.difficulty;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(DifficultyController), "DecreaseDifficulty")]
	private static void DecreaseDifficultyPostFix(DifficultyController __instance)
	{
		BetterUI.configLastDarkness.Value = __instance.difficulty;
	}

	// increases to next difficulty on win and return to menu
	[HarmonyPostfix]
	[HarmonyPatch(typeof(PlayerSurvivedState), "OnClickQuit")]
	private static void OnClickQuitPostFix(DifficultyController __instance)
	{
		if (BetterUI.configLastDarkness.Value < SaveSystem.data.difficultyUnlocked)
			BetterUI.configLastDarkness.Value += 1;
	}
}
