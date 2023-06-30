
using System;
using System.Collections.Generic;
using flanne;
namespace BetterUI;
public enum StatSelection
{
    Default,
    Extra,
    Custom
}
public struct StatLine
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

public class StatList
{
    private Gun gun;
    private ProjectileRecipe __storedPR;
    private ProjectileRecipe currentProjectileRecipe
    {
        get
        {
            if (__storedPR == null) __storedPR = gun.GetProjectileRecipe();
            return __storedPR;
        }
    }
    public List<StatLine> statLines = new List<StatLine>();

    public StatList(StatSelection selection, Gun gun)
    {
        this.gun = gun;
        switch (selection)
        {
            case StatSelection.Default:
                SetupStatLines(true, false, true, true, false, true, true);
                break;
            case StatSelection.Extra:
                SetupStatLines(true, true, true, true, true, true, true);
                break;
            case StatSelection.Custom:
                SetupStatLines(false, true, true, true, true, true, true);
                break;
        }
    }
    void SetupStatLines(bool damage, bool projectiles, bool reloadRate, bool fireRate, bool maxAmmo, bool pierce, bool bounce)
    {
        if (damage) statLines.Add(new StatLine("Damage", 2, () => { return Math.Round(gun.damage, 0); }));
        if (projectiles) statLines.Add(new StatLine("Projectiles", 1, () => { return Math.Round((double)gun.numOfProjectiles, 0); }));
        if (reloadRate) statLines.Add(new StatLine("Reload Time", 1, () => Math.Round(gun.reloadDuration, 1)));
        if (fireRate) statLines.Add(new StatLine("Fire Rate", 2, () => Math.Round(1f / gun.shotCooldown, 1)));
        if (maxAmmo) statLines.Add(new StatLine("Max Ammo", 1, () => Math.Round((double)gun.maxAmmo, 0)));
        if (pierce) statLines.Add(new StatLine("Pierce", 2, () => Math.Round((double)currentProjectileRecipe.piercing, 0)));
        if (bounce) statLines.Add(new StatLine("Bounce", 2, () => Math.Round((double)currentProjectileRecipe.bounce, 0)));
    }
    public string GetAllStats()
    {
        var allStats = "";
        foreach (var statLine in statLines) allStats += statLine.printStat();
        return allStats;
    }
}