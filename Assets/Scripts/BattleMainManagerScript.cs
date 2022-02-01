using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMainManagerScript : MonoBehaviour {
  private SpawnPool spawnPool;

  // Start is called before the first frame update
  void Start() {
    spawnPool = GetComponent<SpawnPool>();
  }

  // Update is called once per frame
  void Update() {

  }
}
