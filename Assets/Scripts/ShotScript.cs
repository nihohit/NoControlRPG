using UnityEngine;

public class ShotScript : MonoBehaviour {
  public string ShotName { get; set; }

  public void Init(int layer) {
    gameObject.layer = layer;
  }

  private void OnTriggerEnter(Collider other) {
    Debug.Log("Collision");
  }
}