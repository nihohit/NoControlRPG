using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleMainManagerScript : MonoBehaviour {
  private SpawnPool spawnPool;
  private GameObject playerGameObject;
  private readonly Dictionary<Guid, EnemyUnitScript> enemies = new();
  private readonly Dictionary<Guid, BulletScript> bullets = new();
  private readonly Dictionary<Guid, BeamScript> beams = new();
  private const float TIME_BETWEEN_SPAWNS = 1;
  private float timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  private Mode mode = Mode.Start;
  private UIManagerScript uiManager;
  private float roundStartTime;
  private readonly HashSet<BulletScript> bulletsToRelease = new();
  private readonly HashSet<BeamScript> beamsToRelease = new();
  protected void Awake() {
    mainCamera = Camera.main;
  }

  private static EnemyConfig MakeScoutMech(WeaponConfig weaponConfig) {
    return new EnemyConfig(health: new LevelBasedValue(constant: 2f, linearCoefficient: 1),
                    imageName: "ScoutMech",
                    speed: LevelBasedValue.ConstantValue(15f),
                    weapon: weaponConfig,
                    dropChance: 0.1f);
  }

  private static EnemyConfig MakeHeavyMech(WeaponConfig weaponConfig) {
    return new EnemyConfig(health: new LevelBasedValue(constant: 4f, linearCoefficient: 1.5f),
                    imageName: "HeavyMech",
                    speed: LevelBasedValue.ConstantValue(10f),
                    weapon: weaponConfig,
                    dropChance: 0.2f);
  }

  private readonly List<EnemyConfig> enemyConfigs = new() {
    MakeScoutMech(WeaponConfig.TWO_SHOT_SHOTGUN),
    MakeScoutMech(WeaponConfig.FLAMER),
    MakeHeavyMech(WeaponConfig.RIFLE),
    MakeHeavyMech(WeaponConfig.MISSILE)
  };

  // Start is called before the first frame update
  protected void Start() {
    spawnPool = GetComponent<SpawnPool>();
    playerGameObject = GameObject.Find("Player").gameObject;
    uiManager = GameObject.FindObjectOfType<UIManagerScript>();
    Player.Instance.StartRound(
      new List<EquipmentBase>{
        new BulletWeaponInstance(WeaponConfig.MACHINE_GUN, 3f),
        new BulletWeaponInstance(WeaponConfig.TWO_SHOT_SHOTGUN, 3f),
        new ReactorInstance(ReactorConfig.DEFAULT, 3),
        new ReactorInstance(ReactorConfig.DEFAULT, 3),
        new ShieldInstance(ShieldConfig.BALANCED, 3),
        RepairSystemConfig.ROBOT_REPAIR.Instantiate(2),
      }.ToReadOnlyCollection(),
      new List<EquipmentBase>{
        new BeamInstance(WeaponConfig.FLAMER, 1f)
      }.ToReadOnlyCollection(),
      Player.INITIAL_HEALTH);

    roundStartTime = Time.timeSinceLevelLoad;
    SwitchToBattle();
  }

  private void SwitchMode(Mode newMode) {
    if (mode == newMode) {
      return;
    }
    var previousMode = mode;
    mode = newMode;
    uiManager.SwitchMode(previousMode, mode);
  }

  public void SwitchToBattle() {
    SwitchMode(Mode.Battle);
  }

  public void SwitchToInventory() {
    SwitchMode(Mode.Inventory);
  }

  private void HitPlayer(float damage) {
    if (Player.Instance.CurrentShieldStrength > 0f) {
      Player.Instance.LastShieldHitTime = TimeSinceRoundStarted();
    }
    var shieldAfterAbsorption = Player.Instance.CurrentShieldStrength - damage;
    Player.Instance.CurrentShieldStrength = Mathf.Max(0f, shieldAfterAbsorption);
    var damageAfterShields = -Mathf.Min(shieldAfterAbsorption, 0f);
    Player.Instance.CurrentHealth -= damageAfterShields;
    DamageEquipment(damageAfterShields);
  }

  private void DamageEquipment(float damageAfterShields) {
    if (Player.Instance.EquippedItems.Count == 0) {
      return;
    }
    var equipmentToDamage = Player.Instance.EquippedItems.ChooseRandomValue();
    equipmentToDamage.Health -= damageAfterShields;
    if (equipmentToDamage.Health < 0) {
      var newEquippedItems = Player.Instance.EquippedItems
        .Where(item => item != equipmentToDamage)
        .ToReadOnlyCollection();
      Player.Instance.ChangeEquipment(newEquippedItems, Player.Instance.AvailableItems);
    }
  }

  private void ClearReleaseLists() {
    bulletsToRelease.Clear();
    beamsToRelease.Clear();
  }

  private void ReleaseBullet(BulletScript bullet) {
    spawnPool.ReturnBullet(bullet);
  }

  private void ReleaseEnemy(EnemyUnitScript enemy) {
    spawnPool.ReturnUnit(enemy);
  }

  private void ReleaseBeam(BeamScript beam) {
    spawnPool.ReturnBeam(beam);
    beam.Weapon.IsCurrentlyFiring = false;
  }

  internal void BulletHitEnemy(BulletScript shotScript, GameObject enemy) {
    bulletsToRelease.Add(shotScript);
    enemy.GetComponent<EnemyUnitScript>().Health -= shotScript.Weapon.DamagePerBullet;
  }

  internal void BulletHitPlayer(BulletScript shotScript, GameObject player) {
    bulletsToRelease.Add(shotScript);
    HitPlayer(shotScript.Weapon.DamagePerBullet);
  }

  internal void BeamHitPlayer(BeamScript beamScript, GameObject player) {
    HitPlayer(beamScript.Weapon.DamagePerSecond * Time.deltaTime);
  }

  internal void BeamHitEnemy(BeamScript beamScript, GameObject enemy) {
    enemy.GetComponent<EnemyUnitScript>().Health -= beamScript.Weapon.DamagePerSecond * Time.deltaTime;
  }

  private float TimeSinceRoundStarted() {
    return Time.timeSinceLevelLoad - roundStartTime;
  }

  private float LevelBasedOnTime() {
    // level starts from 1 and increases by 1 every 10 seconds.
    return 1 + TimeSinceRoundStarted() / 10f;
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
    var verticalSize = mainCamera.orthographicSize;
    var horizontalSize = verticalSize * Screen.width / Screen.height;
    var distance = Mathf.Sqrt(Mathf.Pow(verticalSize, 2) + Mathf.Pow(horizontalSize, 2)) + 0.1f;
    newEnemy.transform.position = UnityEngine.Random.insideUnitCircle.normalized * distance;
    newEnemy.Init(config, LevelBasedOnTime());
    enemies[newEnemy.Identifier] = newEnemy;
    timeToNextSpawn = TIME_BETWEEN_SPAWNS;
  }

  private void MoveEnemies() {
    var playerPosition = playerGameObject.transform.position;
    foreach (var enemy in enemies.Values) {
      enemy.transform.RotateTowards(playerPosition, enemy.Speed * Time.deltaTime);
      enemy.gameObject.MoveForwards(1.5f);
    }
  }

  private EnemyUnitScript FindEnemyInRange(float weaponRange) {
    return enemies.Values.FirstOrDefault(enemy => Vector3.Distance(enemy.transform.position, playerGameObject.transform.position) < weaponRange);
  }

  private void AdjustBeamPosition(BeamScript beam, float rotationSpeed) {
    // If position isn't moved far from target before rotation, the rotation will go nuts.
    var beamTransform = beam.transform;
    beamTransform.position = beam.Shooter.transform.position;
    beamTransform.RotateTowards(beam.Target.transform.position, rotationSpeed);
    beamTransform.Translate(Vector3.right * beam.Weapon.Range / 2, Space.Self);
  }

  private void MoveShots() {
    foreach (var shot in bullets.Values) {
      // TODO - consider using RigidBody's movement function, instead of using kinematic rigidbodies.
      shot.gameObject.MoveForwards(shot.Speed);
      if (!shot.InRange()) {
        bulletsToRelease.Add(shot);
      }
    }

    foreach (var beam in beams.Values) {
      beam.Weapon.CurrentCharge -= Time.deltaTime * beam.Weapon.chargeConsumptionPerSecond;
      if (beam.Weapon.CurrentCharge <= 0f
        || !beam.Shooter.activeSelf
        || !beam.Target.activeSelf // TODO - this condition means that if an enemy is restored in the next frame after being destroyed, the beam will continue shooting.
        || beam.Weapon.Range < beam.Shooter.Distance(beam.Target)) {
        beamsToRelease.Add(beam);
      } else if (beam.Target.activeSelf) {
        AdjustBeamPosition(beam, 360);
      }
    }
  }

  private const float BEAM_SCALE = 0.1f;

  private void CreateBullet(GameObject shooter, BulletWeaponInstance weapon, Vector3 to) {
    var bullet = spawnPool.GetBullet(weapon.Config.shotImageName);
    bullets[bullet.Identifier] = bullet;
    var shooterPosition = shooter.transform.position;
    bullet.transform.position = shooterPosition;
    bullet.Init(shooter, shooterPosition, weapon);
    bullet.transform.RotateTowards(to, 360, (float)Randomiser.NextDouble(-weapon.Config.ShotSpreadInDegrees, weapon.Config.ShotSpreadInDegrees));
  }

  private void ShootBulletsSalvo(GameObject shooter, BulletWeaponInstance weapon, Vector3 to) {
    for (int i = 0; i < weapon.Config.numberOfBulletsPerSalvo; ++i) {
      CreateBullet(shooter, weapon, to);
    }
  }

  private IEnumerator ShootBulletsSalvos(GameObject shooter, BulletWeaponInstance weapon, Vector3 to) {
    weapon.CurrentCharge = 0;
    int salvoCount = weapon.Config.numberOfSalvosPerShot;
    while (true) {
      ShootBulletsSalvo(shooter, weapon, to);
      if (--salvoCount > 0 && mode == Mode.Battle) {
        yield return new WaitForSeconds(weapon.Config.TimeBetweenSalvosInSeconds);
      } else {
        yield break;
      }
    }
  }

  private void CreateBeam(GameObject shooter, BeamInstance weapon, GameObject target) {
    var beam = spawnPool.GetBeam(weapon.Config.shotImageName);
    beams[beam.Identifier] = beam;
    beam.transform.position = shooter.transform.position;
    beam.Init(shooter, weapon, target);
    beam.transform.localScale = new Vector3(weapon.Range * BEAM_SCALE, BEAM_SCALE, BEAM_SCALE);
    AdjustBeamPosition(beam, 360);
    weapon.IsCurrentlyFiring = true;
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
  }

  private void TryShootWeapon(WeaponBase weapon) {
    if (weapon is null || !weapon.CanShoot()) {
      return;
    }
    var enemyInRange = FindEnemyInRange(weapon.Range);
    if (enemyInRange == null) {
      return;
    }

    ShootWeapon(playerGameObject, weapon, enemyInRange.gameObject);
  }

  private void ShootEnemies() {
    Player.Instance.Weapons.ForEach(TryShootWeapon);
  }

  private void ShootPlayer() {
    if (Input.anyKey) {
      return;
    }
    foreach (var enemy in enemies.Values) {
      var weapon = enemy.Weapon;
      weapon.CurrentCharge = Mathf.Min(weapon.MaxCharge, weapon.CurrentCharge + Time.deltaTime);
      if (!weapon.CanShoot() || Vector3.Distance(enemy.transform.position, playerGameObject.transform.position) > weapon.Range) {
        continue;
      }
      ShootWeapon(enemy.gameObject, weapon, playerGameObject);
    }
  }

  private float RechargeSystems() {
    var player = Player.Instance;
    var availableEnergy = player.CurrentEnergyLevel + player.EnergyRecoveryPerSecond * Time.deltaTime;
    var chargeRequirementPerSecond = 0f;

    chargeRequirementPerSecond += player.CurrentEnergyLevel < player.MaxEnergyLevel ? player.Shields.Sum(shield => shield.EnergyConsumptionWhenRechargingPerSecond) : 0;
    chargeRequirementPerSecond += player.Weapons.Sum(weapon => weapon.CurrentChargingRequirementPerSecond);
    var currentFrameRequirement = chargeRequirementPerSecond * Time.deltaTime;

    var ratio = Mathf.Min(availableEnergy / currentFrameRequirement, 1);
    player.Weapons.ForEach(weapon => weapon.Charge(ratio));
    player.CurrentEnergyLevel = Math.Min(player.MaxEnergyLevel, Mathf.Max(0, availableEnergy - currentFrameRequirement));
    var timeSinceLastHit = TimeSinceRoundStarted() - player.LastShieldHitTime;
    var shieldRegeneration = player.Shields.Where(shield => shield.TimeBeforeRecharge < timeSinceLastHit).Sum(shield => shield.RechargeRatePerSecond) * Time.deltaTime;
    player.CurrentShieldStrength = Mathf.Min(player.MaxShieldStrength, player.CurrentShieldStrength + shieldRegeneration);
    return ratio;
  }

  private static readonly List<EquipmentConfigBase> dropList = typeof(EquipmentConfigBase)
    .Assembly.GetTypes()
    .Where(type => type.IsSubclassOf(typeof(EquipmentConfigBase)))
    .SelectMany(type => type.GetFields()
                            .Where(field => field.FieldType.IsSubclassOf(typeof(EquipmentConfigBase))
                                            && field.CustomAttributes.All(attribute => attribute.AttributeType != typeof(NoDropAttribute)))
                            .Select(field => field.GetValue(null) as EquipmentConfigBase))
    .ToList();

  private Camera mainCamera;

  private void RollForDrop(EnemyConfig config, float level) {
    if (Randomiser.ProbabilityCheck(config.DropChance)) {
      var chosenConfig = dropList.ChooseRandomValue();
      var newItem = chosenConfig.Instantiate(level);
      Player.Instance.ChangeEquipment(
        Player.Instance.EquippedItems,
        Player.Instance.AvailableItems.Concat(new[]{newItem}).ToReadOnlyCollection()
        );
    }
    Player.Instance.Scrap += (int)Mathf.Floor(level);
  }

  private void ReleaseEntities() {
    bulletsToRelease.ForEach(bullet => {
      bullets.Remove(bullet.Identifier);
      ReleaseBullet(bullet);
      var bulletTransform = bullet.transform;
      spawnPool.SpawnShotExplosion(bulletTransform.position, bulletTransform.rotation);
    });

    var enemiesToRelease = enemies.Where(pair => pair.Value.Health <= 0).ToList();
    foreach (var (key, enemy) in enemiesToRelease) {
      var enemyTransform = enemy.transform;
      spawnPool.SpawnUnitExplosion(enemyTransform.position, enemyTransform.rotation);
      enemies.Remove(key);
      ReleaseEnemy(enemy);
      RollForDrop(enemy.Config, enemy.Level);
    }

    beamsToRelease.ForEach(beam => {
      beams.Remove(beam.Identifier);
      ReleaseBeam(beam);
    });
    ClearReleaseLists();
  }

  private void RunRepairSystem(float efficiencyRatio) {
    var damagedItems = Player.Instance.EquippedItems.Where(item => item.IsDamaged).ToList();
    var repairRate = efficiencyRatio * Time.deltaTime;
    foreach (var repairSystem in Player.Instance.RepairSystems) {
      Player.Instance.CurrentHealth = Mathf.Min(Player.Instance.FullHealth, Player.Instance.CurrentHealth + repairSystem.HullRepairPerSecond * repairRate);
      for (int i = 0; i < Math.Min(damagedItems.Count, repairSystem.NumberOfSystemsToRepairSimultaneously); ++i) {
        damagedItems[i].Health = Mathf.Min(damagedItems[i].Health + repairSystem.SystemRepairPerSecond * repairRate, damagedItems[i].MaxHealth);
      }
    }
  }

  // Update is called once per frame
  protected void Update() {
    ReleaseEntities();

    if (Player.Instance.CurrentHealth <= 0) {
      SceneManager.LoadScene("LevelStartScene");
    }

    SpawnEnemyIfNeeded();
    MoveEnemies();
    ShootEnemies();
    ShootPlayer();
    MoveShots();
    var efficiencyRatio = RechargeSystems();
    RunRepairSystem(efficiencyRatio);
    uiManager.UpdateFrame();
    Player.Instance.EndFrame();
  }
}
