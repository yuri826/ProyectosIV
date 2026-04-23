using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScene(string sceneName, int time)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneRoutine(sceneName, time));
        TrainGameMode.instance.TransitionIn();
    }

    private IEnumerator LoadSceneRoutine(string sceneName, int time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(sceneName);
    } 

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        TrainGameMode.instance.TransitionIn();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        TrainGameMode.instance.TransitionIn();
    }
}
