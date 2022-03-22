using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Base;

public class Player {
  public ReadOnlyCollection<WeaponInstance> Weapons { private set; get; }
  public ReadOnlyCollection<WeaponInstance> AvailableItems { private set; get; }

  public static readonly float INITIAL_HEALTH = 100f;
  public float FullHealth { get; private set; }
  public float CurrentHealth { get; set; }

  public void StartRound(ReadOnlyCollection<WeaponInstance> weapons, ReadOnlyCollection<WeaponInstance> availableItems, float newHealth) {
    Weapons = weapons;
    AvailableItems = availableItems;
    CurrentHealth = newHealth;
    FullHealth = newHealth;
  }

  public static readonly Player Instance = new() {
    Weapons = new List<WeaponInstance>{
      new WeaponInstance(WeaponConfig.MACHINE_GUN),
      new WeaponInstance(WeaponConfig.TWO_SHOT_SHOTGUN)
    }.ToReadOnlyCollection(),
    AvailableItems = new List<WeaponInstance>{
      new WeaponInstance(WeaponConfig.MISSILE),
      new WeaponInstance(WeaponConfig.FLAMER),
      new WeaponInstance(WeaponConfig.LASER),
      new WeaponInstance(WeaponConfig.RIFLE)
    }.ToReadOnlyCollection()
  };
}