using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {
  private TMPro.TMP_Text switchContextText;

  private BattleMainManagerScript mainManager;

  // Start is called before the first frame update
  void Awake() {
    switchContextText = transform.Find("SwitchContext").transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    mainManager = GameObject.Find("Manager").GetComponent<BattleMainManagerScript>();
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
  }

  public void ToBattleMode() {
    switchContextText.text = "Return to Base";
  }

  public void SwitchContextPressed() {
    mainManager.SwitchContext();
  }
}
