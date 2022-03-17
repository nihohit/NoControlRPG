using System;
using UnityEngine;

public class EnemyUnitScript : MonoBehaviour {
  public Guid Identifier = Guid.NewGuid();

  public float Health { get; set; }

  public WeaponInstance Weapon { get; private set; }

  public void Init(EnemyConfig config) {
    Health = config.Health;
    Weapon = new WeaponInstance {
      config = config.Weapon
    };
  }
}