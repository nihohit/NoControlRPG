using System;
using UnityEngine;

public class EnemyUnitScript : MonoBehaviour {
  public Guid Identifier = Guid.NewGuid();

  public float Health { get; set; }

  public void Init(float health) {
    Health = health;
  }
}