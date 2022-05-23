using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;

public class FillingIconScript : MonoBehaviour {
  private Image baseBackground;
  private Image iconBackground;
  private Image icon;

  // Start is called before the first frame update
  protected void Awake() {
    baseBackground = GetComponent<Image>();
    iconBackground = this.FindInChild<Image>("IconBackground");
    icon = this.FindInChild<Image>("Icon");
  }

  /// Value is expected to be in 0..1 range.
  private void SetIconFill(float value) {
    iconBackground.fillAmount = value;
  }

  /// Value is expected to be in 0..1 range.
  public void SetIconFill(float value, float maxValue) {
    SetIconFill(value / maxValue);
  }

  public void SetIcon(string iconName, TextureHandler textureHandler) {
    textureHandler.UpdateTexture(iconName, icon, "Images/InventoryItems");
  }

  // Allows hiding without disabling, so it will still maintain its position in the vertical layout.
  public void SetVisible(bool isVisible) {
    baseBackground.enabled = isVisible;
    iconBackground.enabled = isVisible;
    icon.enabled = isVisible;
  }
}
