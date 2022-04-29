using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class Player {
  public List<EquipmentBase> AvailableItems { private set; get; }
  public ReadOnlyCollection<EquipmentBase> EquippedItems { private set; get; }

  public static readonly float INITIAL_HEALTH = 100f;
  public float FullHealth { get; private set; }
  public float CurrentHealth { get; set; }
  public float MaxEnergyLevel { get; private set; }
  public float CurrentEnergyLevel { get; set; }
  public float EnergyRecoveryPerSecond { get; private set; }

  public float MaxShieldStrength { get; private set; }
  public float CurrentShieldStrength { get; set; }
  public float LastShieldHitTime { get; set; }
  public IList<ShieldInstance> Shields { get; private set; }
  public List<WeaponBase> Weapons { get; private set; }
  public ulong XP { get; set; }
  public ulong XPToNextLevel { get; private set; }
  public uint Level { get; set; }
  public uint Scrap { get; set; }

  public void StartRound(ReadOnlyCollection<EquipmentBase> equippedItems,
                         List<EquipmentBase> availableItems,
                         float newHealth) {
    EquippedItems = equippedItems;
    AvailableItems = availableItems;
    CurrentHealth = newHealth;
    FullHealth = newHealth;

    Shields = equippedItems.AllOfType<ShieldInstance>();
    MaxShieldStrength = Shields.Sum(shield => shield.MaxStrength);
    CurrentShieldStrength = MaxShieldStrength;
    LastShieldHitTime = 0;

    var reactors = equippedItems.AllOfType<ReactorInstance>();
    MaxEnergyLevel = reactors.Sum(reactor => reactor.MaxEnergyLevel);
    CurrentEnergyLevel = MaxEnergyLevel;
    EnergyRecoveryPerSecond = equippedItems.GetEnergyGeneration();

    Weapons = equippedItems.AllOfType<WeaponBase>();
    Weapons.ForEach(weapon => weapon.CurrentCharge = weapon.MaxCharge);
    XP = 0;
    Level = 1;
    XPToNextLevel = GetXPToNextLevel();
  }

  public static readonly Player Instance = new() {
    EquippedItems = new ReadOnlyCollection<EquipmentBase>(new List<EquipmentBase>{
      new BulletWeaponInstance(WeaponConfig.MACHINE_GUN, 1f),
      new BulletWeaponInstance(WeaponConfig.TWO_SHOT_SHOTGUN, 1f),
      new ReactorInstance(ReactorConfig.DEFAULT, 1),
      new ShieldInstance(ShieldConfig.BALANCED, 1)
    }),
    AvailableItems = new List<EquipmentBase>()
  };

  private uint GetXPToNextLevel() {
    return (uint)Math.Pow(2, Level) * 20;
  }

  internal bool LevelUpIfPossible() {
    if (XP < XPToNextLevel) {
      return false;
    }
    XP -= XPToNextLevel;
    Level += 1;
    return true;
  }
}