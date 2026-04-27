using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject controlsCanvas;
    [SerializeField] private GameObject instructionsCanvas01;
    [SerializeField] private GameObject instructionsCanvas02;
    [SerializeField] private Animator transitionAnim;

    [Header("Menus")]
    [SerializeField] private CanvasMenu mainMenu;
    [SerializeField] private CanvasMenu controlsMenu;
    [SerializeField] private CanvasMenu instructionsMenu01;
    [SerializeField] private CanvasMenu instructionsMenu02;

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
        LoadScene("MapScene",1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    private void LoadScene(string sceneName, int time)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneRoutine(sceneName, time));
        transitionAnim.SetTrigger("TransitionIn");
    }

    private IEnumerator LoadSceneRoutine(string sceneName, int time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(sceneName);
    } 
}