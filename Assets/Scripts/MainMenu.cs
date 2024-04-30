using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const int MENU_SCENE = 0;
    private const int PLAYER_SCENE = 1;
    private const int TOMB_SCENE = 2;

    [SerializeField] GameObject mainUI;
    [SerializeField] GameObject loadingUI;
    [SerializeField] GameObject helpUI;
    [SerializeField] TextMeshProUGUI highscoreTxt;
    [SerializeField] GameObject highscoreUI;

    private void Start()
    {
        mainUI.SetActive(true);
        loadingUI.SetActive(false);
        helpUI.SetActive(false);
    }

    public void LoadGame()
    {
        mainUI.SetActive(false);
        loadingUI.SetActive(true);

        SceneManager.LoadScene(PLAYER_SCENE, LoadSceneMode.Single);
        //SceneManager.LoadScene(TOMB_SCENE, LoadSceneMode.Additive);
    }
    public void ToggleHelp(bool show)
    {
        mainUI.SetActive(!show); 
        helpUI.SetActive(show);
    }
    public void ResetScore()
    {
        HighscoreManager.ClearHighscore();
        LoadHighscore();
    }
    public void LoadHighscore()
    {
        int score = HighscoreManager.GetHighscore();
        highscoreUI.SetActive(score > 0);

        if (score > 0)
            highscoreTxt.text = "Highest Floor: " + score;
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
