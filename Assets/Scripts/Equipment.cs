using System;
using System.Reflection;
using System.Text;
using Assets.Scripts.Base;
using UnityEngine;

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
  public ShieldConfig(string equipmentImageName, string itemDisplayName, LevelBasedValue strength, LevelBasedValue rechargeRate, LevelBasedValue energyConsumptionWhenRechargingPerSecond, LevelBasedValue timeBeforeRecharge, LevelBasedValue baselineEnergyRequirement) : base(equipmentImageName, itemDisplayName, baselineEnergyRequirement) {
    Strength = strength;
    RechargeRate = rechargeRate;
    TimeBeforeRecharge = timeBeforeRecharge;
    EnergyConsumptionWhenRechargingPerSecond = energyConsumptionWhenRechargingPerSecond;
  }

  public LevelBasedValue Strength { get; }
  public LevelBasedValue RechargeRate { get; }
  public LevelBasedValue EnergyConsumptionWhenRechargingPerSecond { get; }
  public LevelBasedValue TimeBeforeRecharge { get; }

  public static ShieldConfig DEFAULT = new("Shield", "Basic shield",
    strength: LevelBasedValue.LinearValue(10),
    rechargeRate: LevelBasedValue.LinearValue(0.5f),
    timeBeforeRecharge: LevelBasedValue.LinearValue(0.5f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
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
    maxEnergyLevel: LevelBasedValue.LinearValue(20),
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

  public abstract float CurrentChargingRequirementPerSecond { get; }

  public abstract void Charge(float chargeRatio);

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

public class ShieldInstance : EquipmentBase {
  public ShieldInstance(ShieldConfig config, float level) : base(config, level) {
    MaxStrength = config.Strength.GetLevelValue(level);
    CurrentStrength = MaxStrength;
    RechargeRatePerSecond = config.RechargeRate.GetLevelValue(level);
    TimeBeforeRecharge = config.TimeBeforeRecharge.GetLevelValue(level);
    EnergyConsumptionWhenRechargingPerSecond = config.EnergyConsumptionWhenRechargingPerSecond.GetLevelValue(level);
  }

  override public EquipmentType Type { get { return EquipmentType.Shield; } }
  public float EnergyConsumptionWhenRechargingPerSecond { get; }
  public float MaxStrength { get; }
  public float RechargeRatePerSecond { get; }
  public float TimeBeforeRecharge { get; }
  public float TimeBeforeNextRecharge { get; set; }
  public float CurrentStrength { get; set; }

  [NoDisplay]
  public override float CurrentChargingRequirementPerSecond => CurrentStrength < MaxStrength ? EnergyConsumptionWhenRechargingPerSecond : 0;

  public override void Charge(float chargeRatio) {
    CurrentStrength = MathF.Min(MaxStrength, CurrentStrength + RechargeRatePerSecond * chargeRatio * Time.deltaTime);
  }
}

public class ReactorInstance : EquipmentBase {
  public ReactorInstance(ReactorConfig config, float level) : base(config, level) {
    MaxEnergyLevel = config.MaxEnergyLevel.GetLevelValue(level);
    CurrentEnergyLevel = MaxEnergyLevel;
    EnergyRecoveryPerSecond = config.EnergyRechargeRate.GetLevelValue(level);
  }
  override public EquipmentType Type { get { return EquipmentType.Reactor; } }
  public float MaxEnergyLevel { get; }
  public float CurrentEnergyLevel { get; set; }
  public float EnergyRecoveryPerSecond { get; }

  [NoDisplay]
  public override float CurrentChargingRequirementPerSecond => throw new NotImplementedException();

  public override void Charge(float energy) {
    throw new NotImplementedException();
  }

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

  [NoDisplay]
  public override float CurrentChargingRequirementPerSecond => throw new NotImplementedException();

  public override void Charge(float energy) {
    throw new NotImplementedException();
  }
}
