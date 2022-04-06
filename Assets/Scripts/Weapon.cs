using System;

public abstract class WeaponConfig : EquipmentConfigBase {
  protected WeaponConfig(LevelBasedValue range, string shotImageName, string equipmentImageName, LevelBasedValue timeBetweenShotsInSeconds, string itemDisplayName, LevelBasedValue baselineEnergyRequirement) : base(equipmentImageName, itemDisplayName, baselineEnergyRequirement) {
    this.range = range;
    this.shotImageName = shotImageName;
    this.timeBetweenShotsInSeconds = timeBetweenShotsInSeconds;
  }

  public readonly LevelBasedValue range;
  public readonly string shotImageName;
  public readonly LevelBasedValue timeBetweenShotsInSeconds;

  public static BeamWeaponConfig LASER = new(
    range: LevelBasedValue.ConstantValue(2),
    shotImageName: "Heat Beam",
    equipmentImageName: "Laser",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.5f),
    beamLifetimeInSeconds: LevelBasedValue.ConstantValue(0.5f),
    damagePerSecond: LevelBasedValue.ConstantValue(2f),
    itemDisplayName: "Laser beam",
    baselineEnergyRequirement: LevelBasedValue.ConstantValue(1.5f)
  );

  public static BulletWeaponConfig RIFLE = new(
    range: LevelBasedValue.ConstantValue(7f),
    shotImageName: "Bullet",
    equipmentImageName: "IncendiaryGun",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(1.5f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(6f),
    damagePerBullet: LevelBasedValue.ConstantValue(2),
    itemDisplayName: "Rifle",
    baselineEnergyRequirement: LevelBasedValue.ConstantValue(1)
  );

  public static BulletWeaponConfig MACHINE_GUN = new(
    range: LevelBasedValue.ConstantValue(6f),
    shotImageName: "Bullet",
    equipmentImageName: "MachineGun",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(1.5f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(6f),
    numberOfSalvosPerShot: 4,
    timeBetweenSalvosInSeconds: 0.2f,
    shotSpreadInDegrees: 5,
    damagePerBullet: LevelBasedValue.ConstantValue(1),
    itemDisplayName: "Machine gun",
    baselineEnergyRequirement: LevelBasedValue.ConstantValue(1)
  );

  public static BulletWeaponConfig TWO_SHOT_SHOTGUN = new(
    range: LevelBasedValue.ConstantValue(4f),
    shotImageName: "ShotgunPellet",
    equipmentImageName: "Shotgun",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(1.5f),
    numberOfBulletsPerSalvo: 5,
    shotMaxMovementSpeed: LevelBasedValue.ConstantValue(8.1f),
    shotMinMovementSpeed: LevelBasedValue.ConstantValue(7.3f),
    numberOfSalvosPerShot: 3,
    timeBetweenSalvosInSeconds: 1.0f / 30f,
    shotSpreadInDegrees: 10,
    damagePerBullet: LevelBasedValue.ConstantValue(0.5f),
    itemDisplayName: "Two-shot shotgun",
    baselineEnergyRequirement: LevelBasedValue.ConstantValue(1)
  );

  public static BeamWeaponConfig FLAMER = new(
    range: LevelBasedValue.ConstantValue(5f),
    shotImageName: "Flamer",
    equipmentImageName: "Flamer",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.0f),
    beamLifetimeInSeconds: LevelBasedValue.ConstantValue(1f),
    damagePerSecond: LevelBasedValue.ConstantValue(1f),
    itemDisplayName: "Flamer",
    baselineEnergyRequirement: LevelBasedValue.ConstantValue(1)
  );

  public static BulletWeaponConfig MISSILE = new(
    range: LevelBasedValue.ConstantValue(10f),
    shotImageName: "Missile",
    equipmentImageName: "Missile",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.0f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(7f),
    damagePerBullet: LevelBasedValue.ConstantValue(1),
    itemDisplayName: "Missile launcher",
    baselineEnergyRequirement: LevelBasedValue.ConstantValue(5)
  );
}

public class BulletWeaponConfig : WeaponConfig {
  public BulletWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    string itemDisplayName,
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue shotMaxMovementSpeed,
    LevelBasedValue shotMinMovementSpeed,
    LevelBasedValue damagePerBullet,
    LevelBasedValue baselineEnergyRequirement,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds, itemDisplayName, baselineEnergyRequirement) {
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
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue shotMovementSpeed,
    LevelBasedValue damagePerBullet,
    LevelBasedValue baselineEnergyRequirement,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds, itemDisplayName, baselineEnergyRequirement) {
    this.shotMaxMovementSpeed = shotMovementSpeed;
    this.shotMinMovementSpeed = shotMovementSpeed;
    this.damagePerBullet = damagePerBullet;
    this.numberOfSalvosPerShot = numberOfSalvosPerShot;
    this.numberOfBulletsPerSalvo = numberOfBulletsPerSalvo;
    this.TimeBetweenSalvosInSeconds = timeBetweenSalvosInSeconds;
    this.ShotSpreadInDegrees = shotSpreadInDegrees;
  }

  public readonly LevelBasedValue shotMaxMovementSpeed;
  public readonly LevelBasedValue shotMinMovementSpeed;

  public readonly LevelBasedValue damagePerBullet;
  public readonly int numberOfBulletsPerSalvo;
  public float TimeBetweenSalvosInSeconds { get; }

  public readonly int numberOfSalvosPerShot;
  public float ShotSpreadInDegrees { get; }

}

public class BeamWeaponConfig : WeaponConfig {
  public BeamWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    string itemDisplayName,
    LevelBasedValue baselineEnergyRequirement,
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue damagePerSecond,
    LevelBasedValue beamLifetimeInSeconds) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds, itemDisplayName, baselineEnergyRequirement) {
    this.damagePerSecond = damagePerSecond;
    this.beamCoherenceTime = beamLifetimeInSeconds;
  }

  public readonly LevelBasedValue damagePerSecond;
  public readonly LevelBasedValue beamCoherenceTime;
}

public abstract class WeaponBase : EquipmentBase {
  public float timeToNextShot = 0f;
  public new WeaponConfig Config { get; }
  public float TimeBetweenShotsInSeconds { get; }
  public float Range { get; }
  protected WeaponBase(WeaponConfig config, float level) : base(config, level) {
    Config = config;
    this.TimeBetweenShotsInSeconds = config.timeBetweenShotsInSeconds.GetLevelValue(level);
    this.Range = config.range.GetLevelValue(level);
  }

  override public EquipmentType Type { get { return EquipmentType.Weapon; } }
}

public abstract class WeaponInstance<T> : WeaponBase where T : WeaponConfig {
  public new readonly T Config;
  protected WeaponInstance(T config, float level) : base(config, level) {
    Config = config;
  }
}

public class BeamInstance : WeaponInstance<BeamWeaponConfig> {
  public float DamagePerSecond { get; }
  public float BeamLifetimeInSeconds { get; }

  public BeamInstance(BeamWeaponConfig config, float level) : base(config, level) {
    this.BeamLifetimeInSeconds = config.beamCoherenceTime.GetLevelValue(level);
    this.DamagePerSecond = config.damagePerSecond.GetLevelValue(level);
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
}