using System;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using flanne;
using flanne.Core;
using flanne.UI;

namespace BetterUI;

public static class InfoDisplay
{
	private static StatsPanel statsPanel = null;

	[HarmonyPatch(typeof(GameController), "Start")]
	static void Prefix(GameController __instance)
	{
		GameObject panelObj = new("StatsPanel", typeof(RectTransform));

		panelObj.transform.SetParent(__instance.hud.transform.parent);
		panelObj.transform.localScale = Vector3.one;

		RectTransform transform = panelObj.transform as RectTransform;
		transform.anchorMin = new Vector2(1f, 0f);
		transform.anchorMax = new Vector2(1f, 0f);

		// Position the panel so that it shows up on the bottom right of the screen.
		transform.anchoredPosition = new(-(transform.sizeDelta.x / 4f), 0f);

		// Avoid calling Awake when we add the component below
		panelObj.SetActive(false);

		var canvasGroup = panelObj.AddComponent<CanvasGroup>();
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
		canvasGroup.ignoreParentGroups = false;

		var layout = panelObj.AddComponent<HorizontalLayoutGroup>();
		layout.useGUILayout = true;
		layout.childAlignment = TextAnchor.LowerRight;

		statsPanel = panelObj.AddComponent<StatsPanel>();
		statsPanel.Init(__instance);

		// Now call Awake
		panelObj.SetActive(true);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PauseState), "Enter")]
	private static void PauseStateEnterPostPatch()
	{
		if(BetterUI.configInfoDisplay.Value)
			statsPanel.Show();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PowerupMenuState), "Enter")]
	private static void PowerupMenuStateEnterPostPatch()
	{
		if(BetterUI.configInfoDisplay.Value)
			statsPanel.Show();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(DevilDealState), "Enter")]
	private static void DevilDealStateEnterPostPatch()
	{
		if(BetterUI.configInfoDisplay.Value)
			statsPanel.Show();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(ChestState), "Enter")]
	private static void ChestStateEnterPostPatch()
	{
		if(BetterUI.configInfoDisplay.Value)
			statsPanel.Show();
	}

	// shows final stats
	[HarmonyPostfix]
	[HarmonyPatch(typeof(PlayerSurvivedState), "Enter")]
	private static void PlayerSurvivedStateEnterPostFix()
	{
		if(BetterUI.configInfoDisplay.Value)
			statsPanel.Show();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(CombatState), "Enter")]
	private static void CombatStateEnterPostPatch()
	{
		statsPanel.Hide();
	}
}

public class StatsPanel: MonoBehaviour
{
	private GameController owner = null;
	private PlayerController player = null;

	private Panel left = null, right = null;

	public void Init(GameController owner)
	{
		this.owner = owner;
	}

	void Awake()
	{
		Transform controlsDisplay = owner
			.hud
			.transform
			.parent
			.Find("ControlsDisplay");

		// copy the controls display so we don't have to manually add
		// a bunch of tweening stuff
		Transform statTextObject = GameObject.Instantiate(
			controlsDisplay,
			transform);

		statTextObject.position = controlsDisplay.position;

		DestroyImmediate(statTextObject.GetComponent<AutoShowPanel>());

		Transform child = null;

		while(statTextObject.childCount > 1)
		{
			child = statTextObject.GetChild(1);
			DestroyImmediate(child.gameObject);
		}

		child = statTextObject.GetChild(0);
		child.name = "Text";

		statTextObject.name = "Labels";

		Transform statLabelsTextObject = Instantiate(statTextObject, transform);
		statLabelsTextObject.name = "Values";

		SetupPanel(statTextObject.gameObject);
		SetupPanel(statLabelsTextObject.gameObject);

		left = statTextObject.GetComponent<Panel>();
		right = statLabelsTextObject.GetComponent<Panel>();

		player = owner.player.GetComponent<PlayerController>();
	}

	private void SetupPanel(GameObject panelObj)
	{
		var fitter = panelObj.AddComponent<ContentSizeFitter>();
		fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		RectTransform rect = panelObj.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = panelObj.GetComponent<CanvasGroup>();

		// Reset positions
		rect.anchorMin = rect.anchorMax = Vector2.zero;
		rect.anchoredPosition = Vector2.zero;

		Panel panel = panelObj.GetComponent<Panel>();

		// some minor changes to make tweening work properly on first go
		Traverse.Create(panel).Field("canvasGroup").SetValue(canvasGroup);
		canvasGroup.alpha = 1f;
	}

	private void UpdatePanel()
	{
		StatsHolder stats = player.stats;

		TextMeshProUGUI leftMesh = left.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI rightMesh = right.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

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
		leftMesh.text = statHeader + statLabels;

		string statValues = $"<line-height=125%><color={BetterUI.configLabelColor.Value}>";
		statValues += $"<align=\"right\">{Math.Round(stats[StatType.BulletDamage].Modify(player.gun.gunData.damage), 0)} \n";
		statValues += $"<align=\"right\">{Math.Round(stats[StatType.Projectiles].Modify(player.gun.gunData.numOfProjectiles), 0)} \n";
		statValues += $"<align=\"right\">{Math.Round(stats[StatType.ReloadRate].Modify(player.gun.gunData.reloadDuration), 1)} \n";
		statValues += $"<align=\"right\">{Math.Round(1f / stats[StatType.FireRate].ModifyInverse(player.gun.gunData.shotCooldown), 1)} \n";
		statValues += $"<align=\"right\">{Math.Round(stats[StatType.MaxAmmo].Modify(player.gun.gunData.maxAmmo), 0)} \n";
		statValues += $"<align=\"right\">{Math.Round(stats[StatType.Piercing].Modify(player.gun.gunData.piercing), 0)} \n";
		statValues += $"<align=\"right\">{Math.Round(stats[StatType.Bounce].Modify(player.gun.gunData.bounce), 0)} \n";
		rightMesh.text = statValues;

		leftMesh.alignment = TextAlignmentOptions.BottomRight;
		rightMesh.alignment = TextAlignmentOptions.BottomRight;
	}

	public void Show()
	{
		UpdatePanel();

		left.Show();
		right.Show();
	}

	public void Hide()
	{
		left.Hide();
		right.Hide();
	}
}
