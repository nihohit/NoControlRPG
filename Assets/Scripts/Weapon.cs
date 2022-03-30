public abstract class WeaponConfig {
  protected WeaponConfig(LevelBasedValue range, string shotImageName, string equipmentImageName, LevelBasedValue timeBetweenShotsInSeconds) {
    this.range = range;
    this.shotImageName = shotImageName;
    this.equipmentImageName = equipmentImageName;
    this.timeBetweenShotsInSeconds = timeBetweenShotsInSeconds;
  }

  public readonly LevelBasedValue range;
  public readonly string shotImageName;
  public readonly string equipmentImageName;
  public readonly LevelBasedValue timeBetweenShotsInSeconds;

  public static BeamWeaponConfig LASER = new(
    range: LevelBasedValue.ConstantValue(2),
    shotImageName: "Heat Beam",
    equipmentImageName: "Laser",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.5f),
    beamCoherenceTime: LevelBasedValue.ConstantValue(0.5f),
    damagePerSecond: LevelBasedValue.ConstantValue(2f)
  );

  public static BulletWeaponConfig RIFLE = new(
    range: LevelBasedValue.ConstantValue(7f),
    shotImageName: "Bullet",
    equipmentImageName: "IncendiaryGun",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(1.5f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(6f),
    damagePerBullet: LevelBasedValue.ConstantValue(2)
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
    damagePerBullet: LevelBasedValue.ConstantValue(1)
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
    damagePerBullet: LevelBasedValue.ConstantValue(0.5f)
  );

  public static BeamWeaponConfig FLAMER = new(
    range: LevelBasedValue.ConstantValue(5f),
    shotImageName: "Flamer",
    equipmentImageName: "Flamer",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.0f),
    beamCoherenceTime: LevelBasedValue.ConstantValue(1f),
    damagePerSecond: LevelBasedValue.ConstantValue(1f)
  );

  public static BulletWeaponConfig MISSILE = new(
    range: LevelBasedValue.ConstantValue(10f),
    shotImageName: "Missile",
    equipmentImageName: "Missile",
    timeBetweenShotsInSeconds: LevelBasedValue.ConstantValue(2.0f),
    shotMovementSpeed: LevelBasedValue.ConstantValue(7f),
    damagePerBullet: LevelBasedValue.ConstantValue(1)
  );
}

public class BulletWeaponConfig : WeaponConfig {
  public BulletWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue shotMaxMovementSpeed,
    LevelBasedValue shotMinMovementSpeed,
    LevelBasedValue damagePerBullet,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds) {
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
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue shotMovementSpeed,
    LevelBasedValue damagePerBullet,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds) {
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
  public readonly float timeBetweenSalvosInSeconds;

  public readonly int numberOfSalvosPerShot;
  public readonly float shotSpreadInDegrees = 0f;

}

public class BeamWeaponConfig : WeaponConfig {
  public BeamWeaponConfig(
    LevelBasedValue range,
    string shotImageName,
    string equipmentImageName,
    LevelBasedValue timeBetweenShotsInSeconds,
    LevelBasedValue damagePerSecond,
    LevelBasedValue beamCoherenceTime) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds) {
    this.damagePerSecond = damagePerSecond;
    this.beamCoherenceTime = beamCoherenceTime;
  }

  public readonly LevelBasedValue damagePerSecond;
  public readonly LevelBasedValue beamCoherenceTime;
}

public abstract class WeaponBase {
  public float timeToNextShot = 0f;
  public WeaponConfig Config { get; private set; }
  public readonly float timeBetweenShotsInSeconds;
  public readonly float range;
  protected WeaponBase(WeaponConfig config, float level) {
    Config = config;
    this.timeBetweenShotsInSeconds = config.timeBetweenShotsInSeconds.getLevelValue(level);
    this.range = config.range.getLevelValue(level);
  }
}

public abstract class WeaponInstance<T> : WeaponBase where T : WeaponConfig {
  public new readonly T Config;
  protected WeaponInstance(T config, float level) : base(config, level) {
    Config = config;
  }
}

public class BeamInstance : WeaponInstance<BeamWeaponConfig> {
  public readonly float damagePerSecond;
  public readonly float beamCoherenceTime;

  public BeamInstance(BeamWeaponConfig config, float level) : base(config, level) {
    this.beamCoherenceTime = config.beamCoherenceTime.getLevelValue(level);
    this.damagePerSecond = config.damagePerSecond.getLevelValue(level);
  }
}

public class BulletWeaponInstance : WeaponInstance<BulletWeaponConfig> {
  public readonly float shotMaxMovementSpeed;
  public readonly float shotMinMovementSpeed;
  public readonly float damagePerBullet;

  public BulletWeaponInstance(BulletWeaponConfig config, float level) : base(config, level) {
    this.shotMaxMovementSpeed = config.shotMaxMovementSpeed.getLevelValue(level);
    this.shotMinMovementSpeed = config.shotMinMovementSpeed.getLevelValue(level);
    this.damagePerBullet = config.damagePerBullet.getLevelValue(level);
  }
}