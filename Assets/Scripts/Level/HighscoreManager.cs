using System;
using UnityEngine;

public static class HighscoreManager
{
    private const string SCORE_KEY = "Highscore";

    public static int GetHighscore()
    {
        if (!PlayerPrefs.HasKey(SCORE_KEY))
        {
            PlayerPrefs.SetInt(SCORE_KEY, 0);
            PlayerPrefs.Save();
            return 0;
        }

        return PlayerPrefs.GetInt(SCORE_KEY);
    }
    /// <returns>True if its a new highscore</returns>
    public static bool SaveHighscore(int newScore)
    {
        var savedScore = GetHighscore();

        if (newScore > savedScore)
        {
            PlayerPrefs.SetInt(SCORE_KEY, newScore);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }

    public static void ClearHighscore()
    {
        PlayerPrefs.SetInt(SCORE_KEY, 0);
        PlayerPrefs.Save();
    }
}