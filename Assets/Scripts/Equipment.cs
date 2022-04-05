using System;
using System.Reflection;
using System.Text;
using Assets.Scripts.Base;

public enum EquipmentType { Weapon, Reactor, Shield, TargetingSystem }

public class EquipmentConfigBase {
  public EquipmentConfigBase(string equipmentImageName, string itemDisplayName) {
    this.EquipmentImageName = equipmentImageName;
    this.ItemDisplayName = itemDisplayName;
  }
  public string EquipmentImageName { get; }
  public string ItemDisplayName { get; }
}

public class NoDisplayAttribute : Attribute { }

public class ShieldConfig : EquipmentConfigBase {
  public ShieldConfig(string equipmentImageName, string itemDisplayName, LevelBasedValue strength, LevelBasedValue rechargeRate, LevelBasedValue timeBeforeRecharge) : base(equipmentImageName, itemDisplayName) {
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
    timeBeforeRecharge: LevelBasedValue.LinearValue(1)
  );
}

public class ReactorConfig : EquipmentConfigBase {
  public ReactorConfig(string equipmentImageName, string itemDisplayName, LevelBasedValue maxEnergyLevel, LevelBasedValue rechargeRate) : base(equipmentImageName, itemDisplayName) {
    MaxEnergyLevel = maxEnergyLevel;
    RechargeRate = rechargeRate;
  }

  public LevelBasedValue MaxEnergyLevel { get; }
  public LevelBasedValue RechargeRate { get; }

  public static ReactorConfig DEFAULT = new("Reactor", "Basic reactor",
    maxEnergyLevel: LevelBasedValue.LinearValue(1),
    rechargeRate: LevelBasedValue.LinearValue(1)
  );
}

public class TargetingSystemConfig : EquipmentConfigBase {
  public TargetingSystemConfig(string equipmentImageName, string itemDisplayName) : base(equipmentImageName, itemDisplayName) { }

  public static TargetingSystemConfig DEFAULT = new("TargetingSystem", "Default targeting system");
}

public abstract class EquipmentBase {
  public EquipmentBase(EquipmentConfigBase config) {
    this.Config = config;
  }
  public abstract EquipmentType Type { get; }
  public EquipmentConfigBase Config { get; }

  protected string FormattedPropertyName(PropertyInfo propertyInfo) {
    if (string.IsNullOrEmpty(propertyInfo.Name)) return string.Empty;

    StringBuilder stringBuilder = new StringBuilder();

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
        stringBuilder.AppendLine(PropertyInfoToString(propertyInfo));
      }
    }
    return stringBuilder.ToString();
  }
}

public class ShieldInstance : EquipmentBase {
  public ShieldInstance(ShieldConfig config, float level) : base(config) {
    MaxStrength = config.Strength.getLevelValue(level);
    CurrentStrength = MaxStrength;
    RechargeRatePerSecond = config.RechargeRate.getLevelValue(level);
    TimeBeforeRecharge = config.TimeBeforeRecharge.getLevelValue(level);
  }

  override public EquipmentType Type { get { return EquipmentType.Shield; } }
  public float MaxStrength { get; }
  public float RechargeRatePerSecond { get; }
  public float TimeBeforeRecharge { get; }
  public float TimeBeforeNextRecharge { get; set; }
  public float CurrentStrength { get; set; }
}

public class ReactorInstance : EquipmentBase {
  public ReactorInstance(ReactorConfig config, float level) : base(config) {
    MaxEnergyLevel = config.MaxEnergyLevel.getLevelValue(level);
    CurrentEnergyLevel = MaxEnergyLevel;
    RechargeRate = config.RechargeRate.getLevelValue(level);
  }
  override public EquipmentType Type { get { return EquipmentType.Reactor; } }
  public float MaxEnergyLevel { get; }
  public float CurrentEnergyLevel { get; set; }
  public float RechargeRate { get; }
}

public class TargetingSystemInstance : EquipmentBase {
  public TargetingSystemInstance(TargetingSystemConfig config, float level) : base(config) { }
  override public EquipmentType Type { get { return EquipmentType.TargetingSystem; } }
}
