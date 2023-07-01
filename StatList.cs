using System;
using System.Collections.Generic;
using flanne;
namespace BetterUI;
public enum StatSelection
{
    Base,
    Extra,
    All,
    Custom
}
struct StatLine
{
    string label;
    Func<double> getStat;
    public StatLine(string title, int tabsNumber, Func<double> getStat, string labelColor = "")
    {
        if (labelColor == "") labelColor = BetterUI.configDefaultLabelColor.Value;
        this.label = $"<color={labelColor}>" + title + ":</color>" + "".PadLeft(tabsNumber, '\t');
        this.getStat = getStat;
    }

    public string printStat()
    {
        return label + getStat() + '\n';
    }
}

public class StatList
{
    private PlayerController playerController;
    private Gun gun;
    private StatsHolder stats;
    private ProjectileRecipe __storedPR;
    private float originalMoveSpeed; // Workaround to avoid spikes of moveSpeed during Hina's dash
    private ProjectileRecipe currentProjectileRecipe
    {
        get
        {
            if (__storedPR == null) __storedPR = gun.GetProjectileRecipe();
            return __storedPR;
        }
    }
    private List<StatLine> statLines = new List<StatLine>();
    public StatList(PlayerController playerController)
    {
        this.playerController = playerController;
        this.gun = playerController.gun;
        this.stats = playerController.stats;
        this.originalMoveSpeed = playerController.movementSpeed;
        BetterUI.configStatsSelection.SettingChanged += Setup;
        Setup();
    }
    public string GetAllStats()
    {
        __storedPR = null;
        var allStats = "";
        foreach (var statLine in statLines) allStats += statLine.printStat();
        return allStats;
    }
    void SetupStatLines(
        bool damage = false,
        bool reloadRate = false,
        bool fireRate = false,
        bool pierce = false,
        bool bounce = false,
        bool spread = false,
        bool maxAmmo = false,
        bool moveSpeed = false,
        bool walkSpeed = false,
        bool summonDamage = false,
        bool summonAtkSpd = false,
        bool projectiles = false,
        bool projSpeed = false,
        bool projSize = false,
        bool knockback = false,
        bool characterSize = false,
        bool pickupRange = false,
        bool visionRange = false,
        bool dodge = false,
        bool dodgeCap = false
       )
    {
        statLines.Clear();
        if (damage) statLines.Add(new StatLine("Damage", 2, () => Math.Round(gun.damage, 0), BetterUI.configGunLabelColor.Value));
        if (reloadRate) statLines.Add(new StatLine("Reload Time", 1, () => Math.Round(gun.reloadDuration, 1), BetterUI.configGunLabelColor.Value));
        if (fireRate) statLines.Add(new StatLine("Fire Rate", 2, () => Math.Round(1f / gun.shotCooldown, 1), BetterUI.configGunLabelColor.Value));
        if (spread) statLines.Add(new StatLine("Spread", 2, () => Math.Round((double)gun.spread, 0), BetterUI.configGunLabelColor.Value));
        if (projectiles) statLines.Add(new StatLine("Projectiles", 1, () => gun.numOfProjectiles, BetterUI.configProjectileLabelColor.Value));
        if (projSpeed) statLines.Add(new StatLine("Proj Speed", 1, () => Math.Round((double)currentProjectileRecipe.projectileSpeed, 0), BetterUI.configProjectileLabelColor.Value));
        if (projSize) statLines.Add(new StatLine("Proj Size", 2, () => Math.Round((double)currentProjectileRecipe.size, 1), BetterUI.configProjectileLabelColor.Value));
        if (pierce) statLines.Add(new StatLine("Pierce", 2, () => Math.Min(currentProjectileRecipe.piercing, 999), BetterUI.configProjectileLabelColor.Value));
        if (bounce) statLines.Add(new StatLine("Bounce", 2, () => currentProjectileRecipe.bounce, BetterUI.configProjectileLabelColor.Value));
        if (knockback) statLines.Add(new StatLine("Knockback", 2, () => Math.Round((double)currentProjectileRecipe.knockback, 1), BetterUI.configProjectileLabelColor.Value));
        if (summonDamage) statLines.Add(new StatLine("Smn Damage", 1, () => Math.Round((stats[StatType.SummonDamage].Modify(1f)), 1), BetterUI.configSummonLabelColor.Value));
        if (summonAtkSpd) statLines.Add(new StatLine("Smn Atk Spd", 1, () => Math.Round((stats[StatType.SummonAttackSpeed].ModifyInverse(1f)), 1), BetterUI.configSummonLabelColor.Value));
        if (moveSpeed) statLines.Add(new StatLine("Move Speed", 1, () => Math.Round(stats[StatType.MoveSpeed].Modify(originalMoveSpeed), 1), BetterUI.configCharacterLabelColor.Value));
        if (walkSpeed) statLines.Add(new StatLine("Walk Speed", 1, () => Math.Round(Math.Min(1f, stats[StatType.WalkSpeed].Modify(0.35f)) * stats[StatType.MoveSpeed].Modify(originalMoveSpeed), 1), BetterUI.configCharacterLabelColor.Value));
        //if (maxAmmo) statLines.Add(new StatLine("Max Ammo", 1, () => Math.Round((double)gun.maxAmmo, 0)));
        if (characterSize) statLines.Add(new StatLine("Char Size", 2, () => Math.Round((stats[StatType.CharacterSize].Modify(1f)), 1), BetterUI.configCharacterLabelColor.Value));
        if (pickupRange) statLines.Add(new StatLine("Pickup Range", 1, () => Math.Round((stats[StatType.PickupRange].Modify(1f)), 1), BetterUI.configCharacterLabelColor.Value));
        if (visionRange) statLines.Add(new StatLine("Vision Range", 1, () => Math.Round((stats[StatType.VisionRange].Modify(1f)), 1), BetterUI.configCharacterLabelColor.Value));
        if (dodge) statLines.Add(new StatLine("Dodge Chance", 1, () => Math.Round((stats[StatType.Dodge].Modify(1f) - 1f).NotifyModifiers(DodgeRoller.TweakDodgeNotification, this), 2), BetterUI.configCharacterLabelColor.Value));
        if (dodgeCap) statLines.Add(new StatLine("Dodge Cap", 2, () => Math.Round(stats[StatType.DodgeCapMod].Modify(60f) / 100f, 2), BetterUI.configCharacterLabelColor.Value));
    }
    void Setup()
    {
        switch (BetterUI.configStatsSelection.Value)
        {
            case StatSelection.Base:
                // ----------  DMG   RloT  FiRt  Prce  Bnce
                SetupStatLines(true, true, true, true, true);
                break;
            case StatSelection.Extra:
                // ----------  DMG   RloT  FiRt  Prce  Bnce  Spre  Amo    mSpd  wSpd   sDmg  sSpd  proj   pSpd   pSz    pKb    chSz   pUpR   visR   dCha  dCap
                SetupStatLines(true, true, true, true, true, true, false, true, false, true, true, false, false, false, false, false, false, false, true, false);
                break;
            case StatSelection.All:
                SetupStatLines(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true);
                break;
            case StatSelection.Custom:
                SetupStatLines(
                    BetterUI.statsCustomHasDamage.Value,
                    BetterUI.statsCustomHasReloadTime.Value,
                    BetterUI.statsCustomHasFireRate.Value,
                    BetterUI.statsCustomHasPierce.Value,
                    BetterUI.statsCustomHasBounce.Value,
                    BetterUI.statsCustomHasSpread.Value,
                    false,
                    BetterUI.statsCustomHasMoveSpeed.Value,
                    BetterUI.statsCustomHasWalkSpeed.Value,
                    BetterUI.statsCustomHasSummonDamage.Value,
                    BetterUI.statsCustomHasSummonAttackSpeed.Value,
                    BetterUI.statsCustomHasProjectileNumber.Value,
                    BetterUI.statsCustomHasProjectileSpeed.Value,
                    BetterUI.statsCustomHasProjectileSize.Value,
                    BetterUI.statsCustomHasKnockback.Value,
                    BetterUI.statsCustomHasCharacterSize.Value,
                    BetterUI.statsCustomHasPickupRange.Value,
                    BetterUI.statsCustomHasVisionRange.Value,
                    BetterUI.statsCustomHasDodge.Value,
                    BetterUI.statsCustomHasDodgeCap.Value
                    );
                break;
        }
    }
    void Setup(object sender, EventArgs e)
    {
        Setup();
    }
}