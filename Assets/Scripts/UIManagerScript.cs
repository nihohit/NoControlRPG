using Assets.Scripts.UnityBase;
using UnityEngine;

public class UIManagerScript : MonoBehaviour {
  private InventoryUIScript inventoryUIHolder;
  private BattleUIScript battleUIHolder;
  private GeneralUIScript generalUIManager;
  private readonly TextureHandler textureHandler = new();

  protected void Awake() {
    inventoryUIHolder = this.FindInChild<InventoryUIScript>("InventoryUI");
    generalUIManager = GameObject.FindObjectOfType<GeneralUIScript>();
    inventoryUIHolder.TextureHandler = textureHandler;
    inventoryUIHolder.gameObject.SetActive(false);
    battleUIHolder = this.FindInChild<BattleUIScript>("BattleUI");
    battleUIHolder.TextureHandler = textureHandler;
    battleUIHolder.gameObject.SetActive(false);
  }

  private void CloseMode(Mode mode) {
    switch (mode) {
      case Mode.Start:
        break;
      case Mode.Battle:
        battleUIHolder.gameObject.SetActive(false);
        break;
      case Mode.Inventory:
        inventoryUIHolder.gameObject.SetActive(false);
        break;
      case Mode.Forge:
        inventoryUIHolder.gameObject.SetActive(false);
        break;
    }
  }

  private void OpenMode(Mode mode) {
    switch (mode) {
      case Mode.Start:
        break;
      case Mode.Battle:
        battleUIHolder.gameObject.SetActive(true);
        battleUIHolder.SetupWeaponBars();
        break;
      case Mode.Inventory:
        inventoryUIHolder.gameObject.SetActive(true);
        inventoryUIHolder.OpenInventory();
        break;
      case Mode.Forge:
        inventoryUIHolder.gameObject.SetActive(true);
        inventoryUIHolder.OpenForge();
        break;
    }
  }

  public void SwitchMode(Mode previous, Mode next) {
    CloseMode(previous);
    OpenMode(next);
  }

  public void UpdateUIOverlay() {
    if (battleUIHolder.gameObject.activeSelf) {
      battleUIHolder.UpdateUIOverlay();
    }
    generalUIManager.UpdateUIOverlay();
  }

  public void UpdateInventoryState() {
    inventoryUIHolder.UpdateInventoryStateExternally();
  }
}
