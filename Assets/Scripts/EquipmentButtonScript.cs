using System;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  private UIManagerScript manager;
  private Button button;
  private Image equipmentImage;
  private Image backgroundImage;

  public EquipmentBase Equipment { get; private set; }
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
    Equipment = equipment;
    if (equipment is null) {
      equipmentImage.gameObject.SetActive(false);
      backgroundImage.color = Color.white;
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
    backgroundImage = transform.Find("ItemBackground").GetComponent<Image>();
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
