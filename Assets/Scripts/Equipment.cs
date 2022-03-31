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

  public LevelBasedValue Strength { get; private set; }
  public LevelBasedValue RechargeRate { get; private set; }
  public LevelBasedValue TimeBeforeRecharge { get; private set; }
}

public class ReactorConfig : EquipmentConfigBase {
  public ReactorConfig(string equipmentImageName, LevelBasedValue maxEnergyLevel, LevelBasedValue rechargeRate) : base(equipmentImageName) {
    MaxEnergyLevel = maxEnergyLevel;
    RechargeRate = rechargeRate;
  }

  public LevelBasedValue MaxEnergyLevel { get; private set; }
  public LevelBasedValue RechargeRate { get; private set; }
}

public class TargetingSystemConfig : EquipmentConfigBase {
  public TargetingSystemConfig(string equipmentImageName) : base(equipmentImageName) { }
}

public abstract class EquipmentBase {
  public EquipmentBase(EquipmentConfigBase config) {
    this.Config = config;
  }
  public abstract EquipmentType Type { get; }
  public EquipmentConfigBase Config { get; private set; }
}

public class ShieldInstance : EquipmentBase {
  public ShieldInstance(ShieldConfig config, float level) : base(config) {
    MaxStrength = config.Strength.getLevelValue(level);
    CurrentStrength = MaxStrength;
    RechargeRate = config.RechargeRate.getLevelValue(level);
    TimeBeforeRecharge = config.TimeBeforeRecharge.getLevelValue(level);
  }

  override public EquipmentType Type { get { return EquipmentType.Shield; } }
  public float MaxStrength { get; private set; }
  public float RechargeRate { get; private set; }
  public float TimeBeforeRecharge { get; private set; }
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
  public float MaxEnergyLevel { get; private set; }
  public float CurrentEnergyLevel { get; private set; }
  public float RechargeRate { get; private set; }
}

public class TargetingSystemInstance : EquipmentBase {
  public TargetingSystemInstance(TargetingSystemConfig config, float level) : base(config) { }
  override public EquipmentType Type { get { return EquipmentType.TargetingSystem; } }
}