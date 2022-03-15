using Assets.Scripts.UnityBase;
using UnityEngine;

public class BeamScript : ShotScript<BeamWeaponConfig> {
  public float Lifetime { get; set; }

  new public void Init(GameObject shooter, BeamWeaponConfig config) {
    base.Init(shooter, config);
    Lifetime = config.beamCoherenceTime;
  }

  public void OnTriggerStay2D(Collider2D other) {
    if (other.gameObject == Shooter) {
      return;
    }
    var enemy = other.gameObject.GetComponent<EnemyUnitScript>();
    if (enemy != null) {
      manager.BeamHit(this, enemy);
    }
  }
}