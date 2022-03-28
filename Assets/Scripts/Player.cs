using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Base;

public class Player {
  public ReadOnlyCollection<WeaponBase> Weapons { private set; get; }
  public ReadOnlyCollection<WeaponBase> AvailableItems { private set; get; }

  public static readonly float INITIAL_HEALTH = 100f;
  public float FullHealth { get; private set; }
  public float CurrentHealth { get; set; }

  public void StartRound(ReadOnlyCollection<WeaponBase> weapons, ReadOnlyCollection<WeaponBase> availableItems, float newHealth) {
    Weapons = weapons;
    AvailableItems = availableItems;
    CurrentHealth = newHealth;
    FullHealth = newHealth;
  }

  public static readonly Player Instance = new() {
    Weapons = new List<WeaponBase>{
      new BulletWeaponInstance(WeaponConfig.MACHINE_GUN, 1f),
      new BulletWeaponInstance(WeaponConfig.TWO_SHOT_SHOTGUN, 1f)
    }.ToReadOnlyCollection(),
    AvailableItems = new List<WeaponBase>{
      new BulletWeaponInstance(WeaponConfig.MISSILE, 1f),
      new BeamInstance(WeaponConfig.FLAMER, 1f),
      new BeamInstance(WeaponConfig.LASER, 1f),
      new BulletWeaponInstance(WeaponConfig.RIFLE, 1f)
    }.ToReadOnlyCollection()
  };
}