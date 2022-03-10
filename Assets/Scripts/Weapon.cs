public abstract class WeaponConfig {
  public float range;
  public string shotImageName;
  public string equipmentImageName;
  public float timeBetweenShots;

  public static BeamWeaponConfig LASER = new() {
    range = 2,
    shotImageName = "Heat Beam",
    equipmentImageName = "Laser",
    timeBetweenShots = 2.5f,
    beamCoherenceTime = 0.5f
  };

  public static BulletWeaponConfig MACHINE_GUN = new() {
    range = 4f,
    shotImageName = "IncendiaryGun",
    equipmentImageName = "IncendiaryGun",
    timeBetweenShots = 1.5f,
    shotMovementSpeed = 6f,
  };

  public static BeamWeaponConfig FLAMER = new() {
    range = 5f,
    shotImageName = "Flamer",
    equipmentImageName = "Flamer",
    timeBetweenShots = 2.0f,
    beamCoherenceTime = 1f
  };

  public static BulletWeaponConfig MISSILE = new() {
    range = 10f,
    shotImageName = "Missile",
    equipmentImageName = "Missile",
    timeBetweenShots = 2.0f,
    shotMovementSpeed = 7f,
  };
}

public class BulletWeaponConfig : WeaponConfig {
  public float shotMovementSpeed;
  public float damage;
}

public class BeamWeaponConfig : WeaponConfig {
  public float damagePerSecond;
  public float beamCoherenceTime;
}

public class WeaponInstance {
  public WeaponConfig config;
  public float timeToNextShot = 0f;
}