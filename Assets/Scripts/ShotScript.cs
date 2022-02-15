using System;
using UnityEngine;

public class ShotScript : MonoBehaviour {
  public Guid Identifier = Guid.NewGuid();
  public WeaponConfig Config { get; private set; }
  public Vector3 StartPoint { get; private set; }

  private BattleMainManagerScript manager;

  public void Init(int layer, Vector3 startPoint, WeaponConfig config, BattleMainManagerScript manager) {
    gameObject.layer = layer;
    StartPoint = startPoint;
    Config = config;
    this.manager = manager;
  }

  public bool InRange() {
    return Vector3.Distance(StartPoint, transform.position) < Config.range;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    var enemy = other.gameObject.GetComponent<EnemyUnitScript>();
    if (enemy != null) {
      manager.ShotHit(this, enemy);
    }
  }
}