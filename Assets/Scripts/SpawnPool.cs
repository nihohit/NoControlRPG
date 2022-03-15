using System.Collections.Generic;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class SpawnPool : MonoBehaviour {
  private GameObject unitBaseResource;
  private GameObject bulletBaseResource;
  private GameObject beamBaseResource;

  private readonly Dictionary<string, GameObject> bulletResources = new();
  private readonly Dictionary<string, GameObject> beamResources = new();
  private readonly Dictionary<string, GameObject> unitResources = new();
  private readonly Dictionary<string, GameObject> explosionResources = new();

  private readonly Dictionary<string, List<EnemyUnitScript>> unitPools = new();
  private readonly Dictionary<string, List<BulletScript>> bulletPools = new();
  private readonly Dictionary<string, List<BeamScript>> beamPools = new();
  private readonly Dictionary<string, List<ExplosionScript>> explosionPools = new();
  private readonly TextureHandler textureHandler = new();

  private void ReturnToPool<TType>(TType item, List<TType> pool) where TType : MonoBehaviour {
    pool.Add(item);
    item.gameObject.SetActive(false);
  }

  private TType GetFromPool<TType>(List<TType> pool, GameObject resource) where TType : MonoBehaviour {
    if (pool.Count > 0) {
      var result = pool[^1];
      pool.RemoveAt(pool.Count - 1);
      result.gameObject.SetActive(true);
      return result;
    } else {
      var result = Instantiate(resource).GetComponent<TType>();
      result.gameObject.name = resource.gameObject.name;
      result.gameObject.SetActive(true);
      return result;
    }
  }

  private void Awake() {
    bulletBaseResource = Resources.Load<GameObject>("prefabs/Shot");
    beamBaseResource = Resources.Load<GameObject>("prefabs/Beam");
    unitBaseResource = Resources.Load<GameObject>("prefabs/EnemyUnit");
  }

  private static List<T> CreateList<T>() {
    return new List<T>();
  }

  private List<T> GetAvailableObjectsPool<T>(string objectName, Dictionary<string, List<T>> poolsDictionary) {
    return poolsDictionary.TryGetOrAdd(objectName, CreateList<T>);
  }

  private GameObject GetObjectResource(string objectName, Dictionary<string, GameObject> resourcesDictionary, GameObject baseResource, string spriteFolder) {
    return resourcesDictionary.TryGetOrAdd(objectName, () => {
      var resource = Instantiate(baseResource);
      var spriteRenderer = resource.GetComponent<SpriteRenderer>();
      textureHandler.UpdateTexture(objectName, spriteRenderer, spriteFolder);
      // If this becomes a perf problem, try using this: 
      // https://github.com/j-bbr/PolygonColliderSimplification/tree/master/Assets/Collider2D%20Optimization
      var collider = resource.gameObject.AddComponent<PolygonCollider2D>();
      collider.isTrigger = true;
      resource.gameObject.name = objectName;
      resource.SetActive(false);
      return resource;
    });
  }

  private T GetObject<T>(string objectName, Dictionary<string, GameObject> resourcesDictionary, GameObject baseResource, Dictionary<string, List<T>> availableObjectsDictionary, string spriteFolder) where T : MonoBehaviour {
    var availableObjects = GetAvailableObjectsPool(objectName, availableObjectsDictionary);
    var resource = GetObjectResource(objectName, resourcesDictionary, baseResource, spriteFolder);
    return GetFromPool(availableObjects, resource);
  }


  public BulletScript GetBullet(string bulletName) {
    return GetObject(bulletName, bulletResources, bulletBaseResource, bulletPools, "Images/VisualEffects");
  }

  public void ReturnBullet(BulletScript shot) {
    ReturnToPool(shot, GetAvailableObjectsPool(shot.Config.shotImageName, bulletPools));
  }

  public BeamScript GetBeam(string beamName) {
    return GetObject(beamName, beamResources, beamBaseResource, beamPools, "Images/VisualEffects");
  }


  public void ReturnBeam(BeamScript beam) {
    ReturnToPool(beam, GetAvailableObjectsPool(beam.Config.shotImageName, beamPools));
  }

  public EnemyUnitScript GetUnit() {
    return GetObject("ScoutMech", unitResources, unitBaseResource, unitPools, "Images/Entities");
  }

  public void ReturnUnit(EnemyUnitScript unit) {
    ReturnToPool(unit, GetAvailableObjectsPool(unit.gameObject.name, unitPools)); ;
  }

  private void SpawnExplosion(Vector3 position, string explosionName) {
    if (!explosionResources.TryGetValue(explosionName, out var resource)) {
      resource = Resources.Load<GameObject>($"effects/{explosionName}");
      explosionResources[explosionName] = resource;
    }
    var explosion = GetFromPool(GetAvailableObjectsPool(explosionName, explosionPools), resource);
    explosion.StartExplosion();
    explosion.transform.position = position;
  }

  public void SpawnUnitExplosion(Vector3 position) {
    SpawnExplosion(position, "UnitExplosion");
  }

  public void ExplosionDone(ExplosionScript explosion) {
    ReturnToPool(explosion, GetAvailableObjectsPool(explosion.gameObject.name, explosionPools));
  }
}
