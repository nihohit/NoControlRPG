using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Base;
using UnityEngine;

public class SpawnPool : MonoBehaviour {
  private readonly List<EnemyUnitScript> units = new List<EnemyUnitScript>();
  private GameObject unitResource;
  private GameObject bulletResource;
  private readonly Dictionary<string, List<BulletScript>> bullets = new Dictionary<string, List<BulletScript>>();
  private GameObject beamResource;
  private readonly Dictionary<string, List<BeamScript>> beams = new Dictionary<string, List<BeamScript>>();

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

  private static List<T> CreateShotList<T>() {
    return new List<T>();
  }

  private List<T> GetShotList<T>(string shotName, Dictionary<string, List<T>> shotDictionary) {
    return shotDictionary.TryGetOrAdd(shotName, CreateShotList<T>);
  }

  public BulletScript GetBullet(string bulletName) {
    if (bulletResource == null) {
      bulletResource = Resources.Load<GameObject>("prefabs/Shot");
    }
    return GetFromPool(GetShotList(bulletName, bullets), bulletResource);
  }

  public void ReturnBullet(BulletScript shot) {
    ReturnToPool(shot, GetShotList(shot.Config.shotImageName, bullets));
  }

  public BeamScript GetBeam(string beamName) {
    if (beamResource == null) {
      beamResource = Resources.Load<GameObject>("prefabs/Beam");
    }
    return GetFromPool(GetShotList(beamName, beams), beamResource);
  }


  public void ReturnBeam(BeamScript beam) {
    ReturnToPool(beam, GetShotList(beam.Config.shotImageName, beams));
  }

  public void Start() {
    unitResource = Resources.Load<GameObject>("prefabs/EnemyUnit");
  }
}
