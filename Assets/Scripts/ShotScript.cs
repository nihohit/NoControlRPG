using System;
using Assets.Scripts.Base;
using UnityEngine;

public class ShotScript<T> : MonoBehaviour where T : WeaponBase {
  public Guid Identifier = Guid.NewGuid();
  public T Weapon { get; protected set; }

  public GameObject Shooter { get; private set; }
  protected BattleMainManagerScript manager;

  private void Awake() {
    manager = FindObjectOfType<BattleMainManagerScript>();
  }

  protected void Init(GameObject shooter, T weapon) {
    Weapon = weapon;
    Shooter = shooter;
  }
}
