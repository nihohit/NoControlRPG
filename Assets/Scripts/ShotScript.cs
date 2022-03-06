using System;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class ShotScript : MonoBehaviour {
  public Guid Identifier = Guid.NewGuid();
  public WeaponConfig Config { get; private set; }
  public Vector3 StartPoint { get; private set; }

  private BattleMainManagerScript manager;
  private SpriteRenderer spriteRenderer;

  private bool alreadyHit;

  private void Awake() {
    manager = FindObjectOfType<BattleMainManagerScript>();
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  public void Init(int layer, Vector3 startPoint, WeaponConfig config, TextureHandler textureHandler) {
    if (Config == null || Config.shotImageName != config.shotImageName) {
      textureHandler.UpdateTexture(config.shotImageName, spriteRenderer, "Images/VisualEffects");
      gameObject.name = config.shotImageName;
    }
    gameObject.layer = layer;
    StartPoint = startPoint;
    Config = config;
    alreadyHit = false;
  }

  public bool InRange() {
    return Vector3.Distance(StartPoint, transform.position) < Config.range;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (alreadyHit) {
      return;
    }
    alreadyHit = true;
    var enemy = other.gameObject.GetComponent<EnemyUnitScript>();
    if (enemy != null) {
      manager.ShotHit(this, enemy);
    }
  }
}