using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMainManagerScript : MonoBehaviour {
  private SpawnPool spawnPool;
  private GameObject player;
  private List<EnemyUnitScript> enemies = new List<EnemyUnitScript>();
  private static readonly float TIME_BETWEEN_SPAWNS = 1;
  private float timeToNextSpawn = TIME_BETWEEN_SPAWNS;

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
    newEnemy.transform.position = Random.insideUnitCircle.normalized * distance;
    enemies.Add(newEnemy);
    timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  }

  private void moveEnemies() {
    var playerPosition = player.transform.position;
    foreach (var enemy in enemies) {
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

  // Update is called once per frame
  void Update() {
    spawnEnemyIfNeeded();
    moveEnemies();
  }
}
