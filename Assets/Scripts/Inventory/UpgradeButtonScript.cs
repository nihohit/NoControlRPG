using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  private InventoryUIScript manager;

  // Start is called before the first frame update
  protected void Awake() {
    manager = FindObjectOfType<InventoryUIScript>();
  }

  public void OnPointerEnter(PointerEventData eventData) {
    manager.MouseOverUpgradeButton();
  }

  public void OnPointerExit(PointerEventData eventData) {
    manager.MouseExitUpgradeButton();
  }
}
