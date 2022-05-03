using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class GeneralUIScript : MonoBehaviour {
  private FillingBarScript healthBar;
  private FillingBarScript shieldBar;
  private FillingBarScript energyBar;
  // Start is called before the first frame update
  void Start() {
    healthBar = this.FindInChild<FillingBarScript>("HealthBar");
    shieldBar = this.FindInChild<FillingBarScript>("ShieldBar");
    energyBar = this.FindInChild<FillingBarScript>("EnergyBar");
  }

  public void UpdateUIOverlay() {
    const string barUiFormat = "{0}: {1:0.#}";
    healthBar.SetBarFill(Player.Instance.CurrentHealth, Player.Instance.FullHealth);
    healthBar.SetDescription(string.Format(barUiFormat, "Health", Player.Instance.CurrentHealth));
    shieldBar.SetBarFill(Player.Instance.CurrentShieldStrength, Player.Instance.MaxShieldStrength);
    shieldBar.SetDescription(string.Format(barUiFormat, "Shield", Player.Instance.CurrentShieldStrength));
    energyBar.SetBarFill(Player.Instance.CurrentEnergyLevel, Player.Instance.MaxEnergyLevel);
    energyBar.SetDescription(string.Format(barUiFormat, "Energy", Player.Instance.CurrentEnergyLevel));
  }
}
