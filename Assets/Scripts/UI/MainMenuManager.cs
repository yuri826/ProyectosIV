using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject controlsCanvas;

    [Header("Menus")]
    [SerializeField] private CanvasMenu mainMenu;
    [SerializeField] private CanvasMenu controlsMenu;

    [Header("Scene Loading")]
    [SerializeField] private SceneLoadManager sceneLoadManager;

    private GameObject previousCanvas;

    private void Start()
    {
        Debug.Log("Start");
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        Debug.Log("OpenMainMenu called");
        previousCanvas = null;

        if (controlsCanvas != null)
        {
            controlsCanvas.SetActive(false);
        }

        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }

        if (mainMenu != null)
        {
            mainMenu.ResetSelection();
        }
    }

    public void OpenControlsFromMainMenu()
    {
        Debug.Log("OpenControlsFromMainMenu called");
        Debug.Log($"Main active before: {mainMenuCanvas.activeSelf}");
        Debug.Log($"Controls active before: {controlsCanvas.activeSelf}");
        previousCanvas = mainMenuCanvas;

        mainMenuCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
        Debug.Log($"Main active after: {mainMenuCanvas.activeSelf}");
        Debug.Log($"Controls active after: {controlsCanvas.activeSelf}");
        controlsMenu.ResetSelection();
    }

    public void BackFromControls()
    {
        if (controlsCanvas != null)
        {
            controlsCanvas.SetActive(false);
        }

        if (previousCanvas == mainMenuCanvas && mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);

            if (mainMenu != null)
            {
                mainMenu.ResetSelection();
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame called");
        if (sceneLoadManager != null)
        {
            sceneLoadManager.LoadMapScene();
        }
    }

    public void QuitGame()
    {
        if (sceneLoadManager != null)
        {
            sceneLoadManager.QuitGame();
        }
    }
}