using UnityEngine;

public class BeamScript : ShotScript<BeamInstance> {
  public GameObject Target { get; private set; }

  public void Init(GameObject shooter, BeamInstance config, GameObject target) {
    base.Init(shooter, config);
    Target = target;
  }

  public void OnTriggerStay2D(Collider2D other) {
    if (other.gameObject == Shooter) {
      return;
    }
    if (other.gameObject.CompareTag("Enemy")) {
      manager.BeamHitEnemy(this, other.gameObject);
    } else if (other.gameObject.CompareTag("Player")) {
      manager.BeamHitPlayer(this, other.gameObject);
    }
  }
}