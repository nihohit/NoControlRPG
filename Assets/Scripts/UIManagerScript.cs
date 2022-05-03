using Assets.Scripts.UnityBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {
  private BattleMainManagerScript mainManager;
  private Button switchContextButton;
  private TMP_Text switchContextText;
  private InventoryUIScript inventoryUIHolder;
  private BattleUIScript battleUIHolder;

  private GeneralUIScript generalUIManager;
  private readonly TextureHandler textureHandler = new();

  protected void Awake() {
    switchContextButton = this.FindInChild<Button>("SwitchContext");
    switchContextText = switchContextButton.FindInChild<TMPro.TMP_Text>("Text");
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = this.FindInChild<InventoryUIScript>("InventoryUI");
    generalUIManager = GameObject.FindObjectOfType<GeneralUIScript>();
    inventoryUIHolder.TextureHandler = textureHandler;
    inventoryUIHolder.SwitchContextText = switchContextText;
    inventoryUIHolder.gameObject.SetActive(false);
    battleUIHolder = this.FindInChild<BattleUIScript>("BattleUI");
    battleUIHolder.TextureHandler = textureHandler;
    battleUIHolder.gameObject.SetActive(false);
  }

  public void SwitchContextPressed() {
    if (inventoryUIHolder.gameObject.activeSelf && !inventoryUIHolder.HasSufficientEnergy()) {
      return;
    }
    mainManager.SwitchContext();
  }

  private void CloseMode(Mode mode) {
    switch (mode) {
      case Mode.Start:
        break;
      case Mode.Battle:
        battleUIHolder.gameObject.SetActive(false);
        break;
      case Mode.Inventory:
        inventoryUIHolder.CloseInventory();
        inventoryUIHolder.gameObject.SetActive(false);
        break;
    }
  }

  private void OpenMode(Mode mode) {
    switch (mode) {
      case Mode.Start:
        break;
      case Mode.Battle:
        switchContextText.text = "Return to Base";
        battleUIHolder.gameObject.SetActive(true);
        battleUIHolder.SetupWeaponBars();
        break;
      case Mode.Inventory:
        switchContextText.text = "Launch to battle";
        inventoryUIHolder.gameObject.SetActive(true);
        inventoryUIHolder.OpenInventory();
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
}
