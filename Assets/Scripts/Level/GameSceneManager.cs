using Assets.Scripts.Level;
using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private const int PLAYER_SCENE = 1;
    private const int TOMB_SCENE = 2;

    public int currentScene = 0;
    public bool isLoading;

    public Transform returnPosition;

    void Start()
    {
        currentScene = PLAYER_SCENE;
        isLoading = false;
    }

    public void HitButton()
    {
        if (isLoading)
            return;

        if (currentScene == PLAYER_SCENE)
            LoadRandomLevel();
    }
    public void LoadRandomLevel()
    {
        LoadScene(TOMB_SCENE);
    }

    public void LoadScene(int scene) => StartCoroutine(LoadSceneRoutine(scene));

    IEnumerator LoadSceneRoutine(int scene)
    {
        Debug.Log("Loading scene: " + scene);
        isLoading = true;
        currentScene = scene;
        var asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        yield return new WaitUntil(() => asyncLoad.isDone);

        //globalLight.SetActive(false);

        isLoading = false;

        var lvlInfo = FindObjectOfType<LevelInfo>();
        lvlInfo.Initialize();
    }
    public void ReturnToCamp() => UnloadScene(currentScene);
    public void UnloadScene(int scene)
    {
        SceneManager.UnloadSceneAsync(scene);
        currentScene = PLAYER_SCENE;

        var player = FindObjectOfType<PlayerEntity>();
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = returnPosition.position;
        player.GetComponent<CharacterController>().enabled = true;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}