using Assets.Scripts.Base;
using UnityEngine;

public class BulletScript : ShotScript<BulletWeaponInstance> {
  private bool alreadyHit;
  public Vector3 StartPoint { get; private set; }
  public float Speed { get; private set; }

  public bool InRange() {
    return Vector3.Distance(StartPoint, transform.position) < Weapon.range;
  }
  public void Init(GameObject shooter, Vector3 startPoint, BulletWeaponInstance weapon) {
    base.Init(shooter, weapon);
    StartPoint = startPoint;
    alreadyHit = false;
    Speed = (float)Randomiser.NextDouble(weapon.shotMinMovementSpeed, weapon.shotMaxMovementSpeed);
  }

  protected void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject == Shooter || alreadyHit) {
      return;
    }
    if (other.gameObject.tag == "Enemy") {
      alreadyHit = true;
      manager.BulletHitEnemy(this, other.gameObject);
    } else if (other.gameObject.tag == "Player") {
      alreadyHit = true;
      manager.BulletHitPlayer(this, other.gameObject);
    }
  }
}