using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public enum EquipmentButtonBehavior { MustMaintainType, MustBeEquippedAndMaintainType, CanBeUnequipped }

public class EquipmentButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  private UIManagerScript manager;
  private Button button;
  private Image equipmentImage;
  private Image backgroundImage;

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

  private Color ColorForType(EquipmentType type) {
    return type switch {
      EquipmentType.Weapon => Color.red,
      EquipmentType.Shield => Color.blue,
      EquipmentType.TargetingSystem => Color.grey,
      EquipmentType.Reactor => Color.yellow,
      _ => Color.white,
    };
  }

  public void LoadEquipment(EquipmentBase equipment, TextureHandler textureHandler) {
    if (!IsNotNullOrButtonCanContainNull(equipment)) {
      throw new ArgumentException("Can't unequip from this button");
    }
    if (!IsValidEquipment(equipment)) {
      throw new ArgumentException($"{equipment.Type} doesn't match expected type {RequiredEquipmentType}");
    }

    Equipment = equipment;
    if (equipment is null) {
      equipmentImage.gameObject.SetActive(false);
      backgroundImage.color = MustMaintainType() ? ColorForType(RequiredEquipmentType) : Color.white;
    } else {
      equipmentImage.gameObject.SetActive(true);
      textureHandler.UpdateTexture(equipment.Config.EquipmentImageName, equipmentImage, "Images/InventoryItems");
      backgroundImage.color = ColorForType(equipment.Type);
    }
  }



  // Start is called before the first frame update
  protected void Awake() {
    manager = GameObject.FindObjectOfType<UIManagerScript>();
    button = GetComponent<Button>();
    button.onClick.AddListener(OnClick);
    equipmentImage = transform.Find("EquipmentImage").GetComponent<Image>();
    backgroundImage = GetComponent<Image>();
  }

  private void OnClick() {
    manager.InventoryButtonSelected(this);
  }

  public void OnPointerEnter(PointerEventData eventData) {
    manager.PointerEnterButton(this);
  }

  public void OnPointerExit(PointerEventData eventData) {
    manager.PointerExitButton(this);
  }
}
