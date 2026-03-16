using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMapScene([Tooltip("Poner el número del nivel aquí")] int mapIndex)
    {
        int thisLevel = mapIndex;
        int nextLevel = thisLevel+1;
        
        levelManager.CurrentLevel = thisLevel;
        levelManager.NewLevel = nextLevel;
        
        SceneManager.LoadScene("MapScene");
    }
    
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
