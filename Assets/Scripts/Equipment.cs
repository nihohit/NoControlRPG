using System.Reflection;
using System.Text;

public enum EquipmentType { Weapon, Reactor, Shield, TargetingSystem }

public class EquipmentConfigBase {
  public EquipmentConfigBase(string equipmentImageName) {
    this.equipmentImageName = equipmentImageName;
  }
  public readonly string equipmentImageName;
}


public class ShieldConfig : EquipmentConfigBase {
  public ShieldConfig(string equipmentImageName, LevelBasedValue strength, LevelBasedValue rechargeRate, LevelBasedValue timeBeforeRecharge) : base(equipmentImageName) {
    Strength = strength;
    RechargeRate = rechargeRate;
    TimeBeforeRecharge = timeBeforeRecharge;
  }

  public LevelBasedValue Strength { get; }
  public LevelBasedValue RechargeRate { get; }
  public LevelBasedValue TimeBeforeRecharge { get; }

  public static ShieldConfig DEFAULT = new("Shield",
    strength: LevelBasedValue.LinearValue(10),
    rechargeRate: LevelBasedValue.LinearValue(0.5f),
    timeBeforeRecharge: LevelBasedValue.LinearValue(1)
  );
}

public class ReactorConfig : EquipmentConfigBase {
  public ReactorConfig(string equipmentImageName, LevelBasedValue maxEnergyLevel, LevelBasedValue rechargeRate) : base(equipmentImageName) {
    MaxEnergyLevel = maxEnergyLevel;
    RechargeRate = rechargeRate;
  }

  public LevelBasedValue MaxEnergyLevel { get; }
  public LevelBasedValue RechargeRate { get; }

  public static ReactorConfig DEFAULT = new("Reactor",
    maxEnergyLevel: LevelBasedValue.LinearValue(1),
    rechargeRate: LevelBasedValue.LinearValue(1)
  );
}

public class TargetingSystemConfig : EquipmentConfigBase {
  public TargetingSystemConfig(string equipmentImageName) : base(equipmentImageName) { }

  public static TargetingSystemConfig DEFAULT = new("TargetingSystem");
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
    return $"{FormattedPropertyName(propertyInfo)}: {propertyInfo.GetValue(this)}";
  }

  public override string ToString() {
    var stringBuilder = new StringBuilder();
    foreach (var propertyInfo in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
      if (!propertyInfo.CanWrite && propertyInfo.Name != "Type" && propertyInfo.Name != "Config") {
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
