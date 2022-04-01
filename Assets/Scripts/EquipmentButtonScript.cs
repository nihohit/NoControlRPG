using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum EquipmentButtonBehavior { MustMaintainType, MustBeEquippedAndMaintainType, CanBeUnequipped }

public class EquipmentButtonScript : MonoBehaviour {
  private UIManagerScript manager;
  private Button button;
  private Image spriteRenderer;

  public EquipmentBase Equipment { get; private set; }
  public EquipmentButtonBehavior Behavior;
  public EquipmentType RequiredEquipmentType;

  public bool IsValidEquipment(EquipmentBase equipment) {
    return IsNotNullOrButtonCanContainNull(equipment) &&
      // ?? RequiredEquipmentType is used so that null equipment will pass the check.
      (!MustMaintainType() || (equipment?.Type ?? RequiredEquipmentType) == RequiredEquipmentType);
  }

  private bool IsNotNullOrButtonCanContainNull(EquipmentBase equipment) {
    return Behavior != EquipmentButtonBehavior.MustBeEquippedAndMaintainType || equipment != null;
  }

  private bool MustMaintainType() {
    return Behavior == EquipmentButtonBehavior.MustMaintainType ||
        Behavior == EquipmentButtonBehavior.MustBeEquippedAndMaintainType;
  }

  public void LoadEquipment(EquipmentBase equipment, TextureHandler textureHandler) {
    if (!IsNotNullOrButtonCanContainNull(equipment)) {
      throw new ArgumentException("Can't unequip from this button");
    }
    if (!IsValidEquipment(equipment)) {
      throw new ArgumentException($"{equipment.Type} doesn't match expected type {RequiredEquipmentType}");
    }

    Equipment = equipment;
    textureHandler.UpdateTexture(equipment?.Config?.equipmentImageName ?? "Empty", spriteRenderer, "Images/InventoryItems");
  }

  // Start is called before the first frame update
  void Awake() {
    manager = GameObject.FindObjectOfType<UIManagerScript>();
    button = GetComponent<Button>();
    button.onClick.AddListener(OnClick);
    spriteRenderer = GetComponent<Image>();
  }

  private void OnClick() {
    manager.InventoryButtonSelected(this);
  }
}
