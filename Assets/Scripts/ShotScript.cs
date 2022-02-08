using UnityEngine;

public class ShotScript : MonoBehaviour {
  public WeaponConfig Config { get; private set; }
  public Vector3 StartPoint { get; private set; }

  public void Init(int layer, Vector3 startPoint, WeaponConfig config) {
    gameObject.layer = layer;
    StartPoint = startPoint;
    Config = config;
  }

  private void OnTriggerEnter(Collider other) {
    Debug.Log("Collision");
  }
}