public class EnemyConfig {
  public EnemyConfig(float health, string imageName, float speed, WeaponConfig weapon) {
    Health = health;
    ImageName = imageName;
    Speed = speed;
    Weapon = weapon;
  }
  public float Health { get; private set; }
  public float Speed { get; private set; }
  public string ImageName { get; private set; }
  public WeaponConfig Weapon { get; private set; }
}