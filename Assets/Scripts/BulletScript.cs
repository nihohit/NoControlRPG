using System;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class BulletScript : ShotScript<BulletWeaponConfig> {
  private bool alreadyHit;
  public Vector3 StartPoint { get; private set; }

  public bool InRange() {
    return Vector3.Distance(StartPoint, transform.position) < Config.range;
  }
  public void Init(GameObject shooter, Vector3 startPoint, BulletWeaponConfig config) {
    base.Init(shooter, config);
    StartPoint = startPoint;
    alreadyHit = false;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject == Shooter || alreadyHit) {
      return;
    }
    var enemy = other.gameObject.GetComponent<EnemyUnitScript>();
    if (enemy != null) {
      alreadyHit = true;
      manager.BulletHit(this, enemy);
    }
  }
}