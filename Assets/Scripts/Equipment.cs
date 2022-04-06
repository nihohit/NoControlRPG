using System;
using System.Reflection;
using System.Text;
using Assets.Scripts.Base;

public enum EquipmentType { Weapon, Reactor, Shield, TargetingSystem }

public class EquipmentConfigBase {
  public EquipmentConfigBase(string equipmentImageName, string itemDisplayName, LevelBasedValue baselineEnergyRequirement) {
    this.EquipmentImageName = equipmentImageName;
    this.ItemDisplayName = itemDisplayName;
    this.BaselineEnergyRequirement = baselineEnergyRequirement;
  }
  public string EquipmentImageName { get; }
  public string ItemDisplayName { get; }

  public LevelBasedValue BaselineEnergyRequirement { get; }
}

public class NoDisplayAttribute : Attribute { }

public class ShieldConfig : EquipmentConfigBase {
  public ShieldConfig(string equipmentImageName, string itemDisplayName, LevelBasedValue strength, LevelBasedValue rechargeRate, LevelBasedValue timeBeforeRecharge, LevelBasedValue baselineEnergyRequirement) : base(equipmentImageName, itemDisplayName, baselineEnergyRequirement) {
    Strength = strength;
    RechargeRate = rechargeRate;
    TimeBeforeRecharge = timeBeforeRecharge;
  }

  public LevelBasedValue Strength { get; }
  public LevelBasedValue RechargeRate { get; }
  public LevelBasedValue TimeBeforeRecharge { get; }

  public static ShieldConfig DEFAULT = new("Shield", "Basic shield",
    strength: LevelBasedValue.LinearValue(10),
    rechargeRate: LevelBasedValue.LinearValue(0.5f),
    timeBeforeRecharge: LevelBasedValue.LinearValue(1),
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1.5f)
  );
}

public class ReactorConfig : EquipmentConfigBase {
  public ReactorConfig(string equipmentImageName, string itemDisplayName, LevelBasedValue maxEnergyLevel, LevelBasedValue rechargeRate) : base(equipmentImageName, itemDisplayName, baselineEnergyRequirement: LevelBasedValue.ConstantValue(0)) {
    MaxEnergyLevel = maxEnergyLevel;
    EnergyRechargeRate = rechargeRate;
  }

  public LevelBasedValue MaxEnergyLevel { get; }
  public LevelBasedValue EnergyRechargeRate { get; }

  public static ReactorConfig DEFAULT = new("Reactor", "Basic reactor",
    maxEnergyLevel: LevelBasedValue.LinearValue(1),
    rechargeRate: LevelBasedValue.LinearValue(5)
  );
}

public class TargetingSystemConfig : EquipmentConfigBase {
  public TargetingSystemConfig(string equipmentImageName, string itemDisplayName, LevelBasedValue baselineEnergyRequirement) : base(equipmentImageName, itemDisplayName, baselineEnergyRequirement) { }

  public static TargetingSystemConfig DEFAULT = new("TargetingSystem", "Default targeting system", LevelBasedValue.ConstantValue(0));
}

public abstract class EquipmentBase {
  public EquipmentBase(EquipmentConfigBase config, float level) {
    this.Config = config;
    BaselineEnergyRequirement = config.BaselineEnergyRequirement.GetLevelValue(level);
  }
  public abstract EquipmentType Type { get; }
  public EquipmentConfigBase Config { get; }

  public float BaselineEnergyRequirement { get; }

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

  protected virtual string? PropertyInfoToString(PropertyInfo propertyInfo) {
    var name = FormattedPropertyName(propertyInfo);
    if (name.EndsWith("in seconds")) {
      return $"{name[..name.LastIndexOf(" in seconds")]}: {propertyInfo.GetValue(this)} seconds";
    }
    if (name.EndsWith("per second")) {
      return $"{name[..name.LastIndexOf(" per second")]}: {propertyInfo.GetValue(this)} per second";
    }
    return $"{name}: {propertyInfo.GetValue(this)}";
  }

  public override string ToString() {
    var stringBuilder = new StringBuilder();
    stringBuilder.AppendLine(this.Config.ItemDisplayName);
    foreach (var propertyInfo in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
      var description = PropertyInfoToString(propertyInfo);
      if (!propertyInfo.CanWrite
          && propertyInfo.Name != "Type"
          && propertyInfo.Name != "Config"
          && propertyInfo.CustomAttributes.None(attribute => attribute.AttributeType == typeof(NoDisplayAttribute))
          && !string.IsNullOrWhiteSpace(description)) {
        stringBuilder.AppendLine(description);
      }
    }
    return stringBuilder.ToString();
  }
}

public class ShieldInstance : EquipmentBase {
  public ShieldInstance(ShieldConfig config, float level) : base(config, level) {
    MaxStrength = config.Strength.GetLevelValue(level);
    CurrentStrength = MaxStrength;
    RechargeRatePerSecond = config.RechargeRate.GetLevelValue(level);
    TimeBeforeRecharge = config.TimeBeforeRecharge.GetLevelValue(level);
  }

  override public EquipmentType Type { get { return EquipmentType.Shield; } }
  public float MaxStrength { get; }
  public float RechargeRatePerSecond { get; }
  public float TimeBeforeRecharge { get; }
  public float TimeBeforeNextRecharge { get; set; }
  public float CurrentStrength { get; set; }
}

public class ReactorInstance : EquipmentBase {
  public ReactorInstance(ReactorConfig config, float level) : base(config, level) {
    MaxEnergyLevel = config.MaxEnergyLevel.GetLevelValue(level);
    CurrentEnergyLevel = MaxEnergyLevel;
    RechargeRate = config.EnergyRechargeRate.GetLevelValue(level);
  }
  override public EquipmentType Type { get { return EquipmentType.Reactor; } }
  public float MaxEnergyLevel { get; }
  public float CurrentEnergyLevel { get; set; }
  public float RechargeRate { get; }

  protected override string PropertyInfoToString(PropertyInfo propertyInfo) {
    if (propertyInfo.Name == nameof(EquipmentBase.BaselineEnergyRequirement)) {
      return null;
    }
    return base.PropertyInfoToString(propertyInfo);
  }
}

public class TargetingSystemInstance : EquipmentBase {
  public TargetingSystemInstance(TargetingSystemConfig config, float level) : base(config, level) { }
  override public EquipmentType Type { get { return EquipmentType.TargetingSystem; } }
}
