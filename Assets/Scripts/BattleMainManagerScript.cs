using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts.UnityBase;
using Assets.Scripts.Base;

public class BattleMainManagerScript : MonoBehaviour {
  private SpawnPool spawnPool;
  private GameObject player;
  private readonly Dictionary<Guid, EnemyUnitScript> enemies = new();
  private readonly Dictionary<Guid, BulletScript> bullets = new();
  private readonly Dictionary<Guid, BeamScript> beams = new();
  private const float TIME_BETWEEN_SPAWNS = 1;
  private float timeToNextSpawn = TIME_BETWEEN_SPAWNS;
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
    ReleaseAllEntities();
    mode = Mode.Inventory;
    uiManager.ToInventoryMode();
  }

  private void ReleaseAllEntities() {
    bulletsToRelease.AddRange(bullets.Values);
    enemiesToRelease.AddRange(enemies.Values);
  }

  internal void BulletHitEnemy(BulletScript shotScript, GameObject enemy) {
    bulletsToRelease.Add(shotScript);
    enemiesToRelease.Add(enemy.GetComponent<EnemyUnitScript>());
  }

  internal void BulletHitPlayer(BulletScript shotScript, GameObject player) {
    bulletsToRelease.Add(shotScript);
  }

  internal void BeamHitPlayer(BeamScript beamScript, GameObject player) {
  }

  internal void BeamHitEnemy(BeamScript beamScript, GameObject enemy) {
    enemiesToRelease.Add(enemy.GetComponent<EnemyUnitScript>());
  }

  private void SpawnEnemyIfNeeded() {
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
    newEnemy.Init(new EnemyConfig(1f, Randomiser.NextBool() ? WeaponConfig.MACHINE_GUN : WeaponConfig.LASER));
    enemies[newEnemy.Identifier] = newEnemy;
    timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  }

  private void MoveEnemies() {
    var playerPosition = player.transform.position;
    foreach (var enemy in enemies.Values) {
      enemy.transform.RotateTowards(playerPosition, 15.0f * Time.deltaTime);
      enemy.gameObject.MoveForwards(1.5f);
    }
  }

  private EnemyUnitScript FindEnemyInRange(float weaponRange) {
    return enemies.Values.FirstOrDefault(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < weaponRange);
  }

  private void AdjustBeamPosition(BeamScript beam, float rotationSpeed) {
    // If position isn't moved far from target before rotation, the rotation will go nuts.
    beam.transform.position = beam.Shooter.transform.position;
    beam.transform.RotateTowards(beam.Target.transform.position, rotationSpeed);
    beam.transform.position += beam.transform.right.normalized * beam.Config.range / 2;
  }

  private void MoveShots() {
    foreach (var shot in bullets.Values) {
      // TODO - consider using RigidBody's movement function, instead of using kinematic rigidbodies.
      shot.gameObject.MoveForwards(shot.Config.shotMovementSpeed);
      if (!shot.InRange()) {
        bulletsToRelease.Add(shot);
      }
    }

    foreach (var beam in beams.Values) {
      beam.Lifetime -= Time.deltaTime;
      if (beam.Lifetime <= 0f || !beam.Shooter.activeSelf) {
        beamsToRelease.Add(beam);
      } else if (beam.Target.activeSelf) {
        AdjustBeamPosition(beam, 360);
      }
    }
  }

  private const float BEAM_SCALE = 0.1f;

  private void CreateBullet(GameObject shooter, BulletWeaponConfig weapon, Vector3 to) {
    var bullet = spawnPool.GetBullet(weapon.shotImageName);
    bullets[bullet.Identifier] = bullet;
    bullet.transform.position = shooter.transform.position;
    bullet.Init(shooter, shooter.transform.position, weapon);
    bullet.transform.RotateTowards(to, 360);
  }

  private void CreateBeam(GameObject shooter, BeamWeaponConfig weapon, GameObject target) {
    var beam = spawnPool.GetBeam(weapon.shotImageName);
    beams[beam.Identifier] = beam;
    beam.transform.position = shooter.transform.position;
    beam.Init(shooter, weapon, target);
    beam.transform.localScale = new Vector3(weapon.range * BEAM_SCALE, BEAM_SCALE, BEAM_SCALE);
    AdjustBeamPosition(beam, 360);
  }

  private void CreateShot(GameObject shooter, WeaponConfig weapon, GameObject target) {
    if (weapon is BeamWeaponConfig beam) {
      CreateBeam(shooter, beam, target);
    } else if (weapon is BulletWeaponConfig bullet) {
      CreateBullet(shooter, bullet, target.transform.position);
    }
  }

  private void ShootWeapon(GameObject shooter, WeaponInstance weapon, GameObject target) {
    CreateShot(shooter, weapon.config, target);
    weapon.timeToNextShot = weapon.config.timeBetweenShots;
  }

  private void ShootEnemies() {
    foreach (var weapon in Player.Instance.Weapons) {
      weapon.timeToNextShot -= Time.deltaTime;
      if (weapon.timeToNextShot > 0) {
        continue;
      }
      var enemyInRange = FindEnemyInRange(weapon.config.range);
      if (enemyInRange == null) {
        continue;
      }

      ShootWeapon(player, weapon, enemyInRange.gameObject);
    }
  }

  private void ShootPlayer() {
    foreach (var enemy in enemies.Values) {
      var weapon = enemy.Weapon;
      weapon.timeToNextShot -= Time.deltaTime;
      if (weapon.timeToNextShot > 0 || Vector3.Distance(enemy.transform.position, player.transform.position) > weapon.config.range) {
        continue;
      }
      ShootWeapon(enemy.gameObject, weapon, player);
    }
  }

  // Update is called once per frame
  void Update() {
    ReleaseEntities();

    if (mode != Mode.Battle) {
      return;
    }
    SpawnEnemyIfNeeded();
    MoveEnemies();
    ShootEnemies();
    ShootPlayer();
    MoveShots();
  }

  private void ReleaseEntities() {
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
