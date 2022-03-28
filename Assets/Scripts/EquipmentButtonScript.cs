using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentButtonScript : MonoBehaviour {
  private UIManagerScript manager;
  private Button button;
  private Image spriteRenderer;

  public WeaponBase Equipment { get; private set; }

  public void LoadEquipment(WeaponBase weapon, TextureHandler textureHandler) {
    Equipment = weapon;
    textureHandler.UpdateTexture(weapon?.Config?.equipmentImageName ?? "Empty", spriteRenderer, "Images/InventoryItems");
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
