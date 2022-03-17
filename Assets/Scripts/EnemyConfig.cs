public class EnemyConfig {
  public EnemyConfig(float health, WeaponConfig weapon) {
    Health = health;
    Weapon = weapon;
  }
  public float Health { get; private set; }
  public WeaponConfig Weapon { get; private set; }
}