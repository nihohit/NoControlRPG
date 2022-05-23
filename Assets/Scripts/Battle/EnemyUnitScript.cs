using System;
using UnityEngine;

public class EnemyUnitScript : MonoBehaviour {
  public Guid Identifier { get; } = Guid.NewGuid();

  public float Health { get; set; }
  public float Speed { get; private set; }

  public WeaponBase Weapon { get; private set; }

  public float Level { get; private set; }

  public EnemyConfig Config { get; private set; }

  public void Init(EnemyConfig config, float level) {
    Level = level;
    Health = config.Health.GetLevelValue(level);
    Speed = config.Speed.GetLevelValue(level);
    Weapon = (config.Weapon is BeamWeaponConfig beam) ?
      new BeamInstance(beam, level) :
      new BulletWeaponInstance(config.Weapon as BulletWeaponConfig, level);
    Config = config;
  }
}