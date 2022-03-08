using Assets.Scripts.UnityBase;
using UnityEngine;

public class BeamScript : ShotScript<BeamWeaponConfig> {
  public float StartTime { get; private set; }

  public void Init(int layer, BeamWeaponConfig config, TextureHandler textureHandler, float startTime) {
    base.Init(layer, config, textureHandler);
    transform.localScale = new Vector3(config.range * 0.1f, 0.1f, 0.1f);
    StartTime = startTime;
  }

}