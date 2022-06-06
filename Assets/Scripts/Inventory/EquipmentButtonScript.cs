using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  #region views
  private InventoryUIScript manager;
  private Button button;
  private Image equipmentImage;
  private Image backgroundImage;
  private Image damageOverlay;
  private Image repairOverlay;
  private Image upgradeOverlay;
  #endregion

  #region state

  private Forge.Action forgeAction;
  public EquipmentBase Equipment { get; private set; }
  #endregion
  
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

  private void OnClick() {
    manager.InventoryButtonSelected(this);
  }

  public void OnPointerEnter(PointerEventData eventData) {
    manager.PointerEnterButton(this);
  }

  public void OnPointerExit(PointerEventData eventData) {
    manager.PointerExitButton(this);
  }
  
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
    SetForgeAction(Forge.Instance.GetActionForEquipment(equipment));
    if (equipment is null) {
      equipmentImage.gameObject.SetActive(false);
      backgroundImage.color = Color.white;
    } else {
      equipmentImage.gameObject.SetActive(true);
      textureHandler.UpdateTexture(equipment.Config.EquipmentImageName, equipmentImage, "Images/InventoryItems");
      backgroundImage.color = ColorForType(equipment.Type);
    }
    damageOverlay.fillAmount = equipment?.DamageRatio ?? 0;
  }

  public void FrameUpdate() {
    UpdateDamageIndicator();
    UpdateUpgradeProgress();
  }
  
  private void UpdateDamageIndicator() {
    damageOverlay.fillAmount = Equipment?.DamageRatio ?? 0;
  }

  public void SetForgeAction(Forge.Action action) {
    forgeAction = action;
    repairOverlay.enabled = action?.ActionType == Forge.Action.Type.Repair;
    upgradeOverlay.enabled = action?.ActionType == Forge.Action.Type.Upgrade;
    upgradeOverlay.fillAmount = 1;
  }
  
  private void UpdateUpgradeProgress() {
    if (forgeAction is not null && forgeAction.Completed) {
      Equipment = forgeAction.ActionType == Forge.Action.Type.Upgrade
        ? Equipment.UpgradedVersion()
        : Equipment;
      SetForgeAction(null);
      return;
    }
    if (Equipment == null || 
        forgeAction == null || 
        forgeAction.ActionType == Forge.Action.Type.Repair) {
      return;
    }
    upgradeOverlay.fillAmount = 1 - forgeAction.CompletionRate;
  }
}
