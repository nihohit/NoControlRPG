using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  private UIManagerScript manager;

  // Start is called before the first frame update
  protected void Awake() {
    manager = GameObject.FindObjectOfType<UIManagerScript>();
  }

  public void OnPointerEnter(PointerEventData eventData) {
    manager.MouseOverUpgradeButton();
  }

  public void OnPointerExit(PointerEventData eventData) {
    manager.MouseExitUpgradeButton();
  }
}
