using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.Scripts.Base;

public enum EquipmentType { Weapon, Reactor, Shield, RepairSystem }

public abstract class EquipmentConfigBase {
  protected EquipmentConfigBase(string equipmentImageName,
                             string itemDisplayName,
                             LevelBasedValue baselineEnergyRequirement) {
    this.EquipmentImageName = equipmentImageName;
    this.ItemDisplayName = itemDisplayName;
    this.BaselineEnergyRequirement = baselineEnergyRequirement;
  }
  public string EquipmentImageName { get; }
  public string ItemDisplayName { get; }

  public LevelBasedValue BaselineEnergyRequirement { get; }

  public abstract EquipmentBase Instantiate(float level);
}

public class NoDisplayAttribute : Attribute { }
public class NoDropAttribute : Attribute { }

public class ShieldConfig : EquipmentConfigBase {
  private ShieldConfig(string equipmentImageName,
                      string itemDisplayName,
                      LevelBasedValue strength,
                      LevelBasedValue rechargeRate,
                      LevelBasedValue energyConsumptionWhenRechargingPerSecond,
                      LevelBasedValue timeBeforeRecharge,
                      LevelBasedValue baselineEnergyRequirement)
                      : base(equipmentImageName,
                             itemDisplayName,
                             baselineEnergyRequirement) {
    Strength = strength;
    RechargeRate = rechargeRate;
    TimeBeforeRecharge = timeBeforeRecharge;
    EnergyConsumptionWhenRechargingPerSecond = energyConsumptionWhenRechargingPerSecond;
  }

  public LevelBasedValue Strength { get; }
  public LevelBasedValue RechargeRate { get; }
  public LevelBasedValue EnergyConsumptionWhenRechargingPerSecond { get; }
  public LevelBasedValue TimeBeforeRecharge { get; }

  public static readonly ShieldConfig BALANCED = new(
    equipmentImageName: "Shield",
    itemDisplayName: "Basic shield",
    strength: LevelBasedValue.LinearValue(20),
    rechargeRate: LevelBasedValue.LinearValue(0.5f),
    timeBeforeRecharge: LevelBasedValue.ConstantValue(1f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1.5f)
  );

  public static ShieldConfig RAPID_CHARGE = new(
    equipmentImageName: "Shield",
    itemDisplayName: "Rapid recharge shield",
    strength: LevelBasedValue.LinearValue(12),
    rechargeRate: LevelBasedValue.LinearValue(1f),
    timeBeforeRecharge: LevelBasedValue.ConstantValue(0.2f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1.5f)
  );

  public static ShieldConfig STRONG = new(
    equipmentImageName: "Shield",
    itemDisplayName: "High strength shield",
    strength: LevelBasedValue.LinearValue(35),
    rechargeRate: LevelBasedValue.LinearValue(0.3f),
    timeBeforeRecharge: LevelBasedValue.ConstantValue(1.5f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1.5f)
  );

  public override EquipmentBase Instantiate(float level) => new ShieldInstance(this, level);
}

public class ReactorConfig : EquipmentConfigBase {
  private ReactorConfig(string equipmentImageName,
                       string itemDisplayName,
                       LevelBasedValue maxEnergyLevel,
                       LevelBasedValue rechargeRate)
                       : base(equipmentImageName,
                              itemDisplayName,
                              baselineEnergyRequirement: LevelBasedValue.ConstantValue(0)) {
    MaxEnergyLevel = maxEnergyLevel;
    EnergyRechargeRate = rechargeRate;
  }

  public LevelBasedValue MaxEnergyLevel { get; }
  public LevelBasedValue EnergyRechargeRate { get; }

  public static readonly ReactorConfig DEFAULT = new(
    equipmentImageName: "Reactor",
    itemDisplayName: "Basic reactor",
    maxEnergyLevel: LevelBasedValue.LinearValue(20),
    rechargeRate: LevelBasedValue.LinearValue(5)
  );

  public static ReactorConfig FAST_CHARGE = new(
    equipmentImageName: "Reactor",
    itemDisplayName: "Fast charge reactor",
    maxEnergyLevel: LevelBasedValue.LinearValue(1),
    rechargeRate: LevelBasedValue.LinearValue(6.5f)
  );

  public static ReactorConfig LARGE_BATTERY = new(
    equipmentImageName: "Reactor",
    itemDisplayName: "Large battery reactor",
    maxEnergyLevel: LevelBasedValue.LinearValue(50),
    rechargeRate: LevelBasedValue.LinearValue(4.2f)
  );

  public override EquipmentBase Instantiate(float level) => new ReactorInstance(this, level);
}

public class RepairSystemConfig : EquipmentConfigBase {
  private RepairSystemConfig(string equipmentImageName,
                      string itemDisplayName,
                      LevelBasedValue hullRepairPerSecond,
                      LevelBasedValue systemRepairPerSecond,
                      int numberOfSystemsToRepairSimultaneously,
                      LevelBasedValue baselineEnergyRequirement)
                       : base(equipmentImageName,
                              itemDisplayName,
                              baselineEnergyRequirement) {
    HullRepairPerSecond = hullRepairPerSecond;
    SystemRepairPerSecond = systemRepairPerSecond;
    NumberOfSystemsToRepairSimultaneously = numberOfSystemsToRepairSimultaneously;
  }

  public LevelBasedValue HullRepairPerSecond { get; }
  public LevelBasedValue SystemRepairPerSecond { get; }
  public int NumberOfSystemsToRepairSimultaneously { get; }

  public static RepairSystemConfig HULL_REPAIR = new(
    equipmentImageName: "RepairSystem",
    itemDisplayName: "Hull repair system",
    hullRepairPerSecond: LevelBasedValue.LinearValue(4),
    systemRepairPerSecond: LevelBasedValue.LinearValue(0),
    numberOfSystemsToRepairSimultaneously: 0,
    baselineEnergyRequirement: LevelBasedValue.LinearValue(2)
  );

  public static RepairSystemConfig NANO_REPAIR = new(
    equipmentImageName: "RepairSystem",
    itemDisplayName: "Nano repair",
    hullRepairPerSecond: LevelBasedValue.LinearValue(0),
    systemRepairPerSecond: LevelBasedValue.LinearValue(2),
    numberOfSystemsToRepairSimultaneously: 3,
    baselineEnergyRequirement: LevelBasedValue.LinearValue(2)
  );

  public static readonly RepairSystemConfig ROBOT_REPAIR = new(
    equipmentImageName: "RepairSystem",
    itemDisplayName: "Robotic repair system",
    hullRepairPerSecond: LevelBasedValue.LinearValue(1),
    systemRepairPerSecond: LevelBasedValue.LinearValue(1),
    numberOfSystemsToRepairSimultaneously: 1,
    baselineEnergyRequirement: LevelBasedValue.LinearValue(2)
  );

  public override EquipmentBase Instantiate(float level) => new RepairSystemInstance(this, level);
}

public abstract class EquipmentBase {
  protected EquipmentBase(EquipmentConfigBase config, float level, Guid? identifier = null) {
    this.Config = config;
    BaselineEnergyRequirement = config.BaselineEnergyRequirement.GetLevelValue(level);
    Level = level;
    Identifier = identifier ?? Guid.NewGuid();
    Health = MaxHealth;
  }
  public abstract EquipmentType Type { get; }
  public EquipmentConfigBase Config { get; }

  public float Level { get; }

  public float BaselineEnergyRequirement { get; }

  [NoDisplay]
  public int ScrapValue => (int)MathF.Ceiling(Level) * 3;
  [NoDisplay]
  public int UpgradeCost => ScrapValue * 3;
  [NoDisplay]
  public Guid Identifier { get; private set; }
  [NoDisplay]
  public float Health { get; set; }
  [NoDisplay]
  public float MaxHealth => Level * 40;
  [NoDisplay]
  public float DamageRatio => 1 - (Health / MaxHealth);
  [NoDisplay]
  public bool IsDamaged => DamageRatio > 0;
  [NoDisplay]
  public int FixCost => (int)(UpgradeCost * DamageRatio);
  
  [NoDisplay]
  public bool IsBeingForged => Forge.Instance.GetActionTypeForEquipment(this) != null;

  public EquipmentBase UpgradedVersion() {
    var newVersion = Config.Instantiate(MathF.Floor(Level + 1));
    newVersion.Identifier = Identifier;
    newVersion.Health = Health * newVersion.MaxHealth / MaxHealth;
    return newVersion;
  }

  #region item description
  protected string FormattedPropertyName(PropertyInfo propertyInfo) {
    if (string.IsNullOrEmpty(propertyInfo.Name)) return string.Empty;

    StringBuilder stringBuilder = new();

    for (int i = 0; i < propertyInfo.Name.Length; i++) {
      stringBuilder.Append(char.ToLower(propertyInfo.Name[i]));

      int nextChar = i + 1;
      if (nextChar < propertyInfo.Name.Length && char.IsUpper(propertyInfo.Name[nextChar]) && !char.IsUpper(propertyInfo.Name[i])) {
        stringBuilder.Append(" ");
      }
    }

    return stringBuilder.ToString();
  }

  protected virtual string PropertyInfoToString(PropertyInfo propertyInfo) {
    var name = FormattedPropertyName(propertyInfo);
    if (name.EndsWith("in seconds")) {
      return $"{name[..name.LastIndexOf(" in seconds", StringComparison.Ordinal)]}: {propertyInfo.GetValue(this):0.#} seconds";
    }
    if (name.EndsWith("per second")) {
      return $"{name[..name.LastIndexOf(" per second", StringComparison.Ordinal)]}: {propertyInfo.GetValue(this):0.#} per second";
    }
    return $"{name}: {propertyInfo.GetValue(this):0.#}";
  }

  public override string ToString() {
    var stringBuilder = new StringBuilder();
    stringBuilder.AppendLine(this.Config.ItemDisplayName);
    foreach (var propertyInfo in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
      if (!propertyInfo.CanWrite
          && propertyInfo.Name != "Type"
          && propertyInfo.Name != "Config"
          && propertyInfo.CustomAttributes.None(attribute => attribute.AttributeType == typeof(NoDisplayAttribute))) {
        var description = PropertyInfoToString(propertyInfo);
        if (!string.IsNullOrWhiteSpace(description)) {
          stringBuilder.AppendLine(description);
        }
      }
    }
    return stringBuilder.ToString();
  }
  #endregion
}

public class RepairSystemInstance : EquipmentBase {
  public RepairSystemInstance(RepairSystemConfig config, float level) : base(config, level) {
    HullRepairPerSecond = config.HullRepairPerSecond.GetLevelValue(level);
    SystemRepairPerSecond = config.SystemRepairPerSecond.GetLevelValue(level);
    NumberOfSystemsToRepairSimultaneously = config.NumberOfSystemsToRepairSimultaneously;
  }

  public override EquipmentType Type => EquipmentType.RepairSystem;
  public float HullRepairPerSecond { get; }
  public float SystemRepairPerSecond { get; }
  public int NumberOfSystemsToRepairSimultaneously { get; }
}

public class ShieldInstance : EquipmentBase {
  public ShieldInstance(ShieldConfig config, float level) : base(config, level) {
    MaxStrength = config.Strength.GetLevelValue(level);
    RechargeRatePerSecond = config.RechargeRate.GetLevelValue(level);
    TimeBeforeRecharge = config.TimeBeforeRecharge.GetLevelValue(level);
    EnergyConsumptionWhenRechargingPerSecond = config.EnergyConsumptionWhenRechargingPerSecond.GetLevelValue(level);
  }

  public override EquipmentType Type => EquipmentType.Shield;
  public float EnergyConsumptionWhenRechargingPerSecond { get; }
  public float MaxStrength { get; }
  public float RechargeRatePerSecond { get; }
  public float TimeBeforeRecharge { get; }
}

public class ReactorInstance : EquipmentBase {
  public ReactorInstance(ReactorConfig config, float level) : base(config, level) {
    MaxEnergyLevel = config.MaxEnergyLevel.GetLevelValue(level);
    EnergyRecoveryPerSecond = config.EnergyRechargeRate.GetLevelValue(level);
  }
  public override EquipmentType Type => EquipmentType.Reactor;
  public float MaxEnergyLevel { get; }
  public float EnergyRecoveryPerSecond { get; }

  protected override string PropertyInfoToString(PropertyInfo propertyInfo) {
    if (propertyInfo.Name == nameof(EquipmentBase.BaselineEnergyRequirement)) {
      return null;
    }
    return base.PropertyInfoToString(propertyInfo);
  }
}

public static class EquipmentExtensions {
  public static List<T> AllOfType<T>(this IEnumerable<EquipmentBase> equipment) where T : EquipmentBase {
    return equipment.Select(item => item as T)
      .Where(item => item != null)
      .ToList();
  }

  public static List<T> AllOfType<T>(this IEnumerable<EquipmentButtonScript> equipmentButtons) where T : EquipmentBase {
    return equipmentButtons
      .Select(button => button.Equipment)
      .AllOfType<T>();
  }

  public static float GetEnergyGeneration(this ICollection<EquipmentBase> equipment) {
    var baselineEnergyConsumption = equipment.Sum(item => item.BaselineEnergyRequirement);
    var baseEnergyGeneration = equipment.AllOfType<ReactorInstance>().Sum(reactor => reactor.EnergyRecoveryPerSecond);
    return baseEnergyGeneration - baselineEnergyConsumption;
  }

  public static float GetEnergyGeneration(this IEnumerable<EquipmentButtonScript> equipmentButtons) {
    return equipmentButtons
      .Select(button => button.Equipment)
      .Where(item => item != null)
      .ToArray()
      .GetEnergyGeneration();
  }
}
