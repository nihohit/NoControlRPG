using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class SpawnPool : MonoBehaviour {
  private GameObject unitBaseResource;
  private GameObject bulletBaseResource;
  private GameObject beamBaseResource;


  private Dictionary<string, GameObject> bulletResources = new();
  private Dictionary<string, GameObject> beamResources = new();

  private readonly List<EnemyUnitScript> unitsPool = new();
  private readonly Dictionary<string, List<BulletScript>> bulletPools = new();
  private readonly Dictionary<string, List<BeamScript>> beamPools = new();
  private readonly TextureHandler textureHandler = new();

  private void ReturnToPool<TType>(TType item, List<TType> pool) where TType : MonoBehaviour {
    pool.Add(item);
    item.gameObject.SetActive(false);
  }

  private TType GetFromPool<TType>(List<TType> pool, GameObject resource) where TType : MonoBehaviour {
    if (pool.Count > 0) {
      var result = pool[pool.Count - 1];
      pool.RemoveAt(pool.Count - 1);
      result.gameObject.SetActive(true);
      return result;
    } else {
      var result = Instantiate(resource).GetComponent<TType>();
      result.gameObject.SetActive(true);
      return result;
    }
  }

  private void Awake() {
    bulletBaseResource = Resources.Load<GameObject>("prefabs/Shot");
    beamBaseResource = Resources.Load<GameObject>("prefabs/Beam");
    unitBaseResource = Resources.Load<GameObject>("prefabs/EnemyUnit");
  }

  public EnemyUnitScript GetUnit() {
    return GetFromPool(unitsPool, unitBaseResource);
  }

  public void ReturnUnit(EnemyUnitScript unit) {
    ReturnToPool(unit, unitsPool);
  }

  private static List<T> CreateList<T>() {
    return new List<T>();
  }

  private List<T> GetAvailableObjectsPool<T>(string objectName, Dictionary<string, List<T>> poolsDictionary) {
    return poolsDictionary.TryGetOrAdd(objectName, CreateList<T>);
  }

  private GameObject GetObjectResource(string objectName, Dictionary<string, GameObject> resourcesDictionary, GameObject baseResource) {
    return resourcesDictionary.TryGetOrAdd(objectName, () => {
      var resource = Instantiate(baseResource);
      var spriteRenderer = resource.GetComponent<SpriteRenderer>();
      textureHandler.UpdateTexture(objectName, spriteRenderer, "Images/VisualEffects");
      gameObject.name = objectName;
      resource.SetActive(false);
      return resource;
    });
  }

  private T GetObject<T>(string objectName, Dictionary<string, GameObject> resourcesDictionary, GameObject baseResource, Dictionary<string, List<T>> availableObjectsDictionary) where T : MonoBehaviour {
    var availableObjects = GetAvailableObjectsPool(objectName, availableObjectsDictionary);
    var resource = GetObjectResource(objectName, resourcesDictionary, baseResource);
    return GetFromPool<T>(availableObjects, resource);
  }

  public BulletScript GetBullet(string bulletName) {
    return GetObject(bulletName, bulletResources, bulletBaseResource, bulletPools);
  }

  public void ReturnBullet(BulletScript shot) {
    ReturnToPool(shot, GetAvailableObjectsPool(shot.Config.shotImageName, bulletPools));
  }

  public BeamScript GetBeam(string beamName) {
    return GetObject(beamName, beamResources, beamBaseResource, beamPools);
  }


  public void ReturnBeam(BeamScript beam) {
    ReturnToPool(beam, GetAvailableObjectsPool(beam.Config.shotImageName, beamPools));
  }
}
