public abstract class WeaponConfig {
  protected WeaponConfig(float range, string shotImageName, string equipmentImageName, float timeBetweenShotsInSeconds) {
    this.range = range;
    this.shotImageName = shotImageName;
    this.equipmentImageName = equipmentImageName;
    this.timeBetweenShotsInSeconds = timeBetweenShotsInSeconds;
  }

  public readonly float range;
  public readonly string shotImageName;
  public readonly string equipmentImageName;
  public readonly float timeBetweenShotsInSeconds;

  public static BeamWeaponConfig LASER = new(
    range: 2,
    shotImageName: "Heat Beam",
    equipmentImageName: "Laser",
    timeBetweenShotsInSeconds: 2.5f,
    beamCoherenceTime: 0.5f,
    damagePerSecond: 2f
  );

  public static BulletWeaponConfig RIFLE = new(
    range: 7f,
    shotImageName: "Bullet",
    equipmentImageName: "IncendiaryGun",
    timeBetweenShotsInSeconds: 1.5f,
    shotMovementSpeed: 6f,
    damagePerBullet: 2
  );

  public static BulletWeaponConfig MACHINE_GUN = new(
    range: 6f,
    shotImageName: "Bullet",
    equipmentImageName: "MachineGun",
    timeBetweenShotsInSeconds: 1.5f,
    shotMovementSpeed: 6f,
    numberOfSalvosPerShot: 4,
    timeBetweenSalvosInSeconds: 0.2f,
    shotSpreadInDegrees: 5,
    damagePerBullet: 1
  );

  public static BulletWeaponConfig TWO_SHOT_SHOTGUN = new(
    range: 4f,
    shotImageName: "ShotgunPellet",
    equipmentImageName: "Shotgun",
    timeBetweenShotsInSeconds: 1.5f,
    numberOfBulletsPerSalvo: 5,
    shotMovementSpeed: 8f,
    numberOfSalvosPerShot: 3,
    timeBetweenSalvosInSeconds: 1.0f / 30f,
    shotSpreadInDegrees: 10,
    damagePerBullet: 0.5f
  );

  public static BeamWeaponConfig FLAMER = new(
    range: 5f,
    shotImageName: "Flamer",
    equipmentImageName: "Flamer",
    timeBetweenShotsInSeconds: 2.0f,
    beamCoherenceTime: 1f,
    damagePerSecond: 1f
  );

  public static BulletWeaponConfig MISSILE = new(
    range: 10f,
    shotImageName: "Missile",
    equipmentImageName: "Missile",
    timeBetweenShotsInSeconds: 2.0f,
    shotMovementSpeed: 7f,
    damagePerBullet: 1
  );
}

public class BulletWeaponConfig : WeaponConfig {
  public BulletWeaponConfig(
    float range,
    string shotImageName,
    string equipmentImageName,
    float timeBetweenShotsInSeconds,
    float shotMovementSpeed,
    float damagePerBullet,
    int numberOfSalvosPerShot = 1,
    int numberOfBulletsPerSalvo = 1,
    float timeBetweenSalvosInSeconds = 0f,
    float shotSpreadInDegrees = 0f) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds) {
    this.shotMovementSpeed = shotMovementSpeed;
    this.damagePerBullet = damagePerBullet;
    this.numberOfSalvosPerShot = numberOfSalvosPerShot;
    this.numberOfBulletsPerSalvo = numberOfBulletsPerSalvo;
    this.timeBetweenSalvosInSeconds = timeBetweenSalvosInSeconds;
    this.shotSpreadInDegrees = shotSpreadInDegrees;
  }
  public readonly float shotMovementSpeed;
  public readonly float damagePerBullet;
  public readonly int numberOfBulletsPerSalvo;
  public readonly float timeBetweenSalvosInSeconds;

  public readonly int numberOfSalvosPerShot;
  public readonly float shotSpreadInDegrees = 0;

}

public class BeamWeaponConfig : WeaponConfig {
  public BeamWeaponConfig(
    float range,
    string shotImageName,
    string equipmentImageName,
    float timeBetweenShotsInSeconds,
    float damagePerSecond,
    float beamCoherenceTime) : base(range, shotImageName, equipmentImageName, timeBetweenShotsInSeconds) {
    this.damagePerSecond = damagePerSecond;
    this.beamCoherenceTime = beamCoherenceTime;
  }

  public readonly float damagePerSecond;
  public readonly float beamCoherenceTime;
}

public class WeaponInstance {
  public WeaponInstance(WeaponConfig config) {
    this.config = config;
  }

  public readonly WeaponConfig config;
  public float timeToNextShot = 0f;
}