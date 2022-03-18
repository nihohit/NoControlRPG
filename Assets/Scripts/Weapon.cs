public abstract class WeaponConfig {
  public float range;
  public string shotImageName;
  public string equipmentImageName;
  public float timeBetweenShotsInSeconds;

  public static BeamWeaponConfig LASER = new() {
    range = 2,
    shotImageName = "Heat Beam",
    equipmentImageName = "Laser",
    timeBetweenShotsInSeconds = 2.5f,
    beamCoherenceTime = 0.5f
  };

  public static BulletWeaponConfig RIFLE = new() {
    range = 7f,
    shotImageName = "Bullet",
    equipmentImageName = "IncendiaryGun",
    timeBetweenShotsInSeconds = 1.5f,
    shotMovementSpeed = 6f,
  };

  public static BulletWeaponConfig MACHINE_GUN = new() {
    range = 6f,
    shotImageName = "Bullet",
    equipmentImageName = "MachineGun",
    timeBetweenShotsInSeconds = 1.5f,
    shotMovementSpeed = 6f,
    numberOfSalvosPerShot = 4,
    timeBetweenSalvosInSeconds = 0.2f,
    shotSpreadInDegrees = 5,
  };

  public static BulletWeaponConfig TWO_SHOT_SHOTGUN = new() {
    range = 4f,
    shotImageName = "ShotgunPellet",
    equipmentImageName = "Shotgun",
    timeBetweenShotsInSeconds = 1.5f,
    numberOfBulletsPerSalvo = 5,
    shotMovementSpeed = 8f,
    numberOfSalvosPerShot = 3,
    timeBetweenSalvosInSeconds = 1.0f / 30f,
    shotSpreadInDegrees = 10,
  };

  public static BeamWeaponConfig FLAMER = new() {
    range = 5f,
    shotImageName = "Flamer",
    equipmentImageName = "Flamer",
    timeBetweenShotsInSeconds = 2.0f,
    beamCoherenceTime = 1f
  };

  public static BulletWeaponConfig MISSILE = new() {
    range = 10f,
    shotImageName = "Missile",
    equipmentImageName = "Missile",
    timeBetweenShotsInSeconds = 2.0f,
    shotMovementSpeed = 7f,
  };
}

public class BulletWeaponConfig : WeaponConfig {
  public float shotMovementSpeed;
  public float damagePerBullet;

  public int numberOfSalvosPerShot = 1;
  public int numberOfBulletsPerSalvo = 1;
  public float timeBetweenSalvosInSeconds = 0;
  public float shotSpreadInDegrees = 0;

}

public class BeamWeaponConfig : WeaponConfig {
  public float damagePerSecond;
  public float beamCoherenceTime;
}

public class WeaponInstance {
  public WeaponConfig config;
  public float timeToNextShot = 0f;
}