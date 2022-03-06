using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UIManagerScript : MonoBehaviour {
  private TMPro.TMP_Text switchContextText;

  private BattleMainManagerScript mainManager;
  private GameObject inventoryUIHolder;

  private EquipmentButtonScript[] equippedItemsButtons;
  private EquipmentButtonScript[] availableItemsButtons;

  private TextureHandler textureHandler = new();

  private EquipmentButtonScript selectedButton = null;

  // Start is called before the first frame update
  void Awake() {
    switchContextText = transform.Find("SwitchContext").transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = transform.Find("inventory").gameObject;
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

  private void SetupButtons(List<WeaponInstance> equipment, IEnumerable<EquipmentButtonScript> buttons) {
    buttons.ForEach((button, index) => {
      button.LoadEquipment(index < equipment.Count ? equipment[index] : null, textureHandler);
    });
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.SetActive(true);
    SetupButtons(Player.Instance.AvailableItems, availableItemsButtons);
    SetupButtons(Player.Instance.Weapons, equippedItemsButtons);
  }

  public List<WeaponInstance> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
    return buttons
      .Where(button => button.Equipment != null)
      .Select(button => button.Equipment)
      .ToList();
  }

  public void ToBattleMode() {
    Player.Instance.AvailableItems = ButtonsToEquipment(availableItemsButtons);
    Player.Instance.Weapons = ButtonsToEquipment(equippedItemsButtons);
    switchContextText.text = "Return to Base";
    inventoryUIHolder.SetActive(false);
  }

  public void SwitchContextPressed() {
    mainManager.SwitchContext();
  }

  public void InventoryButtonSelected(EquipmentButtonScript button) {
    Debug.Log("clicked " + button.gameObject.name);
    if (selectedButton == null) {
      selectedButton = button;
      selectedButton.GetComponent<Button>().Select();
    } else {
      var switchedEquipment = selectedButton.Equipment;
      selectedButton.LoadEquipment(button.Equipment, textureHandler);
      button.LoadEquipment(switchedEquipment, textureHandler);
      //selectedButton.GetComponent<Button>().();
      selectedButton = null;
    }
  }
}
