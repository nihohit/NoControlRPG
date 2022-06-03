using System;
using System.Collections.Generic;
using Assets.Scripts.Base;
using UnityEngine;

public class Forge {
  public static Forge Instance { get; } = new();

  private Forge() {}
  public class Action {
    public enum Type {
      Repair,
      Upgrade
    };
    public Type ActionType { get; }
    public EquipmentBase Equipment { get; private set; }
    private readonly float totalTimeToComplete;
    private float timeToComplete = 0;
  
    private const float REPAIR_RATE = 5;
    private const float UPGRADE_RATE = 3;
    
    public Action(EquipmentBase equipment, Type type) {
      Equipment = equipment;
      ActionType = type;
      totalTimeToComplete = type == Type.Upgrade
        ? (equipment.Level + 1) * UPGRADE_RATE
        : (equipment.MaxHealth - equipment.Health) / REPAIR_RATE;
    }
  
    private EquipmentBase CleanAndReturn() {
      var equipment = Equipment;
      Equipment = null;
      timeToComplete = Single.MaxValue;
      return equipment;
    }
    
    private EquipmentBase AdvanceRepair(float time) {
      Equipment.Health = Mathf.Min(Equipment.Health + time * REPAIR_RATE, Equipment.MaxHealth);
      return Equipment.Health >= Equipment.MaxHealth ? CleanAndReturn() : null;
    }
  
    private EquipmentBase AdvanceUpgrade(float time) {
      timeToComplete += time;
      return timeToComplete >= totalTimeToComplete ? CleanAndReturn().UpgradedVersion() : null;
    }
    
    public EquipmentBase Advance(float time) {
      Assert.NotNull(Equipment, nameof(Equipment));
  
      switch (ActionType) {
        case Action.Type.Repair:
          return AdvanceRepair(time);
        case Action.Type.Upgrade:
          return AdvanceUpgrade(time);
      }
      Assert.UnreachableCode();
      return null;
    }

    public float CompletionRate =>
      ActionType == Action.Type.Upgrade
        ? timeToComplete / totalTimeToComplete
        : Equipment.DamageRatio;
  }

  private readonly Dictionary<Guid, Action> actions = new();

  private Action AddAction(EquipmentBase equipment, int cost, Action.Type forgeActionType) {
    var player = Player.Instance;
    Assert.EqualOrGreater(player.Scrap, cost);
    Assert.AssertConditionMet(!actions.ContainsKey(equipment.Identifier), "Equipment already under forge action");
    player.Scrap -= cost;
    var action = new Action(equipment, forgeActionType);
    actions.Add(equipment.Identifier, action);
    return action;
  }
  
  public Action Repair(EquipmentBase equipment) {
    return AddAction(equipment, equipment.FixCost, Action.Type.Repair);
  }

  public Action Upgrade(EquipmentBase equipment) {
    return AddAction(equipment, equipment.UpgradeCost, Action.Type.Upgrade);
  }

  public List<EquipmentBase> Advance(float time) {
    List<EquipmentBase> results = new();
    foreach (var action in actions.Values) {
      var result = action.Advance(time);
      if (result != null) {
        results.Add(result);
      }
    }

    // iterate in reverse. If we iterated forwards then indices would need to be updated when prior
    // indices were removed.
    foreach (var result in results) {
      actions.Remove(result.Identifier);
    }

    return results;
  }

  public Action GetActionForEquipment(EquipmentBase equipment) {
    if (equipment is null) {
      return null;
    }
    
    if (actions.TryGetValue(equipment.Identifier, out var forgeAction)) {
      return forgeAction;
    }

    return null;
  }
  
  public Action.Type? GetActionTypeForEquipment(EquipmentBase equipment) {
    return GetActionForEquipment(equipment)?.ActionType;
  }

  public int NumberOfActions => actions.Count;
}