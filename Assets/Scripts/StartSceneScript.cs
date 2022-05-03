using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour {
  public void StartGame() {
    SceneManager.LoadScene("BattleScene");
  }
}
