using UnityEngine;

public class LevelSelect : MonoBehaviour
{
  [System.Serializable]
  public struct ButtonPlayerPrefs
  {
    public GameObject gameObject;
    public string playerPrefKey;
  }
  public ButtonPlayerPrefs[] buttons;
  void Start()
  {
    for (int i = 0; i < buttons.Length; i++)
    {
      int score = PlayerPrefs.GetInt(buttons[i].playerPrefKey, 0);

      for (int startIndex = 1; startIndex <= 3; startIndex++)
      {
        Transform star = buttons[i].gameObject.transform.Find("star" + startIndex);
        star.gameObject.SetActive(startIndex <= score);
      }
    }
  }

  public void OnButtonPress(string levelName)
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
  }


}
