using Assets.Scripts.UnityBase;
using UnityEngine;

public class BeamScript : ShotScript<BeamWeaponConfig> {
  public float Lifetime { get; set; }

  new public void Init(int layer, BeamWeaponConfig config, TextureHandler textureHandler) {
    base.Init(layer, config, textureHandler);
    Lifetime = config.beamCoherenceTime;
  }

  void OnTriggerStay2D(Collider2D other) {
    var enemy = other.gameObject.GetComponent<EnemyUnitScript>();
    if (enemy != null) {
      manager.BeamHit(this, enemy);
    }
  }
}