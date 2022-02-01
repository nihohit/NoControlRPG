using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPool : MonoBehaviour {
  private readonly List<EnemyUnitScript> units = new List<EnemyUnitScript>();
  private GameObject unitResource;

  private void returnToPool<TType>(TType item, List<TType> pool) where TType : MonoBehaviour {
    pool.Add(item);
    item.gameObject.SetActive(false);
  }

  private TType getFromPool<TType>(List<TType> pool, GameObject resource) where TType : MonoBehaviour {
    if (pool.Count > 0) {
      var result = pool[pool.Count - 1];
      pool.RemoveAt(pool.Count - 1);
      result.gameObject.SetActive(true);
      return result;
    }
    return Instantiate(resource).GetComponent<TType>();
  }

  public EnemyUnitScript GetUnit() {
    return getFromPool(units, unitResource);
  }

  public void ReturnUnit(EnemyUnitScript unit) {
    returnToPool(unit, units);
  }

  public void Start() {
    unitResource = Resources.Load<GameObject>("prefabs/EnemyUnit");
  }
}
