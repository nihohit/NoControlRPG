using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;

public class BattleUIScript : MonoBehaviour {
  private FillingIconScript[] weaponBars;
  public TextureHandler TextureHandler { get; set; }

  protected void Awake() {
    weaponBars = this.FindChild("CooldownsContainer").GetComponentsInChildren<FillingIconScript>();
  }

  public void UpdateUIOverlay() {
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
