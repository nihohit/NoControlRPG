using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIScript : MonoBehaviour {
  private Button upgradeItemButton;
  private TMP_Text upgradeItemText;
  private Button scrapItemButton;
  private TMP_Text scrapItemText;
  private Button fixItemButton;
  private TMP_Text fixItemText;
  private GameObject equippedItemsContainer;
  private EquipmentButtonScript[] equippedItemsButtons;
  private EquipmentButtonScript[] availableItemsButtons;
  public TextureHandler TextureHandler { get; set; }
  private EquipmentButtonScript selectedButton;
  private GameObject selectedItemTextBackground;
  private TMP_Text selectedItemText;
  private GameObject hoveredItemTextBackground;
  private TMP_Text hoveredItemText;
  private TMP_Text attributeText;
  private EventSystem eventSystem;

  // Start is called before the first frame update
  protected void Awake() {
    equippedItemsContainer = GameObject.Find("Equipped Items");
    equippedItemsButtons = equippedItemsContainer
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => {
        var position = button.transform.position;
        return -position.y * 100000 + position.x;
      })
      .ToArray();
    availableItemsButtons = GameObject
      .Find("Available Items")
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => {
        var position = button.transform.position;
        return -position.y * 100000 + position.x;
      })
      .ToArray();
    selectedItemTextBackground = GameObject.Find("SelectedItemTextBackground").gameObject;
    selectedItemText = selectedItemTextBackground.GetComponentInChildren<TMP_Text>();
    hoveredItemTextBackground = GameObject.Find("HoveredItemTextBackground").gameObject;
    hoveredItemText = hoveredItemTextBackground.GetComponentInChildren<TMP_Text>();
    hoveredItemTextBackground.SetActive(false);
    eventSystem = FindObjectOfType<EventSystem>();
    attributeText = this.FindInChild<TMP_Text>("Attributes");
    upgradeItemButton = this.FindInChild<Button>("UpgradeButton");
    upgradeItemText = upgradeItemButton.FindInChild<TMP_Text>("Text");
    scrapItemButton = this.FindInChild<Button>("ScrapButton");
    scrapItemText = scrapItemButton.FindInChild<TMP_Text>("Text");
    fixItemButton = this.FindInChild<Button>("FixButton");
    fixItemText = fixItemButton.FindInChild<TMP_Text>("Text");
  }

  private void SetupAvailableButtons(IList<EquipmentBase> equipment, EquipmentButtonScript[] buttons) {
    // TODO - handle the items that don't appear in buttons, or add scrolling bar.
    var itemsDictionary = equipment
      .ToDictionary(item => item.Identifier,
                    item => item);
    var buttonsDictionary = buttons
      .Where(button => button.Equipment != null)
      .ToDictionary(button => button.Equipment.Identifier,
                    button => button);
    var unmatchedItems = equipment
      .Where(item => !buttonsDictionary.ContainsKey(item.Identifier))
      .ToList();
    buttons
      .Where(button => !itemsDictionary.ContainsKey(button.Equipment?.Identifier ?? Guid.Empty))
      .ForEach((button, index) => button.LoadEquipment(index < unmatchedItems.Count ? unmatchedItems[index] : null, TextureHandler));
  }

  public void OpenInventory() {
    SetSelectedItem(null);
  }

  private List<EquipmentBase> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
    return buttons
      .Where(button => button.Equipment != null)
      .Select(button => button.Equipment)
      .ToList();
  }

  private void ShowItem(EquipmentBase equipment, (GameObject, TMP_Text) textAndBackground) {
    var (textBackground, textBox) = textAndBackground;
    textBackground.SetActive(equipment != null);
    textBox.text = equipment?.ToString() ?? "";
  }

  private void SetSelectedItem([CanBeNull] EquipmentButtonScript button) {
    eventSystem.SetSelectedGameObject(button?.gameObject);
    selectedButton = button;
    ShowItem(button?.Equipment, (selectedItemTextBackground, selectedItemText));
    var hasEquipment = HasEquipment();
    upgradeItemButton.gameObject.SetActive(hasEquipment);
    fixItemButton.gameObject.SetActive(hasEquipment);
    scrapItemButton.gameObject.SetActive(hasEquipment);
    if (!hasEquipment) {
      return;
    }

    var equipment = button.Equipment;
    upgradeItemText.text = $"Upgrade cost: {equipment.UpgradeCost}";
    fixItemText.text = $"Fix cost: {equipment.FixCost}";
    scrapItemText.text = $"Scrap value: {equipment.ScrapValue}";
  }

  private void UpdateAttributes() {
    var stringBuilder = new StringBuilder();

    var reactors = equippedItemsButtons.AllOfType<ReactorInstance>();
    var shields = equippedItemsButtons.AllOfType<ShieldInstance>();
    stringBuilder.AppendLine($"Shield strength: {shields.Sum(shield => shield.MaxStrength):f1}");
    stringBuilder.AppendLine($"Shield recharge: {shields.Sum(shield => shield.RechargeRatePerSecond):f1}");
    stringBuilder.AppendLine($"Energy charge: {reactors.Sum(reactor => reactor.MaxEnergyLevel):f1}");
    stringBuilder.AppendLine($"Energy recharge: {equippedItemsButtons.GetEnergyGeneration():f1}");
    stringBuilder.AppendLine($"Shields recharge cost: {equippedItemsButtons.AllOfType<ShieldInstance>().Sum(shield => shield.EnergyConsumptionWhenRechargingPerSecond):f1}");
    stringBuilder.AppendLine($"Weapons recharge cost: {equippedItemsButtons.AllOfType<WeaponBase>().Sum(weapon => weapon.EnergyConsumptionWhenRechargingPerSecond):f1}");
    stringBuilder.AppendLine($"Scrap: {Player.Instance.Scrap}");

    attributeText.text = stringBuilder.ToString();
  }

  private void DeselectEquipmentButton() {
    SetSelectedItem(null);
  }

  public void InventoryButtonSelected(EquipmentButtonScript button) {
    if (selectedButton == null || 
        selectedButton.Equipment == null) {
      selectedButton = button;
      SetSelectedItem(button);
    } else {
      var switchedEquipment = selectedButton.Equipment;
      selectedButton.LoadEquipment(button.Equipment, TextureHandler);
      button.LoadEquipment(switchedEquipment, TextureHandler);
      DeselectEquipmentButton();
    }
    InventoryStateChangedInternally();
  }

  private (GameObject, TMP_Text) BackgroundToUseForButton() {
    return selectedButton == null ?
      (selectedItemTextBackground, selectedItemText) :
      (hoveredItemTextBackground, hoveredItemText);
  }

  public void PointerEnterButton(EquipmentButtonScript button) {
    if (button == selectedButton) {
      return;
    }
    ShowItem(button.Equipment, BackgroundToUseForButton());
  }

  public void PointerExitButton(EquipmentButtonScript button) {
    ShowItem(null, BackgroundToUseForButton());
  }

  public void MouseOverUpgradeButton() {
    ShowItem(selectedButton?.Equipment?.UpgradedVersion(),
      (hoveredItemTextBackground, hoveredItemText));
  }

  public void MouseExitUpgradeButton() {
    ShowItem(null, (hoveredItemTextBackground, hoveredItemText));
  }

  private void ForgeButtonPressed(Forge.Action.Type actionType) {
    Assert.NotNull(selectedButton, nameof(selectedButton));
    var equipment = selectedButton.Equipment;
    Assert.NotNull(equipment, nameof(equipment));
    var cost = actionType == Forge.Action.Type.Repair ? equipment.FixCost : equipment.UpgradeCost;
    Assert.EqualOrGreater(Player.Instance.Scrap, cost);
    var forgeAction = actionType == Forge.Action.Type.Repair ? 
      Forge.Instance.Repair(equipment) : 
      Forge.Instance.Upgrade(equipment);
    selectedButton.SetForgeAction(forgeAction);
    
    InventoryStateChangedInternally();
  }
  
  public void UpgradeButtonPressed() {
    ForgeButtonPressed(Forge.Action.Type.Upgrade);
  }
  
  public void FixButtonPressed() {
    ForgeButtonPressed(Forge.Action.Type.Repair);
  }

  public void ScrapButtonPressed() {
    Assert.NotNull(selectedButton, nameof(selectedButton));
    Assert.NotNull(selectedButton.Equipment, nameof(selectedButton.Equipment));
    Player.Instance.Scrap += selectedButton.Equipment.ScrapValue;
    selectedButton.LoadEquipment(null, TextureHandler);
    DeselectEquipmentButton();
    InventoryStateChangedInternally();
  }

  public void UpdateFrame() {
    if (!eventSystem.alreadySelecting && 
        eventSystem.currentSelectedGameObject == null && 
        selectedButton != null) {
      DeselectEquipmentButton();
      ShowItem(null, (hoveredItemTextBackground, hoveredItemText));
    }
    UpdateAttributes();

    if (Player.Instance.AvailableItemsChangedThisFrame) {
      SetupAvailableButtons(Player.Instance.AvailableItems, availableItemsButtons);
    }
    if (Player.Instance.EquippedItemsChangedThisFrame) {
      SetupAvailableButtons(Player.Instance.EquippedItems, equippedItemsButtons);
    }

    foreach (var button in equippedItemsButtons.Concat(availableItemsButtons)) {
      button.FrameUpdate();
    }

    var results = Forge.Instance.Advance(Time.deltaTime);
    if (results.Any()) {
      InventoryStateChangedInternally();
    }
    
    // needs to happen every frame, since IsDamaged can change.
    fixItemButton.interactable =
      HasEquipment() &&
      selectedButton.Equipment.IsDamaged && 
      selectedButton.Equipment.FixCost <= Player.Instance.Scrap &&
      !selectedButton.Equipment.IsBeingForged;
  }

  private bool HasEquipment() {
    return selectedButton is not null && selectedButton.Equipment is not null;
  }
  
  private void RefreshInventoryState() {
    upgradeItemButton.interactable =
      HasEquipment() &&
      selectedButton.Equipment.UpgradeCost <= Player.Instance.Scrap &&
      !selectedButton.Equipment.IsBeingForged;
  }

  private void InventoryStateChangedInternally() {
    Player.Instance.ChangeEquipment(
      ButtonsToEquipment(equippedItemsButtons).ToReadOnlyCollection(),
      ButtonsToEquipment(availableItemsButtons).ToReadOnlyCollection());
    RefreshInventoryState();
  }
}
