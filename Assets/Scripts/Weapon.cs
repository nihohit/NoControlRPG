using System;

public abstract class WeaponConfig : EquipmentConfigBase {
  protected WeaponConfig(LevelBasedValue range, string shotImageName, string equipmentImageName, LevelBasedValue timeBetweenShotsInSeconds, string itemDisplayName) : base(equipmentImageName, itemDisplayName) {
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
    itemDisplayName: "Laser beam"
  );

  public static BulletWeaponConfig RIFLE = new(
    range: LevelBasedValue.ConstantValue(7f),
    shotImageName: "Bullet",
    equipmentImageName: "IncendiaryGun",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(1.5f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(6f),
    damagePerBullet: LevelBasedValue.ConstantValue(2),
    itemDisplayName: "Rifle"
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
    itemDisplayName: "Machine gun"
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
    itemDisplayName: "Two-shot shotgun"
  );

  public static BeamWeaponConfig FLAMER = new(
    range: LevelBasedValue.ConstantValue(5f),
    shotImageName: "Flamer",
    equipmentImageName: "Flamer",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.0f),
    beamLifetimeInSeconds: LevelBasedValue.ConstantValue(1f),
    damagePerSecond: LevelBasedValue.ConstantValue(1f),
    itemDisplayName: "Flamer"
  );

  public static BulletWeaponConfig MISSILE = new(
    range: LevelBasedValue.ConstantValue(10f),
    shotImageName: "Missile",
    equipmentImageName: "Missile",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.0f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(7f),
    damagePerBullet: LevelBasedValue.ConstantValue(1),
    itemDisplayName: "Missile launcher"
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
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds, itemDisplayName) {
    this.shotMaxMovementSpeed = shotMaxMovementSpeed;
    this.shotMinMovementSpeed = shotMinMovementSpeed;
    this.damagePerBullet = damagePerBullet;
    this.numberOfSalvosPerShot = numberOfSalvosPerShot;
    this.numberOfBulletsPerSalvo = numberOfBulletsPerSalvo;
    this.timeBetweenSalvosInSeconds = timeBetweenSalvosInSeconds;
    this.shotSpreadInDegrees = shotSpreadInDegrees;
  }

  public BulletWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    string itemDisplayName,
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue shotMovementSpeed,
    LevelBasedValue damagePerBullet,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds, itemDisplayName) {
    this.shotMaxMovementSpeed = shotMovementSpeed;
    this.shotMinMovementSpeed = shotMovementSpeed;
    this.damagePerBullet = damagePerBullet;
    this.numberOfSalvosPerShot = numberOfSalvosPerShot;
    this.numberOfBulletsPerSalvo = numberOfBulletsPerSalvo;
    this.timeBetweenSalvosInSeconds = timeBetweenSalvosInSeconds;
    this.shotSpreadInDegrees = shotSpreadInDegrees;
  }

  public readonly LevelBasedValue shotMaxMovementSpeed;
  public readonly LevelBasedValue shotMinMovementSpeed;

  public readonly LevelBasedValue damagePerBullet;
  public readonly int numberOfBulletsPerSalvo;
  public float timeBetweenSalvosInSeconds { get; }

  public readonly int numberOfSalvosPerShot;
  public float shotSpreadInDegrees { get; }

}

public class BeamWeaponConfig : WeaponConfig {
  public BeamWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    string itemDisplayName,
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue damagePerSecond,
    LevelBasedValue beamLifetimeInSeconds) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds, itemDisplayName) {
    this.damagePerSecond = damagePerSecond;
    this.beamCoherenceTime = beamLifetimeInSeconds;
  }

  public readonly LevelBasedValue damagePerSecond;
  public readonly LevelBasedValue beamCoherenceTime;
}

public abstract class WeaponBase : EquipmentBase {
  public float timeToNextShot = 0f;
  public new WeaponConfig Config { get; }
  public float timeBetweenShotsInSeconds { get; }
  public float range { get; }
  protected WeaponBase(WeaponConfig config, float level) : base(config) {
    Config = config;
    this.timeBetweenShotsInSeconds = config.timeBetweenShotsInSeconds.getLevelValue(level);
    this.range = config.range.getLevelValue(level);
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
  public float damagePerSecond { get; }
  public float beamLifetimeInSeconds { get; }

  public BeamInstance(BeamWeaponConfig config, float level) : base(config, level) {
    this.beamLifetimeInSeconds = config.beamCoherenceTime.getLevelValue(level);
    this.damagePerSecond = config.damagePerSecond.getLevelValue(level);
  }
}

public class BulletWeaponInstance : WeaponInstance<BulletWeaponConfig> {
  [NoDisplay]
  public float shotMaxMovementSpeed { get; }

  [NoDisplay]
  public float shotMinMovementSpeed { get; }
  public float damagePerBullet { get; }

  public BulletWeaponInstance(BulletWeaponConfig config, float level) : base(config, level) {
    this.shotMaxMovementSpeed = config.shotMaxMovementSpeed.getLevelValue(level);
    this.shotMinMovementSpeed = config.shotMinMovementSpeed.getLevelValue(level);
    this.damagePerBullet = config.damagePerBullet.getLevelValue(level);
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