using System.Linq;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class BattleUIScript : MonoBehaviour {
  private FillingBarScript healthBar;
  private FillingBarScript shieldBar;
  private FillingBarScript energyBar;
  private FillingBarScript xpBar;
  private FillingIconScript[] weaponBars;
  public TextureHandler TextureHandler { get; set; }

  protected void Awake() {
    var barsContainer = this.FindChild("BarsContainer");
    healthBar = barsContainer.FindInChild<FillingBarScript>("HealthBar");
    shieldBar = barsContainer.FindInChild<FillingBarScript>("ShieldBar");
    energyBar = barsContainer.FindInChild<FillingBarScript>("EnergyBar");
    xpBar = this.FindInChild<FillingBarScript>("XPBar");
    weaponBars = this.FindChild("CooldownsContainer").GetComponentsInChildren<FillingIconScript>();
  }

  public void UpdateUIOverlay() {
    const string barUiFormat = "{0}: {1:0.#}";
    healthBar.SetBarFill(Player.Instance.CurrentHealth, Player.Instance.FullHealth);
    healthBar.SetDescription(string.Format(barUiFormat, "Health", Player.Instance.CurrentHealth));
    shieldBar.SetBarFill(Player.Instance.CurrentShieldStrength, Player.Instance.MaxShieldStrength);
    shieldBar.SetDescription(string.Format(barUiFormat, "Shield", Player.Instance.CurrentShieldStrength));
    energyBar.SetBarFill(Player.Instance.CurrentEnergyLevel, Player.Instance.MaxEnergyLevel);
    energyBar.SetDescription(string.Format(barUiFormat, "Energy", Player.Instance.CurrentEnergyLevel));
    xpBar.SetBarFill(Player.Instance.XP, Player.Instance.XPToNextLevel);
    xpBar.SetDescription(string.Format(barUiFormat, "XP", Player.Instance.XP));
    Player.Instance.Weapons.ForEach((weapon, index) => {
      weaponBars[index].SetIconFill(weapon.CurrentCharge, weapon.MaxCharge);
    });
  }

  public void SetupWeaponBars() {
    weaponBars.ForEach((bar, index) => {
      var isActiveIndex = index < Player.Instance.Weapons.Count;
      bar.SetVisible(isActiveIndex);
      if (isActiveIndex) {
        bar.SetIcon(Player.Instance.Weapons[index].Config.EquipmentImageName, TextureHandler);
      }
    });
  }
}
