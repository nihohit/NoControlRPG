using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts.UnityBase;

public class BattleMainManagerScript : MonoBehaviour {
  private SpawnPool spawnPool;
  private GameObject player;
  private readonly Dictionary<Guid, EnemyUnitScript> enemies = new();
  private readonly Dictionary<Guid, ShotScript> shots = new();
  private const float TIME_BETWEEN_SPAWNS = 1;
  private float timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  private int enemyLayerMask;
  private enum Mode { Battle, Inventory }
  private Mode mode;

  private UIManagerScript uiManager;

  private readonly List<ShotScript> shotsToRelease = new();
  private readonly List<EnemyUnitScript> enemiesToRelease = new();

  // Start is called before the first frame update
  void Start() {
    spawnPool = GetComponent<SpawnPool>();
    player = GameObject.Find("Player").gameObject;
    enemyLayerMask = LayerMask.NameToLayer("enemies");
    uiManager = GameObject.Find("Canvas").GetComponent<UIManagerScript>();

    SwitchToInventory();
  }

  public void SwitchContext() {
    switch (mode) {
      case Mode.Battle:
        SwitchToInventory();
        break;
      case Mode.Inventory:
        SwitchToBattle();
        break;
    }
  }

  private void SwitchToBattle() {
    mode = Mode.Battle;
    uiManager.ToBattleMode();
  }

  private void SwitchToInventory() {
    releaseAllEntities();
    mode = Mode.Inventory;
    uiManager.ToInventoryMode();
  }

  private void releaseAllEntities() {
    shotsToRelease.AddRange(shots.Values);
    enemiesToRelease.AddRange(enemies.Values);
  }

  internal void ShotHit(ShotScript shotScript, EnemyUnitScript enemy) {
    shotsToRelease.Add(shotScript);
    enemiesToRelease.Add(enemy);
  }

  private void spawnEnemyIfNeeded() {
    const int TARGET_NUMBER_OF_ENEMIES = 50;
    if (enemies.Count >= TARGET_NUMBER_OF_ENEMIES) {
      return;
    }
    timeToNextSpawn -= Time.deltaTime;
    if (timeToNextSpawn > 0) {
      return;
    }
    var newEnemy = spawnPool.GetUnit();
    var verticalSize = Camera.main.orthographicSize;
    var horizontalSize = verticalSize * Screen.width / Screen.height;
    var distance = Mathf.Sqrt(Mathf.Pow(verticalSize, 2) + Mathf.Pow(horizontalSize, 2)) + 0.1f;
    newEnemy.transform.position = UnityEngine.Random.insideUnitCircle.normalized * distance;
    enemies[newEnemy.Identifier] = newEnemy;
    timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  }

  private void moveEnemies() {
    var playerPosition = player.transform.position;
    foreach (var enemy in enemies.Values) {
      enemy.transform.RotateTowards(playerPosition, 15.0f * Time.deltaTime);
      enemy.gameObject.MoveForwards(1.5f);
    }
  }

  private EnemyUnitScript findEnemyInRange(float weaponRange) {
    return enemies.Values.FirstOrDefault(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < weaponRange);
  }

  private void moveShots() {
    foreach (var shot in shots.Values) {
      // TODO - consider using RigidBody's movement function, instead of using kinematic rigidbodies.
      shot.gameObject.MoveForwards(shot.Config.shotMovementSpeed);
      if (!shot.InRange()) {
        shotsToRelease.Add(shot);
      }
    }
  }

  private void shootEnemies() {
    foreach (var weapon in Player.Instance.Weapons) {
      weapon.timeToNextShot -= Time.deltaTime;
      if (weapon.timeToNextShot > 0) {
        continue;
      }
      var enemyInRange = findEnemyInRange(weapon.config.range);
      if (enemyInRange == null) {
        continue;
      }

      switch (weapon.config.behavior) {
        case ShotBehavior.Beam:

          break;
        case ShotBehavior.Direct:
          var shot = spawnPool.GetShot(weapon.config.shotImageName);
          shots[shot.Identifier] = shot;
          shot.transform.position = player.transform.position;
          shot.Init(enemyLayerMask, player.transform.position, weapon.config, this);
          shot.transform.RotateTowards(enemyInRange.transform.position, 360);
          break;
      }

      weapon.timeToNextShot = weapon.config.timeBetweenShots;
    }
  }

  // Update is called once per frame
  void Update() {
    releaseEntities();

    if (mode != Mode.Battle) {
      return;
    }
    spawnEnemyIfNeeded();
    moveEnemies();
    moveShots();
    shootEnemies();
  }

  private void releaseEntities() {
    foreach (var shot in shotsToRelease) {
      shots.Remove(shot.Identifier);
      spawnPool.ReturnShot(shot);
    }
    shotsToRelease.Clear();

    foreach (var enemy in enemiesToRelease) {
      enemies.Remove(enemy.Identifier);
      spawnPool.ReturnUnit(enemy);
    }
    enemiesToRelease.Clear();
  }
}
