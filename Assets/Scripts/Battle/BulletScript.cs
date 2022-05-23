using Assets.Scripts.Base;
using UnityEngine;

public class BulletScript : ShotScript<BulletWeaponInstance> {
  private bool alreadyHit;
  private Vector3 StartPoint { get; set; }
  public float Speed { get; private set; }

  public bool InRange() {
    return Vector3.Distance(StartPoint, transform.position) < Weapon.Range;
  }
  public void Init(GameObject shooter, Vector3 startPoint, BulletWeaponInstance weapon) {
    base.Init(shooter, weapon);
    StartPoint = startPoint;
    alreadyHit = false;
    Speed = (float)Randomiser.NextDouble(weapon.ShotMinMovementSpeed, weapon.ShotMaxMovementSpeed);
  }

  protected void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject == Shooter || alreadyHit) {
      return;
    }
    if (other.gameObject.CompareTag("Enemy")) {
      alreadyHit = true;
      manager.BulletHitEnemy(this, other.gameObject);
    } else if (other.gameObject.CompareTag("Player")) {
      alreadyHit = true;
      manager.BulletHitPlayer(this, other.gameObject);
    }
  }
}