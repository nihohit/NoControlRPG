using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {
  private TMPro.TMP_Text switchContextText;

  private BattleMainManagerScript mainManager;
  private GameObject inventoryUIHolder;

  // Start is called before the first frame update
  void Awake() {
    switchContextText = transform.Find("SwitchContext").transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = transform.Find("inventory").gameObject;
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.SetActive(true);
  }

  public void ToBattleMode() {
    switchContextText.text = "Return to Base";
    inventoryUIHolder.SetActive(false);
  }

  public void SwitchContextPressed() {
    mainManager.SwitchContext();
  }
}
