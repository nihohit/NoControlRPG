using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  private InventoryUIScript manager;
  private Button button;
  private Image equipmentImage;
  private Image backgroundImage;
  private Image damageOverlay;
  private Image repairOverlay;
  private Image upgradeOverlay;

  public EquipmentBase Equipment { get; private set; }
  private Color ColorForType(EquipmentType type) {
    return type switch {
      EquipmentType.Weapon => Color.red,
      EquipmentType.Shield => Color.blue,
      EquipmentType.RepairSystem => Color.grey,
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
    damageOverlay.fillAmount = equipment?.DamageRatio ?? 0;
    upgradeOverlay.enabled = false;
    repairOverlay.enabled = false;
  }

  // Start is called before the first frame update
  protected void Awake() {
    manager = GameObject.FindObjectOfType<InventoryUIScript>();
    button = GetComponent<Button>();
    button.onClick.AddListener(OnClick);
    equipmentImage = this.FindInChild<Image>("EquipmentImage");
    backgroundImage = this.FindInChild<Image>("ItemBackground");
    damageOverlay = this.FindInChild<Image>("DamageOverlay");
    upgradeOverlay = this.FindInChild<Image>("UpgradeIcon");
    repairOverlay = this.FindInChild<Image>("RepairIcon");
  }

  public void UpdateDamageIndicator() {
    damageOverlay.fillAmount = Equipment?.DamageRatio ?? 0;
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

  public void SetForgeAction(Forge.Action.Type? actionType) {
    repairOverlay.enabled = actionType == Forge.Action.Type.Repair;
    upgradeOverlay.enabled = actionType == Forge.Action.Type.Upgrade;
  }
}
