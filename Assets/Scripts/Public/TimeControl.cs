using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pause And Play Game
/// </summary>
public static class TimeControl
{
    public delegate void PauseGame();
    public delegate void PlayGame();
    public static PauseGame Pause = null;
    public static PlayGame Play = null;

    static TimeControl()
    {
        Pause = () =>
        {
            Time.timeScale = 0f;
        };

        Play = () =>
        {
            Time.timeScale = 1f;
        };
    }
}
