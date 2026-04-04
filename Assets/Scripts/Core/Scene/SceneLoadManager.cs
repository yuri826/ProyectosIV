using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMapScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapScene");
    }

    public void LoadMapSceneFromLevel(int mapIndex)
    {
        Time.timeScale = 1f;

        if (levelManager == null)
        {
            Debug.LogWarning("SceneLoadManager: LevelManager no está asignado.");
            SceneManager.LoadScene("MapScene");
            return;
        }

        int thisLevel = mapIndex;
        int nextLevel = thisLevel + 1;

        levelManager.CurrentLevel = thisLevel;
        levelManager.NewLevel = nextLevel;

        SceneManager.LoadScene("MapScene");
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
