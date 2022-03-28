using System.Collections.Generic;
using System.Collections;
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

  private readonly HashSet<BulletScript> bulletsToRelease = new();
  private readonly HashSet<BeamScript> beamsToRelease = new();

  private readonly List<EnemyConfig> enemyConfigs = new() {
    new EnemyConfig(LevelBasedValue.ConstantValue(3f), "ScoutMech", LevelBasedValue.ConstantValue(15f), WeaponConfig.TWO_SHOT_SHOTGUN),
    new EnemyConfig(LevelBasedValue.ConstantValue(3f), "ScoutMech", LevelBasedValue.ConstantValue(15f), WeaponConfig.FLAMER),
    new EnemyConfig(LevelBasedValue.ConstantValue(5f), "HeavyMech", LevelBasedValue.ConstantValue(10f), WeaponConfig.RIFLE),
    new EnemyConfig(LevelBasedValue.ConstantValue(5f), "HeavyMech", LevelBasedValue.ConstantValue(10f), WeaponConfig.MISSILE)
  };

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
    bullets.Values.ForEach(bullet => bulletsToRelease.Add(bullet));
    beams.Values.ForEach(beam => beamsToRelease.Add(beam));
    // pretty ugly hack, but it works
    enemies.Values.ForEach(enemy => enemy.Health = 0);
  }

  internal void BulletHitEnemy(BulletScript shotScript, GameObject enemy) {
    bulletsToRelease.Add(shotScript);
    enemy.GetComponent<EnemyUnitScript>().Health -= shotScript.Weapon.damagePerBullet;
  }

  internal void BulletHitPlayer(BulletScript shotScript, GameObject player) {
    bulletsToRelease.Add(shotScript);
    Player.Instance.CurrentHealth -= shotScript.Weapon.damagePerBullet;
  }

  internal void BeamHitPlayer(BeamScript beamScript, GameObject player) {
    Player.Instance.CurrentHealth -= beamScript.Weapon.damagePerSecond * Time.deltaTime;
  }

  internal void BeamHitEnemy(BeamScript beamScript, GameObject enemy) {
    enemy.GetComponent<EnemyUnitScript>().Health -= beamScript.Weapon.damagePerSecond * Time.deltaTime;
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
    var config = enemyConfigs.ChooseRandomValue();
    var newEnemy = spawnPool.GetUnit(config.ImageName);
    var verticalSize = Camera.main.orthographicSize;
    var horizontalSize = verticalSize * Screen.width / Screen.height;
    var distance = Mathf.Sqrt(Mathf.Pow(verticalSize, 2) + Mathf.Pow(horizontalSize, 2)) + 0.1f;
    newEnemy.transform.position = UnityEngine.Random.insideUnitCircle.normalized * distance;
    newEnemy.Init(config, 1f);
    enemies[newEnemy.Identifier] = newEnemy;
    timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  }

  private void MoveEnemies() {
    var playerPosition = player.transform.position;
    foreach (var enemy in enemies.Values) {
      enemy.transform.RotateTowards(playerPosition, enemy.Speed * Time.deltaTime);
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
    beam.transform.position += beam.transform.right.normalized * beam.Weapon.range / 2;
  }

  private void MoveShots() {
    foreach (var shot in bullets.Values) {
      // TODO - consider using RigidBody's movement function, instead of using kinematic rigidbodies.
      shot.gameObject.MoveForwards(shot.Weapon.shotMovementSpeed);
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

  private void CreateBullet(GameObject shooter, BulletWeaponInstance weapon, Vector3 to) {
    var bullet = spawnPool.GetBullet(weapon.SpecializedConfig.shotImageName);
    bullets[bullet.Identifier] = bullet;
    bullet.transform.position = shooter.transform.position;
    bullet.Init(shooter, shooter.transform.position, weapon);
    bullet.transform.RotateTowards(to, 360, (float)Randomiser.NextDouble(-weapon.SpecializedConfig.shotSpreadInDegrees, weapon.SpecializedConfig.shotSpreadInDegrees));
  }

  private void ShootBulletsSalvo(GameObject shooter, BulletWeaponInstance weapon, Vector3 to) {
    for (int i = 0; i < weapon.SpecializedConfig.numberOfBulletsPerSalvo; ++i) {
      CreateBullet(shooter, weapon, to);
    }
  }

  private IEnumerator ShootBulletsSalvos(GameObject shooter, BulletWeaponInstance weapon, Vector3 to) {
    int salvoCount = weapon.SpecializedConfig.numberOfSalvosPerShot;
    while (true) {
      ShootBulletsSalvo(shooter, weapon, to);
      if (--salvoCount > 0) {
        yield return new WaitForSeconds(weapon.SpecializedConfig.timeBetweenSalvosInSeconds);
      } else {
        yield break;
      }
    }
  }

  private void CreateBeam(GameObject shooter, BeamInstance weapon, GameObject target) {
    var beam = spawnPool.GetBeam(weapon.SpecializedConfig.shotImageName);
    beams[beam.Identifier] = beam;
    beam.transform.position = shooter.transform.position;
    beam.Init(shooter, weapon, target);
    beam.transform.localScale = new Vector3(weapon.range * BEAM_SCALE, BEAM_SCALE, BEAM_SCALE);
    AdjustBeamPosition(beam, 360);
  }

  private void CreateShot(GameObject shooter, WeaponBase weapon, GameObject target) {
    if (weapon is BeamInstance beam) {
      CreateBeam(shooter, beam, target);
    } else if (weapon is BulletWeaponInstance bullet) {
      StartCoroutine(ShootBulletsSalvos(shooter, bullet, target.transform.position));
    }
  }

  private void ShootWeapon(GameObject shooter, WeaponBase weapon, GameObject target) {
    CreateShot(shooter, weapon, target);
    weapon.timeToNextShot = weapon.timeBetweenShotsInSeconds;
  }

  private void ShootEnemies() {
    foreach (var weapon in Player.Instance.Weapons) {
      weapon.timeToNextShot -= Time.deltaTime;
      if (weapon.timeToNextShot > 0) {
        continue;
      }
      var enemyInRange = FindEnemyInRange(weapon.range);
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
      if (weapon.timeToNextShot > 0 || Vector3.Distance(enemy.transform.position, player.transform.position) > weapon.range) {
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

    if (Player.Instance.CurrentHealth <= 0) {
      SwitchToInventory();
    }

    SpawnEnemyIfNeeded();
    MoveEnemies();
    ShootEnemies();
    ShootPlayer();
    MoveShots();
    uiManager.UpdatePlayerHealthOverlay();
  }

  private void ReleaseEntities() {
    foreach (var bullet in bulletsToRelease) {
      bullets.Remove(bullet.Identifier);
      spawnPool.ReturnBullet(bullet);
    }
    bulletsToRelease.Clear();

    var enemiesToRelease = enemies.Where(pair => pair.Value.Health <= 0).ToList();
    foreach (var (key, enemy) in enemiesToRelease) {
      spawnPool.SpawnUnitExplosion(enemy.transform.position);
      enemies.Remove(key);
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
