using Assets.Scripts.UnityBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillingBarScript : MonoBehaviour {
  private Image background;
  private Image bar;
  private TMP_Text text;

  // Start is called before the first frame update
  protected void Awake() {
    background = GetComponent<Image>();
    bar = this.FindInChild<Image>("Bar");
    text = this.FindInChild<TMP_Text>("Text");
  }

  /// Value is expected to be in 0..1 range.
  public void SetBarFill(float value) {
    bar.fillAmount = value;
  }

  /// Value is expected to be in 0..1 range.
  public void SetBarFill(float value, float maxValue) {
    SetBarFill(value / maxValue);
  }

  /// Value is expected to be in 0..1 range.
  public void SetDescription(string description) {
    text.text = description;
  }

  // Allows hiding without disabling, so it will still maintain its position in the vertical layout.
  public void SetVisible(bool isVisible) {
    bar.gameObject.SetActive(isVisible);
    text.gameObject.SetActive(isVisible);
    background.enabled = isVisible;
  }
}
