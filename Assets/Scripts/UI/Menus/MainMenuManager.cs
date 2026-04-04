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
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        previousCanvas = null;

        controlsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        mainMenu.ResetSelection();
    }

    public void OpenControlsFromMainMenu()
    {
        previousCanvas = mainMenuCanvas;

        mainMenuCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
        controlsMenu.ResetSelection();
    }

    public void BackFromControls()
    {
        controlsCanvas.SetActive(false);

        if (previousCanvas == mainMenuCanvas)
        {
            mainMenuCanvas.SetActive(true);
            mainMenu.ResetSelection();
        }
    }

    public void StartGame()
    {
        sceneLoadManager.LoadMapScene();
    }

    public void QuitGame()
    {
        sceneLoadManager.QuitGame();
    }
}