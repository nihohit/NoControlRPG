using System;
using UnityEngine;

public class ShotScript<T> : MonoBehaviour where T : WeaponConfig {
  public Guid Identifier = Guid.NewGuid();
  public T Config { get; protected set; }

  public GameObject Shooter { get; private set; }
  protected BattleMainManagerScript manager;


  private void Awake() {
    manager = FindObjectOfType<BattleMainManagerScript>();
  }

  protected void Init(GameObject shooter, T config) {
    Config = config;
    Shooter = shooter;
  }
}
