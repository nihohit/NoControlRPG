using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleMainManagerScript : MonoBehaviour {
  private SpawnPool spawnPool;
  private GameObject player;
  private readonly Dictionary<Guid, EnemyUnitScript> enemies = new Dictionary<Guid, EnemyUnitScript>();
  private readonly List<ShotScript> shots = new List<ShotScript>();
  private const float TIME_BETWEEN_SPAWNS = 1;
  private float timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  private readonly int enemyLayerMask = LayerMask.NameToLayer("enemies");

  // Start is called before the first frame update
  void Start() {
    spawnPool = GetComponent<SpawnPool>();
    player = GameObject.Find("Player").gameObject;
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
      rotateTowards(enemy.transform, playerPosition, 15.0f);
      var movementSpeed = Time.deltaTime * 1.5f;
      var direction = enemy.transform.rotation * Vector3.right;
      enemy.transform.position += direction * movementSpeed;
    }
  }

  private void rotateTowards(Transform toRotate, Vector3 targetPosition, float rotateSpeed) {
    var offset = targetPosition - toRotate.position;
    offset.z = 0;
    float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    toRotate.rotation = Quaternion.RotateTowards(toRotate.rotation, targetRotation, rotateSpeed * Time.deltaTime);
  }

  private EnemyUnitScript findEnemyInRange(float weaponRange) {
    return enemies.Values.FirstOrDefault(enemy => Vector3.Distance(enemy.transform.position, player.transform.position) < weaponRange);
  }

  private void shootEnemies() {
    foreach (var weapon in Player.Instance.Weapons) {
      weapon.timeToNextShot -= Time.deltaTime;
      if (weapon.timeToNextShot <= 0) {
        var enemyInRange = findEnemyInRange(weapon.config.range);
        if (enemyInRange == null) {
          continue;
        }

        switch (weapon.config.behavior) {
          case ShotBehavior.Beam:

            break;
          case ShotBehavior.Direct:
            var shot = spawnPool.GetShot(weapon.config.shotImageName);
            shots.Add(shot);
            shot.Init(enemyLayerMask, player.transform.position, weapon.config);
            shot.transform.rotation
            break;
        }

        weapon.timeToNextShot = weapon.config.timeBetweenShots;
      }
    }
  }

  // Update is called once per frame
  void Update() {
    spawnEnemyIfNeeded();
    moveEnemies();
    shootEnemies();
  }
}
