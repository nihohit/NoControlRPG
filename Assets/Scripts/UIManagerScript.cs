using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
  private TMP_Text attributeText;
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
    attributeText = inventoryUIHolder.transform.Find("Attributes").GetComponent<TMP_Text>();
  }

  private void SetupAvailableButtons(IList<EquipmentBase> equipment, IEnumerable<EquipmentButtonScript> buttons) {
    // TODO - handle the items that don't appear in buttons, or add scrolling bar.
    var sortedEquipment = equipment.OrderByDescending(item => item.Level);
    buttons.ForEach((button, index) => {
      button.LoadEquipment(index < equipment.Count ? equipment[index] : null, textureHandler);
    });
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.SetActive(true);
    battleUIHolder.SetActive(false);
    SetupAvailableButtons(Player.Instance.AvailableItems, availableItemsButtons);
    SetupAvailableButtons(Player.Instance.EquippedItems, equippedItemsButtons);
    UpdateAttributes();
    SetLaunchButtonAvailability();
  }

  public List<EquipmentBase> ButtonsToEquipment(IEnumerable<EquipmentButtonScript> buttons) {
    return buttons
      .Where(button => button.Equipment != null)
      .Select(button => button.Equipment)
      .ToList();
  }

  public void ToBattleMode() {
    Player.Instance.StartRound(ButtonsToEquipment(equippedItemsButtons).ToReadOnlyCollection(),
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
    shieldBar.fillAmount = Player.Instance.CurrentShieldStrength / Player.Instance.MaxShieldStrength;
    shieldText.text = string.Format(barUiFormat, "Shield", Player.Instance.CurrentShieldStrength);
    energyBar.fillAmount = Player.Instance.CurrentEnergyLevel / Player.Instance.MaxEnergyLevel;
    energyText.text = string.Format(barUiFormat, "Energy", Player.Instance.CurrentEnergyLevel);
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

    attributeText.text = stringBuilder.ToString();
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
      UpdateAttributes();
    }
  }

  public void PointerEnterButton(EquipmentButtonScript button) {
    ShowItem(button, hoveredItemTextBackground, hoveredItemText);
  }

  public void PointerExitButton(EquipmentButtonScript button) {
    ShowItem(null, hoveredItemTextBackground, hoveredItemText);
  }
}
