using System;
using UnityEngine;

public class EnemyUnitScript : MonoBehaviour {
  public Guid Identifier = Guid.NewGuid();

  public float Health { get; set; }

  public WeaponBase Weapon { get; private set; }

  public void Init(EnemyConfig config, float level) {
    Health = config.Health;
    Weapon = (config.Weapon is BeamWeaponConfig beam) ?
      new BeamInstance(beam, level) :
      new BulletWeaponInstance(config.Weapon as BulletWeaponConfig, level);
  }
}