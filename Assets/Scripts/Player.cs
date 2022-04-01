using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Base;

public class Player {
  public WeaponBase Weapon1 { get; private set; }
  public WeaponBase Weapon2 { get; private set; }
  public ReactorInstance Reactor { get; private set; }
  public ShieldInstance Shield { get; private set; }
  public TargetingSystemInstance TargetingSystem { get; private set; }
  public ReadOnlyCollection<EquipmentBase> AvailableItems { private set; get; }

  public static readonly float INITIAL_HEALTH = 100f;
  public float FullHealth { get; private set; }
  public float CurrentHealth { get; set; }

  public void StartRound(WeaponBase weapon1, WeaponBase weapon2, ReactorInstance reactor, ShieldInstance shield, TargetingSystemInstance targetingSystem, ReadOnlyCollection<EquipmentBase> availableItems, float newHealth) {
    Weapon1 = weapon1;
    Weapon2 = weapon2;
    Reactor = reactor;

    Shield = shield;
    Shield.CurrentStrength = Shield.MaxStrength;
    Shield.TimeBeforeNextRecharge = 0;

    TargetingSystem = targetingSystem;
    AvailableItems = availableItems;
    CurrentHealth = newHealth;
    FullHealth = newHealth;
  }

  public static readonly Player Instance = new() {
    Weapon1 = new BulletWeaponInstance(WeaponConfig.MACHINE_GUN, 1f),
    Weapon2 = new BulletWeaponInstance(WeaponConfig.TWO_SHOT_SHOTGUN, 1f),
    Reactor = new ReactorInstance(ReactorConfig.DEFAULT, 1),
    Shield = new ShieldInstance(ShieldConfig.DEFAULT, 1),
    TargetingSystem = new TargetingSystemInstance(TargetingSystemConfig.DEFAULT, 1),
    AvailableItems = new List<EquipmentBase>{
      new BulletWeaponInstance(WeaponConfig.MISSILE, 1f),
      new BeamInstance(WeaponConfig.FLAMER, 1f),
      new BeamInstance(WeaponConfig.LASER, 1f),
      new BulletWeaponInstance(WeaponConfig.RIFLE, 1f)
    }.ToReadOnlyCollection()
  };
}