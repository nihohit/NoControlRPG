using System;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class ShotScript<T> : MonoBehaviour where T : WeaponConfig {
  public Guid Identifier = Guid.NewGuid();
  public T Config { get; protected set; }

  protected BattleMainManagerScript manager;
  private SpriteRenderer spriteRenderer;


  private void Awake() {
    manager = FindObjectOfType<BattleMainManagerScript>();
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  protected void Init(int layer, T config) {
    gameObject.layer = layer;
    Config = config;
  }
}
