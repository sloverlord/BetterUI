using System.Collections.Generic;
using System;

using flanne;
using UtillI;
namespace BetterUI;

struct StatLine
{
    string label;
    Func<double> getStat;
    public StatLine(string title, int tabsNumber, Func<double> getStat)
    {
        this.label = $"<color={BetterUI.configLabelColor.Value}>" + title + ":</color>" + "".PadLeft(tabsNumber, '\t');
        this.getStat = getStat;
    }

    public string printStat()
    {
        return label + getStat() + '\n';
    }
}

public class StatsPanel : Registration
{
    public StatsPanel() : base(PanelPosition.BottomRight, DisplayRule.Always) { }
    private Gun gun;
    private string statHeader = "";
    private List<StatLine> statLines = new List<StatLine>();
    private ProjectileRecipe __storedPR;
    private ProjectileRecipe currentProjectileRecipe
    {
        get
        {
            if (__storedPR == null) __storedPR = gun.GetProjectileRecipe();
            return __storedPR;
        }
    }

    override public void Init()
    {
        var player = UnityEngine.Object.FindObjectOfType<PlayerController>();
        gun = player.gun;

        statHeader += $"<line-height=115%><align=left><color={BetterUI.configheaderColor.Value}>";
        statHeader += $"{Loadout.CharacterSelection.nameString}\n";
        statHeader += $"{Loadout.GunSelection.nameString}\n";
        // *should* be localized in the same way the game does internal
        statHeader += $"{LocalizationSystem.GetLocalizedValue("difficulty_label")} {Loadout.difficultyLevel}\n</color>";
        statHeader += "——————————————\n";

        statLines.Add(new StatLine("Damage", 2, () => { return Math.Round(gun.damage, 0); }));
        statLines.Add(new StatLine("Projectiles", 1, () => { return Math.Round((double)gun.numOfProjectiles, 0); }));
        statLines.Add(new StatLine("Reload Time", 1, () => Math.Round(gun.reloadDuration, 1)));
        statLines.Add(new StatLine("Fire Rate", 2, () => Math.Round(1f / gun.shotCooldown, 1)));
        statLines.Add(new StatLine("Max Ammo", 1, () => Math.Round((double)gun.maxAmmo, 0)));
        statLines.Add(new StatLine("Pierce", 2, () => Math.Round((double)currentProjectileRecipe.piercing, 0)));
        statLines.Add(new StatLine("Bounce", 2, () => Math.Round((double)currentProjectileRecipe.bounce, 0)));
    }
    override public string GetUpdatedText()
    {
        __storedPR = null;
        var allStats = "";
        foreach (var statLine in statLines) allStats += statLine.printStat();
        return statHeader + allStats;
    }
}
