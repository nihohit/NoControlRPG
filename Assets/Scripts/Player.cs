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

  public int Scrap { get; set; }

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
  }

  public static readonly Player Instance = new();
}