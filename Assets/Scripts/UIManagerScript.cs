using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Base;
using Assets.Scripts.UnityBase;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManagerScript : MonoBehaviour {
  private TMPro.TMP_Text switchContextText;

  private BattleMainManagerScript mainManager;
  private GameObject inventoryUIHolder;

  private UnequippedButtonScript[] buttons;

  private TextureHandler textureHandler = new();

  // Start is called before the first frame update
  void Awake() {
    switchContextText = transform.Find("SwitchContext").transform.Find("Text").GetComponent<TMPro.TMP_Text>();
    mainManager = GameObject.FindObjectOfType<BattleMainManagerScript>();
    inventoryUIHolder = transform.Find("inventory").gameObject;
    buttons = FindObjectsOfType<UnequippedButtonScript>()
      .OrderBy(button => (-button.transform.position.y) * 100000 + button.transform.position.x)
      .ToArray();
  }

  public void ToInventoryMode() {
    switchContextText.text = "Launch to battle";
    inventoryUIHolder.SetActive(true);
    buttons.ForEach((button, index) => {
      if (index < 16) {
        button.LoadEquipment(new WeaponInstance { config = WeaponConfig.LASER }, textureHandler);
      } else {
        button.LoadEquipment(null, textureHandler);
      }
    });
  }

  public void ToBattleMode() {
    switchContextText.text = "Return to Base";
    inventoryUIHolder.SetActive(false);
  }

  public void SwitchContextPressed() {
    mainManager.SwitchContext();
  }

  public void InventoryButtonSelected(UnequippedButtonScript button) {
    Debug.Log("clicked " + button.gameObject.name);
  }
}
