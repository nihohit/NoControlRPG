using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class Player {
  public ReadOnlyCollection<EquipmentBase> AvailableItems { private set; get; }
  public ReadOnlyCollection<EquipmentBase> EquippedItems { private set; get; }

  public static readonly float INITIAL_HEALTH = 500f;
  
  public float FullHealth { get; private set; }
  public float CurrentHealth { get; set; }
  public float MaxEnergyLevel { get; private set; }
  public float CurrentEnergyLevel { get; set; }
  public float EnergyRecoveryPerSecond { get; private set; }
  public float MaxShieldStrength { get; private set; }
  public float CurrentShieldStrength { get; set; }
  public float LastShieldHitTime { get; set; }
  public IList<ShieldInstance> Shields { get; private set; }
  public IList<RepairSystemInstance> RepairSystems { get; private set; }
  public List<WeaponBase> Weapons { get; private set; }
  public bool AvailableItemsChangedThisFrame { get; private set; } = true;
  public bool EquippedItemsChangedThisFrame { get; private set; } = true;

  public int Scrap { get; set; }

  public void StartRound(ReadOnlyCollection<EquipmentBase> equippedItems,
    ReadOnlyCollection<EquipmentBase> availableItems,
                         float newHealth) {
    CurrentHealth = newHealth;
    FullHealth = newHealth;
    LastShieldHitTime = 0;

    ChangeEquipmentInternal(equippedItems, availableItems);
    CurrentShieldStrength = MaxShieldStrength;
    CurrentEnergyLevel = MaxEnergyLevel;
    Weapons.ForEach(weapon => weapon.CurrentCharge = weapon.MaxCharge);
  }

  public void EndFrame() {
    EquippedItemsChangedThisFrame = false;
    AvailableItemsChangedThisFrame = false;
  }
  
  private void ChangeEquipmentInternal(ReadOnlyCollection<EquipmentBase> equippedItems,
    ReadOnlyCollection<EquipmentBase> availableItems) {
    AvailableItems = availableItems;
    AvailableItemsChangedThisFrame = true;
    if (EquippedItems == equippedItems) {
      return;
    }
    EquippedItems = equippedItems;

    var activeEquippedItems = equippedItems.Where(item => !item.IsBeingForged).ToArray();
    Shields = activeEquippedItems.AllOfType<ShieldInstance>();
    MaxShieldStrength = Shields.Sum(shield => shield.MaxStrength);

    var reactors = activeEquippedItems.AllOfType<ReactorInstance>();
    MaxEnergyLevel = reactors.Sum(reactor => reactor.MaxEnergyLevel);
    EnergyRecoveryPerSecond = equippedItems.GetEnergyGeneration();

    Weapons = activeEquippedItems.AllOfType<WeaponBase>();
    RepairSystems = activeEquippedItems.AllOfType<RepairSystemInstance>();
    EquippedItemsChangedThisFrame = true;
  }

  public void ChangeEquipment(ReadOnlyCollection<EquipmentBase> equippedItems,
    ReadOnlyCollection<EquipmentBase> availableItems) {
    ChangeEquipmentInternal(equippedItems, availableItems);
    CurrentEnergyLevel = Mathf.Min(MaxEnergyLevel, CurrentEnergyLevel);
    CurrentShieldStrength = Mathf.Min(MaxShieldStrength, CurrentShieldStrength);
  }

  public static readonly Player Instance = new();
}