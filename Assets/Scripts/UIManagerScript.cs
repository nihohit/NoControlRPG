using System.Collections.Generic;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.ObjectModel;

public class UIManagerScript : MonoBehaviour {
  private TMPro.TMP_Text switchContextText;

  private BattleMainManagerScript mainManager;
  private GameObject inventoryUIHolder;
  private GameObject battleUIHolder;
  private Image healthBar;

  private EquipmentButtonScript[] equippedItemsButtons;
  private EquipmentButtonScript[] availableItemsButtons;

  private readonly TextureHandler textureHandler = new();

  private EquipmentButtonScript selectedButton = null;

  // Start is called before the first frame update
  void Awake() {
    switchContextText = transform.Find("SwitchContext").transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = transform.Find("inventory").gameObject;
    battleUIHolder = transform.Find("BattleUI").gameObject;
    healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    var equippedItems = GameObject.Find("Equipped Items");
    equippedItemsButtons = GameObject
      .Find("Equipped Items")
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .ToArray();
    availableItemsButtons = GameObject
      .Find("Available Items")
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .ToArray();
  }

  private void SetupButtons(ReadOnlyCollection<WeaponBase> equipment, IEnumerable<EquipmentButtonScript> buttons) {
    buttons.ForEach((button, index) => {
      button.LoadEquipment(index < equipment.Count ? equipment[index] : null, textureHandler);
    });
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.SetActive(true);
    battleUIHolder.SetActive(false);
    SetupButtons(Player.Instance.AvailableItems, availableItemsButtons);
    SetupButtons(Player.Instance.Weapons, equippedItemsButtons);
  }

  public ReadOnlyCollection<WeaponBase> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
    return buttons
      .Where(button => button.Equipment != null)
      .Select(button => button.Equipment)
      .ToReadOnlyCollection();
  }

  public void ToBattleMode() {
    Player.Instance.StartRound(ButtonsToEquipment(equippedItemsButtons), ButtonsToEquipment(availableItemsButtons), Player.INITIAL_HEALTH);
    switchContextText.text = "Return to Base";
    inventoryUIHolder.SetActive(false);
    battleUIHolder.SetActive(true);
  }

  public void UpdatePlayerHealthOverlay() {
    healthBar.fillAmount = Player.Instance.CurrentHealth / Player.Instance.FullHealth;
  }

  public void SwitchContextPressed() {
    mainManager.SwitchContext();
  }

  public void InventoryButtonSelected(EquipmentButtonScript button) {
    if (selectedButton == null) {
      selectedButton = button;
      selectedButton.GetComponent<Button>().Select();
    } else {
      var switchedEquipment = selectedButton.Equipment;
      selectedButton.LoadEquipment(button.Equipment, textureHandler);
      button.LoadEquipment(switchedEquipment, textureHandler);
      selectedButton = null;
    }
  }
}
