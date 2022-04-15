using Assets.Scripts.UnityBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillingBarScript : MonoBehaviour {
  private Image bar;
  /// text might be null. Caller is expected to now whether setting text is possible.
  private TMP_Text text;

  // Start is called before the first frame update
  protected void Awake() {
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
}
