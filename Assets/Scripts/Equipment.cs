using UnityEngine;

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
