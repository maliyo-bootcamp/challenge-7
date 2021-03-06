using UnityEngine;

public class LevelTimer : Level
{
  public int timeInSeconds;
  public int targetScore;
  private float timer;
  private bool timeOut = false;
  // private HUD hud;

  private void Start()
  {
    type = LevelType.TIMER;
    hud.SetLevelType(type);
    hud.SetScore(currentScore);
    hud.SetTarget(targetScore);
    hud.SetRemaining($"{timeInSeconds / 60}:{timeInSeconds % 60:00}");
  }

  private void Update()
  {
    if (timeOut) { return; }
    timer += Time.deltaTime;
    hud.SetRemaining($"{(int)Mathf.Max((timeInSeconds - timer) / 60, 0)}:{(int)Mathf.Max((timeInSeconds - timer) % 60, 0):00}");
    if (timeInSeconds - timer <= 0)
    {
      if (currentScore >= targetScore)
      {
        GameWin();
      }
    }
    else
    {
      GameLose();
    }
  }
}
