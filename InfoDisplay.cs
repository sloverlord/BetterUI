using HarmonyLib;
using flanne.Core;
using flanne;
using flanne.UI;
using TMPro;
using UnityEngine;
using System;

namespace BetterUI;


class InfoDisplay : MonoBehaviour
{
    private static Panel statsPanel = null;

    private static GameController _gameController;
    
    [HarmonyPatch(typeof(GameController), "Start")]
    static void Prefix(GameController __instance)
    {
        _gameController = __instance;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PauseState), "Enter")]
    private static void PauseStateEnterPostPatch(ref GameController ___owner)
    {
        if(BetterUI.configInfoDisplay.Value) {
            // copy the controls display so we don't have to manually add a bunch of tweening stuff
            var pausedTextObject = Instantiate(___owner.hud.transform.parent.Find("ControlsDisplay"), ___owner.hud.transform.parent);
            var rect = pausedTextObject.GetComponent<RectTransform>();
            var canvasGroup = pausedTextObject.GetComponent<CanvasGroup>();

            // this panel will be useful later for adding a stats display
            // for now we can just handle darkness level
            // todo: add an option to always display instead of just on pause
            statsPanel = pausedTextObject.GetComponent<Panel>();

            // some minor changes to make tweening work properly on first go
            Traverse.Create(pausedTextObject.GetComponent<AutoShowPanel>()).Field("startTime").SetValue(0f);
            Traverse.Create(statsPanel).Field("canvasGroup").SetValue(canvasGroup);
            canvasGroup.alpha = 0f;

            var allTextObjects = pausedTextObject.GetComponentsInChildren<TextMeshProUGUI>();

            for (var i = 0; i < allTextObjects.Length; i++)
            {
                var text = allTextObjects[i];
                text.alignment = TextAlignmentOptions.BottomRight;
                
                if (i == 0)
                {
                    PlayerController player = _gameController.player.GetComponent<PlayerController>();
                    StatsHolder stats = player.stats;

                    var difficultyText = allTextObjects[i];
                    difficultyText.text = "";

                    difficultyText.text += $"{Loadout.CharacterSelection.nameString}" + System.Environment.NewLine;
                    // *should* be localized in the same way the game does internal
                    difficultyText.text += $"{LocalizationSystem.GetLocalizedValue("difficulty_label")} {Loadout.difficultyLevel}" + System.Environment.NewLine;
                    difficultyText.text += $"Damage: {Math.Round(stats[StatType.BulletDamage].Modify(player.gun.gunData.damage), 0)}" + System.Environment.NewLine;
                    difficultyText.text += $"Projectiles: {Math.Round(stats[StatType.Projectiles].Modify(player.gun.gunData.numOfProjectiles), 0)}" + System.Environment.NewLine;
                    difficultyText.text += $"Reload Time: {Math.Round(stats[StatType.ReloadRate].Modify(player.gun.gunData.reloadDuration), 1)}" + System.Environment.NewLine;
                    difficultyText.text += $"Fire Rate: {Math.Round(1/stats[StatType.FireRate].ModifyInverse(player.gun.gunData.shotCooldown), 1)}" + System.Environment.NewLine;
                    difficultyText.text += $"Max Ammo: {Math.Round(stats[StatType.MaxAmmo].Modify(player.gun.gunData.maxAmmo), 0)}" + System.Environment.NewLine;
                    difficultyText.text += $"Pierce: {Math.Round(stats[StatType.Piercing].Modify(player.gun.gunData.piercing), 0)}" + System.Environment.NewLine;
                    difficultyText.text += $"Bounce: {Math.Round(stats[StatType.Bounce].Modify(player.gun.gunData.bounce), 0)}" + System.Environment.NewLine;
                }
                text.gameObject.SetActive(i == 0);
            }

            // redo positioning so it comes up from the bottom left
            rect.anchorMin = new Vector2(.8f, 0f);
            rect.anchorMax = new Vector2(.8f, 0f);
            rect.anchoredPosition = new Vector2(50f+10f, 16f);
            rect.sizeDelta = new Vector2(100, 30f);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CombatState), "Enter")]
    private static void CombatStateEnterPostPatch(ref GameController ___owner)
    {
        // we can't use PauseState.Exit because that will also disable things when going to settings
        if(statsPanel != null) statsPanel.Hide();
    }
} 