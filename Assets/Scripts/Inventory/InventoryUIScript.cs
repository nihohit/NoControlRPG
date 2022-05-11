using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIScript : MonoBehaviour {
  private Button upgradeItemButton;
  public TMP_Text SwitchContextText { get; set; }
  private TMP_Text upgradeItemText;
  private Button scrapItemButton;
  private TMP_Text scrapItemText;
  private GameObject equippedItemsContainer;
  private GameObject forgeItemsContainer;
  private List<EquipmentButtonScript> equippedItemsButtons;
  private List<ForgeButtonScript> forgeItemsButtons;
  private EquipmentButtonScript[] availableItemsButtons;
  public TextureHandler TextureHandler { get; set; }
  private EquipmentButtonScript selectedButton = null;
  private GameObject selectedItemTextBackground;
  private TMP_Text selectedItemText;
  private GameObject hoveredItemTextBackground;
  private TMP_Text hoveredItemText;
  private TMP_Text attributeText;
  private EventSystem eventSystem;
  private Mode mode;

  // Start is called before the first frame update
  protected void Awake() {
    equippedItemsContainer = GameObject.Find("Equipped Items");
    equippedItemsButtons = equippedItemsContainer
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .ToList();
    forgeItemsContainer = GameObject.Find("Forge Items");
    forgeItemsButtons = forgeItemsContainer
      .GetComponentsInChildren<ForgeButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .ToList();
    availableItemsButtons = GameObject
      .Find("Available Items")
      .GetComponentsInChildren<EquipmentButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .ToArray();
    selectedItemTextBackground = GameObject.Find("SelectedItemTextBackground").gameObject;
    selectedItemText = selectedItemTextBackground.GetComponentInChildren<TMPro.TMP_Text>();
    hoveredItemTextBackground = GameObject.Find("HoveredItemTextBackground").gameObject;
    hoveredItemText = hoveredItemTextBackground.GetComponentInChildren<TMPro.TMP_Text>();
    hoveredItemTextBackground.SetActive(false);
    eventSystem = FindObjectOfType<EventSystem>();
    attributeText = this.FindInChild<TMP_Text>("Attributes");
    upgradeItemButton = this.FindInChild<Button>("UpgradeButton");
    upgradeItemText = upgradeItemButton.FindInChild<TMP_Text>("Text");
    scrapItemButton = this.FindInChild<Button>("ScrapButton");
    scrapItemText = scrapItemButton.FindInChild<TMP_Text>("Text");
  }

  private void SetupAvailableButtons(IList<EquipmentBase> equipment, IEnumerable<EquipmentButtonScript> buttons) {
    // TODO - handle the items that don't appear in buttons, or add scrolling bar.
    var sortedEquipment = equipment.OrderByDescending(item => item.Level);
    buttons.ForEach((button, index) => {
      button.LoadEquipment(index < equipment.Count ? equipment[index] : null, TextureHandler);
    });
  }

  private void Open(Mode mode) {
    this.mode = mode;
    UpdateInventoryStateExternally();
    SetSelectedItem(null);
    equippedItemsContainer.SetActive(mode == Mode.Inventory);
    forgeItemsContainer.SetActive(mode == Mode.Forge);
  }

  public void OpenInventory() {
    Open(Mode.Inventory);
  }

  public void OpenForge() {
    Open(Mode.Forge);
  }

  public List<EquipmentBase> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
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

  private void SetSelectedItem(EquipmentButtonScript button) {
    eventSystem.SetSelectedGameObject(button?.gameObject);
    selectedButton = button;
    ShowItem(button?.Equipment, (selectedItemTextBackground, selectedItemText));
    var hasEquipment = button?.Equipment != null;
    upgradeItemButton.gameObject.SetActive(hasEquipment && mode == Mode.Forge);
    scrapItemButton.gameObject.SetActive(hasEquipment);
    if (hasEquipment) {
      upgradeItemText.text = $"Upgrade cost: {button.Equipment.UpgradeCost}";
      scrapItemText.text = $"Scrap value: {button.Equipment.ScrapValue}";
    }
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
    if (selectedButton == null) {
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
    ShowItem(selectedButton?.Equipment?.UpgradedVersion(), (hoveredItemTextBackground, hoveredItemText));
  }

  public void MouseExitUpgradeButton() {
    ShowItem(null, (hoveredItemTextBackground, hoveredItemText));
  }

  public void UpgradeButtonPressed() {
    Assert.NotNull(selectedButton, nameof(selectedButton));
    Assert.NotNull(selectedButton.Equipment, nameof(selectedButton.Equipment));
    Assert.EqualOrGreater(Player.Instance.Scrap, selectedButton.Equipment.UpgradeCost);
    var forgeButton = forgeItemsButtons.First(button => button.Equipment is null);
    Player.Instance.Scrap -= selectedButton.Equipment.UpgradeCost;
    forgeButton.StartProcess(selectedButton.Equipment, TextureHandler, ForgeAction.Upgrade);
    selectedButton.LoadEquipment(null, TextureHandler);
    SetSelectedItem(null);
    InventoryStateChangedInternally();
  }

  public void ScrapButtonPressed() {
    Assert.NotNull(selectedButton, nameof(selectedButton));
    Assert.NotNull(selectedButton.Equipment, nameof(selectedButton.Equipment));
    Player.Instance.Scrap += selectedButton.Equipment.ScrapValue;
    selectedButton.LoadEquipment(null, TextureHandler);
    DeselectEquipmentButton();
    InventoryStateChangedInternally();
  }

  public void Update() {
    if (!eventSystem.alreadySelecting && eventSystem.currentSelectedGameObject == null && selectedButton != null) {
      DeselectEquipmentButton();
      ShowItem(null, (hoveredItemTextBackground, hoveredItemText));
    }
    foreach (var button in forgeItemsButtons) {
      if (button.Equipment == null) {
        continue;
      }
      button.AdvanceProgress(Time.deltaTime);
      if (button.Progress >= 1) {
        var equipment = button.Action == ForgeAction.Repair ? button.Equipment : button.Equipment.UpgradedVersion();
        Player.Instance.AvailableItems.Add(equipment);
        button.RemoveEquipment();
        UpdateInventoryStateExternally();
      }
    }
  }

  private void RefreshInventoryState() {
    UpdateAttributes();
    upgradeItemButton.interactable =
      selectedButton?.Equipment != null &&
      selectedButton?.Equipment?.UpgradeCost <= Player.Instance.Scrap &&
      forgeItemsButtons.Any(button => button.Equipment is null);
  }

  public void UpdateInventoryStateExternally() {
    SetupAvailableButtons(Player.Instance.AvailableItems, availableItemsButtons);
    SetupAvailableButtons(Player.Instance.EquippedItems, equippedItemsButtons);
    RefreshInventoryState();
  }

  private void InventoryStateChangedInternally() {
    Player.Instance.ChangeEquipment(
      ButtonsToEquipment(equippedItemsButtons).ToReadOnlyCollection(),
      ButtonsToEquipment(availableItemsButtons));
    RefreshInventoryState();
  }
}
