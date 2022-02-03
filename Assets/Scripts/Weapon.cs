public enum ShotBehavior { Direct, Beam }

public class WeaponConfig {
  public float range;
  public ShotBehavior behavior;
  public string shotImageName;
  public float shotMovementSpeed;
  public float timeBetweenShots;

  public static WeaponConfig LASER = new WeaponConfig {
    range = 2,
    shotImageName = "VisualEffects/Heat Beam",
    behavior = ShotBehavior.Beam,
    timeBetweenShots = 2.5f
  };

  public static WeaponConfig MACHINE_GUN = new WeaponConfig {
    range = 1.5f,
    shotImageName = "VisualEffects/Incendiary Gun",
    behavior = ShotBehavior.Direct,
    timeBetweenShots = 1.5f,
    shotMovementSpeed = 3f,
  };
}

public class WeaponInstance {
  public WeaponConfig config;
  public float timeToNextShot;
}