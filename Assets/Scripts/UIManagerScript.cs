using System.Collections.Generic;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.ObjectModel;
using TMPro;

public class UIManagerScript : MonoBehaviour {
  private Button switchContextButton;
  private TMPro.TMP_Text switchContextText;

  private BattleMainManagerScript mainManager;
  private GameObject inventoryUIHolder;
  private GameObject battleUIHolder;
  private Image healthBar;
  private Image shieldBar;

  private Dictionary<EquipmentType, List<EquipmentButtonScript>> equippedItemsButtons;
  private EquipmentButtonScript[] availableItemsButtons;

  private readonly TextureHandler textureHandler = new();

  private EquipmentButtonScript selectedButton = null;
  private GameObject itemTextBackground;
  private TMP_Text itemText;

  // Start is called before the first frame update
  protected void Awake() {
    switchContextButton = transform.Find("SwitchContext").GetComponent<Button>();
    switchContextText = switchContextButton.transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = transform.Find("inventory").gameObject;
    battleUIHolder = transform.Find("BattleUI").gameObject;
    healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    shieldBar = GameObject.Find("ShieldBar").GetComponent<Image>();
    var equippedItems = GameObject.Find("Equipped Items");
    equippedItemsButtons = GameObject
      .Find("Equipped Items")
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .GroupBy(button => button.RequiredEquipmentType)
      .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
    availableItemsButtons = GameObject
      .Find("Available Items")
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .ToArray();
    itemTextBackground = GameObject.Find("TextBackground").gameObject;
    itemText = itemTextBackground.GetComponentInChildren<TMPro.TMP_Text>();
    itemTextBackground.SetActive(false);
  }

  private void SetupAvailableButtons(ReadOnlyCollection<EquipmentBase> equipment, IEnumerable<EquipmentButtonScript> buttons) {
    buttons.ForEach((button, index) => {
      button.LoadEquipment(index < equipment.Count ? equipment[index] : null, textureHandler);
    });
  }

  private void SetupEquippedButtons(Player player) {
    equippedItemsButtons[EquipmentType.Weapon][0].LoadEquipment(player.Weapon1, textureHandler);
    equippedItemsButtons[EquipmentType.Weapon][1].LoadEquipment(player.Weapon2, textureHandler);
    equippedItemsButtons[EquipmentType.Reactor][0].LoadEquipment(player.Reactor, textureHandler);
    equippedItemsButtons[EquipmentType.Shield][0].LoadEquipment(player.Shield, textureHandler);
    equippedItemsButtons[EquipmentType.TargetingSystem][0].LoadEquipment(player.TargetingSystem, textureHandler);
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.SetActive(true);
    battleUIHolder.SetActive(false);
    SetupAvailableButtons(Player.Instance.AvailableItems, availableItemsButtons);
    SetupEquippedButtons(Player.Instance);
  }

  public ReadOnlyCollection<EquipmentBase> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
    return buttons
      .Where(button => button.Equipment != null)
      .Select(button => button.Equipment)
      .ToReadOnlyCollection();
  }

  public void ToBattleMode() {
    Player.Instance.StartRound(equippedItemsButtons[EquipmentType.Weapon][0].Equipment as WeaponBase,
      equippedItemsButtons[EquipmentType.Weapon][1].Equipment as WeaponBase,
      equippedItemsButtons[EquipmentType.Reactor][0].Equipment as ReactorInstance,
      equippedItemsButtons[EquipmentType.Shield][0].Equipment as ShieldInstance,
      equippedItemsButtons[EquipmentType.TargetingSystem][0].Equipment as TargetingSystemInstance,
      ButtonsToEquipment(availableItemsButtons),
      Player.INITIAL_HEALTH);
    switchContextText.text = "Return to Base";
    inventoryUIHolder.SetActive(false);
    battleUIHolder.SetActive(true);
  }

  public void UpdateUIOverlay() {
    healthBar.fillAmount = Player.Instance.CurrentHealth / Player.Instance.FullHealth;
    shieldBar.fillAmount = Player.Instance.Shield.CurrentStrength / Player.Instance.Shield.MaxStrength;
  }

  public void SwitchContextPressed() {
    if (!HasSufficientEnergy()) {
      return;
    }
    mainManager.SwitchContext();
  }

  private void SetLaunchButtonAvailability() {
    if (HasSufficientEnergy()) {
      switchContextText.text = "Launch to battle";
    } else {
      switchContextText.text = "Insufficient energy";
    }
  }

  private void SetSelectedItemText(EquipmentButtonScript button) {
    selectedButton = button;
    var equipment = selectedButton?.Equipment;
    itemTextBackground.SetActive(equipment != null);
    itemText.text = equipment?.ToString() ?? "";
    SetLaunchButtonAvailability();
  }

  private bool HasSufficientEnergy() {
    var energySum = (equippedItemsButtons[EquipmentType.Reactor][0].Equipment as ReactorInstance).RechargeRate;
    energySum -= equippedItemsButtons[EquipmentType.Shield][0].Equipment.BaselineEnergyRequirement;
    energySum -= equippedItemsButtons[EquipmentType.Weapon][0].Equipment.BaselineEnergyRequirement;
    energySum -= equippedItemsButtons[EquipmentType.Weapon][1]?.Equipment?.BaselineEnergyRequirement ?? 0;
    energySum -= equippedItemsButtons[EquipmentType.TargetingSystem][0].Equipment.BaselineEnergyRequirement;
    return energySum > 0;
  }

  public void InventoryButtonSelected(EquipmentButtonScript button) {
    if (selectedButton == null) {
      selectedButton = button;
      selectedButton.GetComponent<Button>().Select();
      SetSelectedItemText(button);
    } else {
      var switchedEquipment = selectedButton.Equipment;
      if (!button.IsValidEquipment(switchedEquipment) ||
          !selectedButton.IsValidEquipment(button.Equipment)) {
        SetSelectedItemText(button);
        //TODO maybe add some feedback?
        return;
      }
      selectedButton.LoadEquipment(button.Equipment, textureHandler);
      button.LoadEquipment(switchedEquipment, textureHandler);
      SetSelectedItemText(null);

    }
  }
}
