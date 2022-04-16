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
  private readonly TextureHandler textureHandler = new();

  protected void Awake() {
    switchContextButton = this.FindInChild<Button>("SwitchContext");
    switchContextText = switchContextButton.FindInChild<TMPro.TMP_Text>("Text");
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = this.FindInChild<InventoryUIScript>("inventory");
    inventoryUIHolder.TextureHandler = textureHandler;
    inventoryUIHolder.SwitchContextText = switchContextText;
    battleUIHolder = this.FindInChild<BattleUIScript>("BattleUI");
    battleUIHolder.TextureHandler = textureHandler;
  }

  public void SwitchContextPressed() {
    if (!inventoryUIHolder.HasSufficientEnergy()) {
      return;
    }
    mainManager.SwitchContext();
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.gameObject.SetActive(true);
    battleUIHolder.gameObject.SetActive(false);
    inventoryUIHolder.OpenInventory();
  }

  public void ToBattleMode() {
    inventoryUIHolder.CloseInventory();
    switchContextText.text = "Return to Base";
    inventoryUIHolder.gameObject.SetActive(false);
    battleUIHolder.gameObject.SetActive(true);
    battleUIHolder.SetupWeaponBars();
  }
}
