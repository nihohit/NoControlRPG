using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Base;
using UnityEngine;

public class SpawnPool : MonoBehaviour {
  private readonly List<EnemyUnitScript> units = new List<EnemyUnitScript>();
  private GameObject unitResource;
  private readonly Dictionary<string, GameObject> shotResources = new Dictionary<string, GameObject>();
  private readonly Dictionary<string, List<ShotScript>> shots = new Dictionary<string, List<ShotScript>>();

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
    }
    return Instantiate(resource).GetComponent<TType>();
  }

  public EnemyUnitScript GetUnit() {
    return GetFromPool(units, unitResource);
  }

  public void ReturnUnit(EnemyUnitScript unit) {
    ReturnToPool(unit, units);
  }

  private static List<ShotScript> CreateShotList() {
    return new List<ShotScript>();
  }

  private List<ShotScript> GetShotList(string shotName) {
    return shots.TryGetOrAdd(shotName, CreateShotList);
  }

  public ShotScript GetShot(string shotName) {
    var resource = shotResources.TryGetOrAdd(shotName, () => Resources.Load<GameObject>("prefabs/Shot"));
    return GetFromPool(GetShotList(shotName), resource);
  }

  public void ReturnShot(ShotScript shot) {
    ReturnToPool(shot, GetShotList(shot.Config.shotImageName));
  }

  public void Start() {
    unitResource = Resources.Load<GameObject>("prefabs/EnemyUnit");
  }
}
