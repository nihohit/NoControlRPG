using Assets.Scripts.UnityBase;
using UnityEngine;

public class BeamScript : ShotScript<BeamWeaponConfig> {
  public float StartTime { get; private set; }

  public void Init(int layer, BeamWeaponConfig config, TextureHandler textureHandler, float startTime) {
    base.Init(layer, config, textureHandler);
    StartTime = startTime;
  }

}