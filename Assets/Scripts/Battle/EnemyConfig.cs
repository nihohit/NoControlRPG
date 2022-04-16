using UnityEngine;

public class LevelBasedValue {
  private readonly float constant;
  private readonly float linearCoefficient;
  private readonly float exponentValue;
  private readonly float exponentCoefficient;

  public static LevelBasedValue ConstantValue(float constant) {
    return new LevelBasedValue(constant);
  }

  public static LevelBasedValue LinearValue(float coefficient) {
    return new LevelBasedValue(linearCoefficient: coefficient);
  }

  public static LevelBasedValue ExponentialValue(float value, float coefficient) {
    return new LevelBasedValue(exponentValue: value, exponentCoefficient: coefficient);
  }

  public LevelBasedValue(float constant = 0, float linearCoefficient = 0, float exponentValue = 0, float exponentCoefficient = 0) {
    this.constant = constant;
    this.linearCoefficient = linearCoefficient;
    this.exponentCoefficient = exponentCoefficient;
    this.exponentValue = exponentValue;
  }

  public float GetLevelValue(float level) {
    return constant + level * linearCoefficient + Mathf.Pow(level, exponentValue) * exponentCoefficient;
  }
}

public class EnemyConfig {
  public EnemyConfig(LevelBasedValue health, string imageName, LevelBasedValue speed, WeaponConfig weapon, float dropChance) {
    Health = health;
    ImageName = imageName;
    Speed = speed;
    Weapon = weapon;
    DropChance = dropChance;
  }
  public LevelBasedValue Health { get; }
  public LevelBasedValue Speed { get; }
  public string ImageName { get; }
  public WeaponConfig Weapon { get; }
  public float DropChance { get; }
}