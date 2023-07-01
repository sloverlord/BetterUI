using System.Collections;

using HarmonyLib;

using UnityEngine;
using UnityEngine.InputSystem;

using flanne;
using flanne.Core;
using MTDUI.Data;
using MTDUI.Controllers;

namespace BetterUI;

// Allow closing menus with escape
public static class CloseOnEscapePatch
{
    private static GameController gameController = null;
    private static bool paused = false;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PauseState), "Enter")]
    private static void PauseStateEnterPostPatch(PauseState __instance)
    {
        if (!paused)
            __instance.StartCoroutine(WaitForPause());
    }

    private static IEnumerator WaitForPause()
    {
        yield return new WaitForEndOfFrame();

        paused = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PauseState), "OnResume")]
    private static void PauseStateOnResumePostPatch()
    {
        paused = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameController), "Start")]
    private static void GameControllerStartPostPatch(GameController __instance)
    {
        gameController = __instance;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerController), "Update")]
    private static void PlayerControllerUpdatePostPatch()
    {
        if (gameController == null)
            return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return; // No keyboard connected.

        if (keyboard.escapeKey.wasPressedThisFrame && paused)
        {
            if (gameController.CurrentState is PauseState)
                gameController.pauseResumeButton.onClick.Invoke();
            else if (gameController.CurrentState is SynergyUIState)
                gameController.synergiesUIBackButton.onClick.Invoke();
            else if (gameController.CurrentState is ModOptionsPauseState) //MTDUI specific
                ModOptionsMenuController.PauseMenuBackButton?.onClick.Invoke();
            else if (gameController.CurrentState is ModOptionsPauseSubmenuState)
                ModOptionsMenuController.PauseSubmenuBackButton?.onClick.Invoke();
        }
    }
}
