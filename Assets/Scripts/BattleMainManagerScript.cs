using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts.UnityBase;

public class BattleMainManagerScript : MonoBehaviour {
  private SpawnPool spawnPool;
  private GameObject player;
  private readonly Dictionary<Guid, EnemyUnitScript> enemies = new();
  private readonly Dictionary<Guid, BulletScript> bullets = new();
  private readonly Dictionary<Guid, BeamScript> beams = new();
  private const float TIME_BETWEEN_SPAWNS = 1;
  private float timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  private int enemyLayerMask;
  private enum Mode { Battle, Inventory }
  private Mode mode;

  private UIManagerScript uiManager;

  private readonly List<BulletScript> bulletsToRelease = new();
  private readonly List<BeamScript> beamsToRelease = new();
  private readonly List<EnemyUnitScript> enemiesToRelease = new();

  // Start is called before the first frame update
  void Start() {
    spawnPool = GetComponent<SpawnPool>();
    player = GameObject.Find("Player").gameObject;
    enemyLayerMask = LayerMask.NameToLayer("enemies");
    uiManager = GameObject.FindObjectOfType<UIManagerScript>();

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
    bulletsToRelease.AddRange(bullets.Values);
    enemiesToRelease.AddRange(enemies.Values);
  }

  internal void BulletHit(BulletScript shotScript, EnemyUnitScript enemy) {
    bulletsToRelease.Add(shotScript);
    enemiesToRelease.Add(enemy);
  }

  internal void BeamHit(BeamScript beamScript, EnemyUnitScript enemy) {
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
    foreach (var shot in bullets.Values) {
      // TODO - consider using RigidBody's movement function, instead of using kinematic rigidbodies.
      shot.gameObject.MoveForwards(shot.Config.shotMovementSpeed);
      if (!shot.InRange()) {
        bulletsToRelease.Add(shot);
      }
    }

    foreach (var beam in beams.Values) {
      beam.Lifetime -= Time.deltaTime;
      if (beam.Lifetime <= 0f) {
        beamsToRelease.Add(beam);
      }
    }
  }

  private const float BEAM_SCALE = 0.1f;

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

      if (weapon.config is BeamWeaponConfig) {
        var shot = spawnPool.GetBeam(weapon.config.shotImageName);
        beams[shot.Identifier] = shot;
        shot.transform.position = player.transform.position;
        shot.Init(player.gameObject, weapon.config as BeamWeaponConfig);
        shot.transform.localScale = new Vector3(weapon.config.range * BEAM_SCALE, BEAM_SCALE, BEAM_SCALE);
        shot.transform.RotateTowards(enemyInRange.transform.position, 360);
        shot.transform.position = player.transform.position + (enemyInRange.transform.position - player.transform.position).normalized * weapon.config.range / 2;
      } else if (weapon.config is BulletWeaponConfig) {
        var shot = spawnPool.GetBullet(weapon.config.shotImageName);
        bullets[shot.Identifier] = shot;
        shot.transform.position = player.transform.position;
        shot.Init(player.gameObject, player.transform.position, weapon.config as BulletWeaponConfig);
        shot.transform.RotateTowards(enemyInRange.transform.position, 360);
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
    foreach (var bullet in bulletsToRelease) {
      bullets.Remove(bullet.Identifier);
      spawnPool.ReturnBullet(bullet);
    }
    bulletsToRelease.Clear();

    foreach (var enemy in enemiesToRelease) {
      spawnPool.SpawnUnitExplosion(enemy.transform.position);
      enemies.Remove(enemy.Identifier);
      spawnPool.ReturnUnit(enemy);
    }
    enemiesToRelease.Clear();

    foreach (var beam in beamsToRelease) {
      beams.Remove(beam.Identifier);
      spawnPool.ReturnBeam(beam);
    }
    beamsToRelease.Clear();
  }
}
