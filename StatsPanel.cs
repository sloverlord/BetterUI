using System.Collections.Generic;
using System;
using flanne;
using UtillI;

namespace BetterUI;
public class StatsPanel : Registration
{
    public StatsPanel() : base(PanelPosition.BottomRight, DisplayRule.Always) { }
    private string statHeader = "";
    private StatList stats;
    override public void Init()
    {
        var player = UnityEngine.Object.FindObjectOfType<PlayerController>();

        statHeader += $"<line-height=115%><align=left><color={BetterUI.configHeaderColor.Value}>";
        statHeader += $"{Loadout.CharacterSelection.nameString}\n";
        statHeader += $"{Loadout.GunSelection.nameString}\n";
        // *should* be localized in the same way the game does internal
        statHeader += $"{LocalizationSystem.GetLocalizedValue("difficulty_label")} {Loadout.difficultyLevel}\n</color>";
        statHeader += "——————————————\n";

        stats = new StatList(BetterUI.configStatsSelection.Value, player.gun);
    }
    override public string GetUpdatedText()
    {
        return statHeader + stats.GetAllStats();
    }
}
