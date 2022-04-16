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
  private List<EquipmentButtonScript> equippedItemsButtons;
  private EquipmentButtonScript[] availableItemsButtons;
  public TextureHandler TextureHandler { get; set; }
  private EquipmentButtonScript selectedButton = null;
  private GameObject selectedItemTextBackground;
  private TMP_Text selectedItemText;
  private GameObject hoveredItemTextBackground;
  private TMP_Text hoveredItemText;
  private TMP_Text attributeText;
  private EventSystem eventSystem;

  // Start is called before the first frame update
  protected void Awake() {
    equippedItemsButtons = GameObject
      .Find("Equipped Items")
      .GetComponentsInChildren<EquipmentButtonScript>()
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

  public void OpenInventory() {
    SetupAvailableButtons(Player.Instance.AvailableItems, availableItemsButtons);
    SetupAvailableButtons(Player.Instance.EquippedItems, equippedItemsButtons);
    UpdateAttributes();
    SetLaunchButtonAvailability();
    SetSelectedItem(null);
  }

  public List<EquipmentBase> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
    return buttons
      .Where(button => button.Equipment != null)
      .Select(button => button.Equipment)
      .ToList();
  }

  public void CloseInventory() {
    Player.Instance.StartRound(ButtonsToEquipment(equippedItemsButtons).ToReadOnlyCollection(),
      ButtonsToEquipment(availableItemsButtons),
      Player.INITIAL_HEALTH);
  }



  private void SetLaunchButtonAvailability() {
    if (HasSufficientEnergy()) {
      SwitchContextText.text = "Launch to battle";
    } else {
      SwitchContextText.text = "Insufficient energy";
    }
  }

  private void ShowItem(EquipmentBase equipment, GameObject textBackground, TMP_Text textBox) {
    textBackground.SetActive(equipment != null);
    textBox.text = equipment?.ToString() ?? "";
  }

  private void SetSelectedItem(EquipmentButtonScript button) {
    eventSystem.SetSelectedGameObject(button?.gameObject);
    selectedButton = button;
    ShowItem(button?.Equipment, selectedItemTextBackground, selectedItemText);
    SetLaunchButtonAvailability();
    var hasEquipment = button?.Equipment != null;
    upgradeItemButton.gameObject.SetActive(hasEquipment);
    scrapItemButton.gameObject.SetActive(hasEquipment);
    if (hasEquipment) {
      upgradeItemText.text = $"Upgrade cost: {button.Equipment.UpgradeCost}";
      scrapItemText.text = $"Scrap value: {button.Equipment.ScrapValue}";
      upgradeItemButton.interactable = button.Equipment.UpgradeCost <= Player.Instance.Scrap;
    }
  }

  public bool HasSufficientEnergy() {
    return equippedItemsButtons.GetEnergyGeneration() > 0f;
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
      UpdateAttributes();
    }
  }

  public void PointerEnterButton(EquipmentButtonScript button) {
    ShowItem(button.Equipment, hoveredItemTextBackground, hoveredItemText);
  }

  public void PointerExitButton(EquipmentButtonScript button) {
    ShowItem(null, hoveredItemTextBackground, hoveredItemText);
  }

  public void MouseOverUpgradeButton() {
    ShowItem(selectedButton?.Equipment?.UpgradedVersion(), hoveredItemTextBackground, hoveredItemText);
  }

  public void MouseExitUpgradeButton() {
    ShowItem(null, hoveredItemTextBackground, hoveredItemText);
  }

  public void UpgradeButtonPressed() {
    Assert.NotNull(selectedButton, nameof(selectedButton));
    Assert.NotNull(selectedButton.Equipment, nameof(selectedButton.Equipment));
    Assert.EqualOrGreater(Player.Instance.Scrap, selectedButton.Equipment.UpgradeCost);
    Player.Instance.Scrap -= selectedButton.Equipment.UpgradeCost;
    selectedButton.LoadEquipment(selectedButton.Equipment.UpgradedVersion(), TextureHandler);
    SetSelectedItem(selectedButton);
    UpdateAttributes();
  }

  public void ScrapButtonPressed() {
    Assert.NotNull(selectedButton, nameof(selectedButton));
    Assert.NotNull(selectedButton.Equipment, nameof(selectedButton.Equipment));
    Player.Instance.Scrap += selectedButton.Equipment.ScrapValue;
    selectedButton.LoadEquipment(null, TextureHandler);
    DeselectEquipmentButton();
    UpdateAttributes();
  }

  public void Update() {
    if (!eventSystem.alreadySelecting && eventSystem.currentSelectedGameObject == null) {
      DeselectEquipmentButton();
    }
  }
}
