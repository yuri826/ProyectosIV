using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject controlsCanvas;
    [SerializeField] private GameObject instructionsCanvas01;
    [SerializeField] private GameObject instructionsCanvas02;

    [Header("Menus")]
    [SerializeField] private CanvasMenu mainMenu;
    [SerializeField] private CanvasMenu controlsMenu;
    [SerializeField] private CanvasMenu instructionsMenu01;
    [SerializeField] private CanvasMenu instructionsMenu02;

    [Header("Scene Loading")]
    [SerializeField] private SceneLoadManager sceneLoadManager;
    [SerializeField] private string firstLevelSceneName;

    private GameObject previousCanvas;

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        previousCanvas = null;

        mainMenuCanvas.SetActive(true);
        controlsCanvas.SetActive(false);
        instructionsCanvas01.SetActive(false);
        instructionsCanvas02.SetActive(false);

        mainMenu.ResetSelection();
    }

    public void OpenControlsFromMainMenu()
    {
        previousCanvas = mainMenuCanvas;

        mainMenuCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
        instructionsCanvas01.SetActive(false);
        instructionsCanvas02.SetActive(false);

        controlsMenu.ResetSelection();
    }

    public void OpenInstructions01FromMainMenu()
    {
        previousCanvas = mainMenuCanvas;

        mainMenuCanvas.SetActive(false);
        controlsCanvas.SetActive(false);
        instructionsCanvas01.SetActive(true);
        instructionsCanvas02.SetActive(false);

        instructionsMenu01.ResetSelection();
    }

    public void OpenInstructions02()
    {
        previousCanvas = instructionsCanvas01;

        mainMenuCanvas.SetActive(false);
        controlsCanvas.SetActive(false);
        instructionsCanvas01.SetActive(false);
        instructionsCanvas02.SetActive(true);

        instructionsMenu02.ResetSelection();
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

    public void ContinueFromInstructions02()
    {
        SceneLoadManager.instance.LoadScene(firstLevelSceneName,1);
    }

    public void QuitGame()
    {
        sceneLoadManager.QuitGame();
    }
}