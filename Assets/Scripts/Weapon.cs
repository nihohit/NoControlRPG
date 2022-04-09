using System;
using System.Reflection;
using UnityEngine;

public abstract class WeaponConfig : EquipmentConfigBase {
  protected WeaponConfig(LevelBasedValue range,
                         string shotImageName,
                         string equipmentImageName,
                         LevelBasedValue maxCharge,
                         LevelBasedValue energyConsumptionWhenRechargingPerSecond,
                         string itemDisplayName,
                         LevelBasedValue baselineEnergyRequirement)
                          : base(equipmentImageName,
                                 itemDisplayName,
                                 baselineEnergyRequirement) {
    this.range = range;
    this.shotImageName = shotImageName;
    this.maxCharge = maxCharge;
    this.energyConsumptionWhenRechargingPerSecond = energyConsumptionWhenRechargingPerSecond;
  }

  public readonly LevelBasedValue range;
  public readonly string shotImageName;
  public readonly LevelBasedValue maxCharge;
  public readonly LevelBasedValue energyConsumptionWhenRechargingPerSecond;

  public static BeamWeaponConfig LASER = new(
    range: LevelBasedValue.ConstantValue(2),
    shotImageName: "Heat Beam",
    equipmentImageName: "Laser",
    maxCharge: LevelBasedValue.ConstantValue(2.5f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
    damagePerSecond: LevelBasedValue.LinearValue(2f),
    itemDisplayName: "Laser beam",
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1.5f)
  );

  public static BulletWeaponConfig RIFLE = new(
    range: LevelBasedValue.ConstantValue(7f),
    shotImageName: "Bullet",
    equipmentImageName: "IncendiaryGun",
    maxCharge: LevelBasedValue.ConstantValue(1.5f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
    shotMovementSpeed: LevelBasedValue.ConstantValue(6f),
    damagePerBullet: LevelBasedValue.LinearValue(2),
    itemDisplayName: "Rifle",
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1)
  );

  public static BulletWeaponConfig MACHINE_GUN = new(
    range: LevelBasedValue.ConstantValue(6f),
    shotImageName: "Bullet",
    equipmentImageName: "MachineGun",
    maxCharge: LevelBasedValue.ConstantValue(1.5f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
    shotMovementSpeed: LevelBasedValue.ConstantValue(6f),
    numberOfSalvosPerShot: 4,
    timeBetweenSalvosInSeconds: 0.2f,
    shotSpreadInDegrees: 5,
    damagePerBullet: LevelBasedValue.LinearValue(1),
    itemDisplayName: "Machine gun",
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1)
  );

  public static BulletWeaponConfig TWO_SHOT_SHOTGUN = new(
    range: LevelBasedValue.ConstantValue(4f),
    shotImageName: "ShotgunPellet",
    equipmentImageName: "Shotgun",
    maxCharge: LevelBasedValue.ConstantValue(1.5f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1),
    numberOfBulletsPerSalvo: 5,
    shotMaxMovementSpeed: LevelBasedValue.ConstantValue(8.1f),
    shotMinMovementSpeed: LevelBasedValue.ConstantValue(7.3f),
    numberOfSalvosPerShot: 3,
    timeBetweenSalvosInSeconds: 1.0f / 30f,
    shotSpreadInDegrees: 10,
    damagePerBullet: LevelBasedValue.LinearValue(0.5f),
    itemDisplayName: "Two-shot shotgun",
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1)
  );

  public static BeamWeaponConfig FLAMER = new(
    range: LevelBasedValue.ConstantValue(5f),
    shotImageName: "Flamer",
    equipmentImageName: "Flamer",
    maxCharge: LevelBasedValue.ConstantValue(2.0f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1.0f),
    damagePerSecond: LevelBasedValue.LinearValue(1f),
    itemDisplayName: "Flamer",
    baselineEnergyRequirement: LevelBasedValue.LinearValue(1)
  );

  public static BulletWeaponConfig MISSILE = new(
    range: LevelBasedValue.ConstantValue(10f),
    shotImageName: "Missile",
    equipmentImageName: "Missile",
    maxCharge: LevelBasedValue.ConstantValue(2.0f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1.0f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(7f),
    damagePerBullet: LevelBasedValue.LinearValue(2),
    itemDisplayName: "Missile launcher",
    baselineEnergyRequirement: LevelBasedValue.LinearValue(5)
  );

  public static BulletWeaponConfig MINI_MISSILE = new(
    range: LevelBasedValue.ConstantValue(10f),
    shotImageName: "Missile",
    equipmentImageName: "Missile",
    maxCharge: LevelBasedValue.ConstantValue(2.0f),
    energyConsumptionWhenRechargingPerSecond: LevelBasedValue.LinearValue(1.0f),
    damagePerBullet: LevelBasedValue.LinearValue(0.8f),
    itemDisplayName: "Mini Missiles",
    shotMaxMovementSpeed: LevelBasedValue.ConstantValue(8.3f),
    shotMinMovementSpeed: LevelBasedValue.ConstantValue(8.1f),
    numberOfSalvosPerShot: 3,
    baselineEnergyRequirement: LevelBasedValue.LinearValue(5)
  );
}

public class BulletWeaponConfig : WeaponConfig {
  public BulletWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    string itemDisplayName,
    LevelBasedValue maxCharge,
    LevelBasedValue energyConsumptionWhenRechargingPerSecond,
    LevelBasedValue shotMaxMovementSpeed,
    LevelBasedValue shotMinMovementSpeed,
    LevelBasedValue damagePerBullet,
    LevelBasedValue baselineEnergyRequirement,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f)
    : base(range,
           shotImageName,
           equipmentImageName,
           maxCharge,
           energyConsumptionWhenRechargingPerSecond,
           itemDisplayName,
           baselineEnergyRequirement) {
    this.shotMaxMovementSpeed = shotMaxMovementSpeed;
    this.shotMinMovementSpeed = shotMinMovementSpeed;
    this.damagePerBullet = damagePerBullet;
    this.numberOfSalvosPerShot = numberOfSalvosPerShot;
    this.numberOfBulletsPerSalvo = numberOfBulletsPerSalvo;
    this.TimeBetweenSalvosInSeconds = timeBetweenSalvosInSeconds;
    this.ShotSpreadInDegrees = shotSpreadInDegrees;
  }

  public BulletWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    string itemDisplayName,
    LevelBasedValue maxCharge,
    LevelBasedValue energyConsumptionWhenRechargingPerSecond,
    LevelBasedValue shotMovementSpeed,
    LevelBasedValue damagePerBullet,
    LevelBasedValue baselineEnergyRequirement,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f)
    : this(range,
           shotImageName,
           equipmentImageName,
           itemDisplayName,
           maxCharge,
           energyConsumptionWhenRechargingPerSecond,
           shotMovementSpeed,
           shotMovementSpeed,
           damagePerBullet,
           baselineEnergyRequirement,
           numberOfSalvosPerShot,
           numberOfBulletsPerSalvo,
           timeBetweenSalvosInSeconds,
           shotSpreadInDegrees) {
  }

  public readonly LevelBasedValue shotMaxMovementSpeed;
  public readonly LevelBasedValue shotMinMovementSpeed;

  public readonly LevelBasedValue damagePerBullet;
  public readonly int numberOfBulletsPerSalvo;
  public float TimeBetweenSalvosInSeconds { get; }

  public readonly int numberOfSalvosPerShot;
  public float ShotSpreadInDegrees { get; }

  public override EquipmentBase Instantiate(float level) => new BulletWeaponInstance(this, level);
}

public class BeamWeaponConfig : WeaponConfig {
  public BeamWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    string itemDisplayName,
    LevelBasedValue baselineEnergyRequirement,
    LevelBasedValue maxCharge,
    LevelBasedValue energyConsumptionWhenRechargingPerSecond,
    LevelBasedValue damagePerSecond)
    : base(range,
           shotImageName,
           equipmentImageName,
           maxCharge,
           energyConsumptionWhenRechargingPerSecond,
           itemDisplayName,
           baselineEnergyRequirement) {
    this.damagePerSecond = damagePerSecond;
  }

  public readonly LevelBasedValue damagePerSecond;

  public override EquipmentBase Instantiate(float level) => new BeamInstance(this, level);
}

public abstract class WeaponBase : EquipmentBase {
  public new WeaponConfig Config { get; }
  public float MaxCharge { get; }
  public float CurrentCharge { get; set; }
  public float EnergyConsumptionWhenRechargingPerSecond { get; }
  public float Range { get; }
  protected WeaponBase(WeaponConfig config, float level) : base(config, level) {
    Config = config;
    this.MaxCharge = config.maxCharge.GetLevelValue(level);
    this.Range = config.range.GetLevelValue(level);
    this.EnergyConsumptionWhenRechargingPerSecond = config.energyConsumptionWhenRechargingPerSecond.GetLevelValue(level);
  }

  override public EquipmentType Type { get { return EquipmentType.Weapon; } }

  [NoDisplay]
  public override float CurrentChargingRequirementPerSecond => CurrentCharge < MaxCharge ? EnergyConsumptionWhenRechargingPerSecond : 0;

  public override void Charge(float chargeRatio) {
    CurrentCharge = MathF.Min(MaxCharge, CurrentCharge + chargeRatio * Time.deltaTime);
  }

  public abstract bool CanShoot();
}

public abstract class WeaponInstance<T> : WeaponBase where T : WeaponConfig {
  public new readonly T Config;
  protected WeaponInstance(T config, float level) : base(config, level) {
    Config = config;
  }
}

public class BeamInstance : WeaponInstance<BeamWeaponConfig> {
  public float DamagePerSecond { get; }

  public BeamInstance(BeamWeaponConfig config, float level) : base(config, level) {
    this.DamagePerSecond = config.damagePerSecond.GetLevelValue(level);
  }

  public bool IsCurrentlyfiring { get; set; }

  public override bool CanShoot() {
    return !IsCurrentlyfiring && CurrentCharge > 0.2f * MaxCharge;
  }
}

public class BulletWeaponInstance : WeaponInstance<BulletWeaponConfig> {
  [NoDisplay]
  public float ShotMaxMovementSpeed { get; }

  [NoDisplay]
  public float ShotMinMovementSpeed { get; }
  public float DamagePerBullet { get; }

  public BulletWeaponInstance(BulletWeaponConfig config, float level) : base(config, level) {
    this.ShotMaxMovementSpeed = config.shotMaxMovementSpeed.GetLevelValue(level);
    this.ShotMinMovementSpeed = config.shotMinMovementSpeed.GetLevelValue(level);
    this.DamagePerBullet = config.damagePerBullet.GetLevelValue(level);
  }

  public override string ToString() {
    var baseDescription = base.ToString();
    var numberOfBulletsPerShot = Config.numberOfBulletsPerSalvo * Config.numberOfSalvosPerShot;
    if (numberOfBulletsPerShot == 1) {
      return baseDescription;
    }

    return $"{baseDescription}number of bullets per shot: {numberOfBulletsPerShot}";
  }

  protected override string PropertyInfoToString(PropertyInfo propertyInfo) {
    if (propertyInfo.Name == nameof(WeaponBase.MaxCharge)) {
      return $"time between shots: {this.MaxCharge / this.EnergyConsumptionWhenRechargingPerSecond} seconds";
    }
    return base.PropertyInfoToString(propertyInfo);
  }

  public override bool CanShoot() {
    return CurrentCharge == MaxCharge;
  }
}