using System.Collections.Generic;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class UIManagerScript : MonoBehaviour {
  private Button switchContextButton;
  private TMP_Text switchContextText;
  private TMP_Text healthText;
  private TMP_Text shieldText;
  private TMP_Text energyText;


  private BattleMainManagerScript mainManager;
  private GameObject inventoryUIHolder;
  private GameObject battleUIHolder;
  private Image healthBar;
  private Image shieldBar;
  private Image energyBar;

  private List<EquipmentButtonScript> equippedItemsButtons;
  private EquipmentButtonScript[] availableItemsButtons;

  private readonly TextureHandler textureHandler = new();

  private EquipmentButtonScript selectedButton = null;
  private GameObject selectedItemTextBackground;
  private TMP_Text selectedItemText;
  private GameObject hoveredItemTextBackground;
  private TMP_Text hoveredItemText;
  private EventSystem eventSystem;

  // Start is called before the first frame update
  protected void Awake() {
    switchContextButton = transform.Find("SwitchContext").GetComponent<Button>();
    switchContextText = switchContextButton.transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    healthText = GameObject.Find("HealthText").GetComponent<TMPro.TMP_Text>();
    shieldText = GameObject.Find("ShieldText").GetComponent<TMPro.TMP_Text>();
    energyText = GameObject.Find("EnergyText").GetComponent<TMPro.TMP_Text>();
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = transform.Find("inventory").gameObject;
    battleUIHolder = transform.Find("BattleUI").gameObject;
    healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    shieldBar = GameObject.Find("ShieldBar").GetComponent<Image>();
    energyBar = GameObject.Find("EnergyBar").GetComponent<Image>();
    var equippedItems = GameObject.Find("Equipped Items");
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
    selectedItemTextBackground.SetActive(false);
    hoveredItemTextBackground = GameObject.Find("HoveredItemTextBackground").gameObject;
    hoveredItemText = hoveredItemTextBackground.GetComponentInChildren<TMPro.TMP_Text>();
    hoveredItemTextBackground.SetActive(false);
    eventSystem = FindObjectOfType<EventSystem>();
  }

  private void SetupAvailableButtons(IList<EquipmentBase> equipment, IEnumerable<EquipmentButtonScript> buttons) {
    // TODO - handle the items that don't appear in buttons, or add scrolling bar.
    var sortedEquipment = equipment.OrderByDescending(item => item.Level);
    buttons.ForEach((button, index) => {
      button.LoadEquipment(index < equipment.Count ? equipment[index] : null, textureHandler);
    });
  }

  private void SetupEquippedButtons(Player player) {
    equippedItemsButtons[0].LoadEquipment(player.Weapon1, textureHandler);
    equippedItemsButtons[1].LoadEquipment(player.Weapon2, textureHandler);
    equippedItemsButtons[2].LoadEquipment(player.Reactor, textureHandler);
    equippedItemsButtons[3].LoadEquipment(player.Shield, textureHandler);
    equippedItemsButtons[4].LoadEquipment(player.TargetingSystem, textureHandler);
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.SetActive(true);
    battleUIHolder.SetActive(false);
    SetupAvailableButtons(Player.Instance.AvailableItems, availableItemsButtons);
    SetupEquippedButtons(Player.Instance);
  }

  public List<EquipmentBase> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
    return buttons
      .Where(button => button.Equipment != null)
      .Select(button => button.Equipment)
      .ToList();
  }

  public void ToBattleMode() {
    var weapons = equippedItemsButtons.AllOfType<WeaponBase>();
    Player.Instance.StartRound(weapons[0],
      weapons.Count > 1 ? weapons[1] : null,
      equippedItemsButtons.AllOfType<ReactorInstance>()[0],
      equippedItemsButtons.AllOfType<ShieldInstance>()[0],
      equippedItemsButtons.AllOfType<TargetingSystemInstance>()[0],
      ButtonsToEquipment(availableItemsButtons),
      Player.INITIAL_HEALTH);
    switchContextText.text = "Return to Base";
    inventoryUIHolder.SetActive(false);
    battleUIHolder.SetActive(true);
  }

  public void UpdateUIOverlay() {
    string barUiFormat = "{0}: {1:f2}";
    healthBar.fillAmount = Player.Instance.CurrentHealth / Player.Instance.FullHealth;
    healthText.text = string.Format(barUiFormat, "Health", Player.Instance.CurrentHealth);
    shieldBar.fillAmount = Player.Instance.Shield.CurrentStrength / Player.Instance.Shield.MaxStrength;
    shieldText.text = string.Format(barUiFormat, "Shield", Player.Instance.Shield.CurrentStrength);
    energyBar.fillAmount = Player.Instance.Reactor.CurrentEnergyLevel / Player.Instance.Reactor.MaxEnergyLevel;
    energyText.text = string.Format(barUiFormat, "Energy", Player.Instance.Reactor.CurrentEnergyLevel);
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

  private void ShowItem(EquipmentButtonScript button, GameObject textBackground, TMP_Text textBox) {
    var equipment = button?.Equipment;
    textBackground.SetActive(equipment != null);
    textBox.text = equipment?.ToString() ?? "";
  }

  private void SetSelectedItemText(EquipmentButtonScript button) {
    selectedButton = button;
    ShowItem(button, selectedItemTextBackground, selectedItemText);
    SetLaunchButtonAvailability();
  }

  private bool HasSufficientEnergy() {
    var energyProduction = equippedItemsButtons.AllOfType<ReactorInstance>().Sum(reactor => reactor.EnergyRecoveryPerSecond);
    var baselineEnergyConsumption = equippedItemsButtons.Sum(button => button?.Equipment?.BaselineEnergyRequirement ?? 0);
    return energyProduction > baselineEnergyConsumption;
  }

  public void InventoryButtonSelected(EquipmentButtonScript button) {
    if (selectedButton == null) {
      selectedButton = button;
      SetSelectedItemText(button);
    } else {
      var switchedEquipment = selectedButton.Equipment;
      selectedButton.LoadEquipment(button.Equipment, textureHandler);
      button.LoadEquipment(switchedEquipment, textureHandler);
      SetSelectedItemText(null);
      eventSystem.SetSelectedGameObject(null);
    }
  }

  public void PointerEnterButton(EquipmentButtonScript button) {
    ShowItem(button, hoveredItemTextBackground, hoveredItemText);
  }

  public void PointerExitButton(EquipmentButtonScript button) {
    ShowItem(null, hoveredItemTextBackground, hoveredItemText);
  }
}
