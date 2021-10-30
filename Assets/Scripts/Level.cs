using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
  public LevelType Type => type;
  public Grid grid;
  public HUD hud;
  public int score1Star, score2Star, score3Star;
  protected LevelType type;
  protected int currentScore;
  private bool _didWin;
  // Start is called before the first frame update
  private void Start()
  {
    hud.SetScore(currentScore);
  }

  public virtual void GameWin()
  {
    grid.GameOver();
    _didWin = true;
    StartCoroutine(WaitForGridFill());
  }

  public virtual void GameLose()
  {
    grid.GameOver();
    _didWin = false;
    StartCoroutine(WaitForGridFill());
  }
  public virtual void OnMove()
  {
    Debug.Log("You moved!");
  }

  public virtual void OnPieceCleared(GamePiece piece)
  {
    currentScore += piece.score;
    hud.SetScore(currentScore);
  }

  protected virtual IEnumerator WaitForGridFill()
  {
    while (grid.isFilling)
    {
      yield return null;
    }

    if (_didWin)
    {
      hud.OnGameWin(currentScore);
    }
    else
    {
      hud.OnGameLose();
    }
  }
}
