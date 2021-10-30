using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class HUD : MonoBehaviour
{
  public Level level;
  public GameOver gameOver;
  public Text remainingText, remainingSubText, targetText, targetSubtext, scoreText;
  public Image[] stars;
  private int _starIndex = 0;

  internal void SetRemaining(int v)
  {
    throw new NotImplementedException();
  }

  void Start()
  {
    for (int i = 0; i < stars.Length; i++)
    {
      stars[i].enabled = i == _starIndex;
    }
  }

  public void SetScore(int score)
  {
    scoreText.text = score.ToString();
    int visibleStar = 0;
    if (score >= level.score1Star && score < level.score2Star)
    {
      visibleStar = 1;
    }
    else if (score >= level.score2Star && score < level.score3Star)
    {
      visibleStar = 2;
    }
    else if (score >= level.score3Star)
    {
      visibleStar = 3;
    }

    for (int i = 0; i < stars.Length; i++)
    {
      stars[i].enabled = (i == visibleStar);
    }
    _starIndex = visibleStar;
  }

  public void SetTarget(int target)
  {
    targetText.text = target.ToString();
  }

  public void SetRemaining(string remaining)
  {
    remainingText.text = remaining;
  }

  public void SetLevelType(LevelType type)
  {
    switch (type)
    {
      case LevelType.MOVES:
        remainingSubText.text = "moves remaining";
        targetSubtext.text = "target score";
        break;
      case LevelType.OBSTACLE:
        remainingSubText.text = "moves remaining";
        targetSubtext.text = "bubbles remaining";
        break;
      case LevelType.TIMER:
        remainingSubText.text = "time remaining";
        targetSubtext.text = "target score";
        break;
    }
  }

  public void OnGameWin(int score)
  {
    gameOver.ShowWin(score, _starIndex);
    if (_starIndex > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0))
    {
      PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, _starIndex);
    }
  }

  public void OnGameLose()
  {
    gameOver.ShowLose();
  }
}
