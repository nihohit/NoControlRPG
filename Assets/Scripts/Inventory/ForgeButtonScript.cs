using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ForgeAction { Repair, Upgrade }

public class ForgeButtonScript : MonoBehaviour {
  private Image equipmentImage;
  private Image backgroundImage;
  public EquipmentBase Equipment { get; private set; }
  public ForgeAction Action { get; private set; }
  public float TimeToComplete { get; private set; }

  private Color ColorForType(ForgeAction type) {
    return type switch {
      ForgeAction.Repair => Color.grey,
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
    TimeToComplete = action == ForgeAction.Upgrade ? (equipment.Level + 1) * 3 : equipment.Level * 2; // TODO - repair should take into account amount of damage;
  }

  public void RemoveEquipment() {
    Equipment = null;
    equipmentImage.gameObject.SetActive(false);
    backgroundImage.color = Color.white;
    backgroundImage.fillAmount = 1;
  }

  // Start is called before the first frame update
  protected void Awake() {
    equipmentImage = this.FindInChild<Image>("EquipmentImage");
    backgroundImage = this.GetComponent<Image>();
  }

  public float Progress {
    get {
      return backgroundImage.fillAmount;
    }
  }

  public void AdvanceProgress(float timePassed) {
    backgroundImage.fillAmount += timePassed / TimeToComplete;
  }
}
