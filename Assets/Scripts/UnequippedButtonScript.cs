using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;

public class UnequippedButtonScript : MonoBehaviour {
  private UIManagerScript manager;
  private Button button;
  private Image spriteRenderer;

  public void LoadEquipment(string iconName, TextureHandler textureHandler) {
    textureHandler.UpdateTexture(iconName, spriteRenderer, "Images/InventoryItems");
  }

  // Start is called before the first frame update
  void Awake() {
    manager = GameObject.FindObjectOfType<UIManagerScript>();
    button = GetComponent<Button>();
    button.onClick.AddListener(OnClick);
    spriteRenderer = GetComponent<Image>();
  }

  private void OnClick() {
    manager.InventoryButtonSelected(this);
  }
}
