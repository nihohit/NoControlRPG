using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;

public enum ForgeAction { Repair, Upgrade }

public class ForgeButtonScript : MonoBehaviour {
  private Image equipmentImage;
  private Image backgroundImage;
  private Image damageOverlay;
  public EquipmentBase Equipment { get; private set; }
  public ForgeAction Action { get; private set; }
  private float TimeToComplete { get; set; }

  private Color ColorForType(ForgeAction type) {
    return type switch {
      ForgeAction.Upgrade => Color.green,
      _ => Color.white,
    };
  }

  public void StartProcess(EquipmentBase equipment, TextureHandler textureHandler, ForgeAction action) {
    Assert.NotNull(equipment, nameof(equipment));
    Assert.NotNull(textureHandler, nameof(textureHandler));
    Equipment = equipment;
    equipmentImage.gameObject.SetActive(true);
    textureHandler.UpdateTexture(equipment.Config.EquipmentImageName, equipmentImage, "Images/InventoryItems");
    backgroundImage.color = ColorForType(action);
    Action = action;
    backgroundImage.fillAmount = 0; // TODO - fill be partial when fixing partially damaged parts.
    damageOverlay.fillAmount = equipment.DamageRatio;
    TimeToComplete = action == ForgeAction.Upgrade ? (equipment.Level + 1) * 3 : equipment.DamageRatio * equipment.Level * 2; // TODO - repair should take into account amount of damage;
  }

  public void RemoveEquipment() {
    if (Action == ForgeAction.Repair) {
      Equipment.Health = Equipment.MaxHealth;
    }
    Equipment = null;
    equipmentImage.gameObject.SetActive(false);
    backgroundImage.color = Color.white;
    backgroundImage.fillAmount = 1;
  }

  // Start is called before the first frame update
  protected void Awake() {
    equipmentImage = this.FindInChild<Image>("EquipmentImage");
    backgroundImage = this.GetComponent<Image>();
    damageOverlay = this.FindInChild<Image>("DamageOverlay");
    damageOverlay.fillAmount = 0;
  }

  public float Progress => Action == ForgeAction.Repair ? 1f - damageOverlay.fillAmount : backgroundImage.fillAmount;

  public void AdvanceProgress(float timePassed) {
    if (Action == ForgeAction.Repair) {
      damageOverlay.fillAmount -= timePassed / TimeToComplete;
    } else {
      backgroundImage.fillAmount += timePassed / TimeToComplete;
    }
  }
}
