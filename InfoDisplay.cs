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
    private static Panel statLabelsPanel = null;

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
            var statTextObject = Instantiate(___owner.hud.transform.parent.Find("ControlsDisplay"), ___owner.hud.transform.parent);
            var statsRect = statTextObject.GetComponent<RectTransform>();
            var statsCanvasGroup = statTextObject.GetComponent<CanvasGroup>();

            var statLabelsTextObject = Instantiate(___owner.hud.transform.parent.Find("ControlsDisplay"), ___owner.hud.transform.parent);
            var statLabelsRect = statLabelsTextObject.GetComponent<RectTransform>();
            var statLabelsCanvasGroup = statLabelsTextObject.GetComponent<CanvasGroup>();

            // redo positioning so it comes up from the bottom left
            statsRect.anchorMin = new Vector2(.8f, 0f);
            statsRect.anchorMax = new Vector2(.8f, 0f);
            statsRect.anchoredPosition = new Vector2(50f+10f, 16f);
            statsRect.sizeDelta = new Vector2(100, 30f);

            statLabelsRect.anchorMin = new Vector2(.85f, 0f);
            statLabelsRect.anchorMax = new Vector2(.85f, 0f);
            statLabelsRect.anchoredPosition = new Vector2(50f+10f, 16f);
            statLabelsRect.sizeDelta = new Vector2(100, 30f);

            // this panel will be useful later for adding a stats display
            // for now we can just handle darkness level
            // todo: add an option to always display instead of just on pause
            statsPanel = statTextObject.GetComponent<Panel>();
            statLabelsPanel = statLabelsTextObject.GetComponent<Panel>();

            // some minor changes to make tweening work properly on first go
            Traverse.Create(statTextObject.GetComponent<AutoShowPanel>()).Field("startTime").SetValue(0f);
            Traverse.Create(statsPanel).Field("canvasGroup").SetValue(statsCanvasGroup);
            statsCanvasGroup.alpha = 0f;

            Traverse.Create(statLabelsTextObject.GetComponent<AutoShowPanel>()).Field("startTime").SetValue(0f);
            Traverse.Create(statLabelsPanel).Field("canvasGroup").SetValue(statLabelsCanvasGroup);
            statLabelsCanvasGroup.alpha = 0f;

            var statTextMesh = statTextObject.GetComponentsInChildren<TextMeshProUGUI>();
            var statLabelsTextMesh = statLabelsTextObject.GetComponentsInChildren<TextMeshProUGUI>();

            PlayerController player = _gameController.player.GetComponent<PlayerController>();
            StatsHolder stats = player.stats;

            string statHeader = $"<line-height=125%><color={BetterUI.configheaderColor.Value}>";
            statHeader += $"<align=\"left\">{Loadout.CharacterSelection.nameString}\n";
            statHeader += $"<align=\"left\">{player.gun.gunData.nameString}\n";
            // *should* be localized in the same way the game does internal
            statHeader += $"<align=\"left\">{LocalizationSystem.GetLocalizedValue("difficulty_label")} {Loadout.difficultyLevel} \n</color>";

            string statLabels = $"<line-height=125%><color={BetterUI.configLabelColor.Value}>";
            statLabels += $"<align=\"left\">Damage:\n";
            statLabels += $"<align=\"left\">Projectiles:\n";
            statLabels += $"<align=\"left\">Reload Time:\n";
            statLabels += $"<align=\"left\">Fire Rate:\n";
            statLabels += $"<align=\"left\">Max Ammo:\n";
            statLabels += $"<align=\"left\">Pierce:\n";
            statLabels += $"<align=\"left\">Bounce:\n</color>";
            statLabelsTextMesh[0].text = statHeader + statLabels;

            string statValues = "<line-height=125%>";
            statValues += $"<align=\"right\">{Math.Round(stats[StatType.BulletDamage].Modify(player.gun.gunData.damage), 0)} \n";
            statValues += $"<align=\"right\"> {Math.Round(stats[StatType.Projectiles].Modify(player.gun.gunData.numOfProjectiles), 0)} \n";
            statValues += $"<align=\"right\"> {Math.Round(stats[StatType.ReloadRate].Modify(player.gun.gunData.reloadDuration), 1)} \n";
            statValues += $"<align=\"right\"> {Math.Round(1/stats[StatType.FireRate].ModifyInverse(player.gun.gunData.shotCooldown), 1)} \n";
            statValues += $"<align=\"right\"> {Math.Round(stats[StatType.MaxAmmo].Modify(player.gun.gunData.maxAmmo), 0)} \n";
            statValues += $"<align=\"right\"> {Math.Round(stats[StatType.Piercing].Modify(player.gun.gunData.piercing), 0)} \n";
            statValues += $"<align=\"right\"> {Math.Round(stats[StatType.Bounce].Modify(player.gun.gunData.bounce), 0)} \n";
            statTextMesh[0].text = statValues;

            for (var i = 0; i < statTextMesh.Length; i++)
            {
                var statText = statTextMesh[i];
                statText.alignment = TextAlignmentOptions.BottomRight;
                statText.gameObject.SetActive(i <= 0);
                
                var statLabelsText = statLabelsTextMesh[i];
                statLabelsText.alignment = TextAlignmentOptions.BottomRight;
                statLabelsText.gameObject.SetActive(i <= 0);
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CombatState), "Enter")]
    private static void CombatStateEnterPostPatch(ref GameController ___owner)
    {
        // we can't use PauseState.Exit because that will also disable things when going to settings
        if(statsPanel != null) statsPanel.Hide();
        if(statLabelsPanel != null) statLabelsPanel.Hide();
    }
}