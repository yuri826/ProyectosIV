using UnityEngine;
using UnityEngine.InputSystem;

public class LevelMenuManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private GameObject defeatCanvas;
    [SerializeField] private GameObject controlsCanvas;

    [Header("Menus")]
    [SerializeField] private CanvasMenu pauseMenu;
    [SerializeField] private WinCanvasMenu victoryMenu;
    [SerializeField] private CanvasMenu defeatMenu;
    [SerializeField] private CanvasMenu controlsMenu;

    [Header("Scene Loading")]
    [SerializeField] private SceneLoadManager sceneLoadManager;

    [Header("Input")]
    [SerializeField] private PlayerInput gameplayPlayerInput;

    [Header("Victory Stars")]
    [SerializeField] [Range(0f, 1f)] private float threeStarsLifePercent = 0.9f;
    [SerializeField] [Range(0f, 1f)] private float twoStarsLifePercent = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float oneStarLifePercent = 0.5f;

    private bool isPaused = false;
    private GameObject previousCanvasBeforeControls;

    private InputAction gameplayPauseAction;
    private InputAction uiPauseAction;

    private void Awake()
    {
        InputActionMap gameplayMap = gameplayPlayerInput.actions.FindActionMap("Gameplay", true);
        InputActionMap uiMap = gameplayPlayerInput.actions.FindActionMap("UI", true);

        if (gameplayMap != null)
        {
            gameplayPauseAction = gameplayMap.FindAction("Pause", true);
        }

        if (uiMap != null)
        {
            uiPauseAction = uiMap.FindAction("Pause", true);
        }
    }

    private void Start()
    {
        CloseAllMenus();
        Time.timeScale = 1f;
        SwitchToGameplayInput();
    }

    private void OnEnable()
    {
        RegisterPauseInput();
    }

    private void OnDisable()
    {
        UnregisterPauseInput();
    }

    private void RegisterPauseInput()
    {
        if (gameplayPauseAction != null)
        {
            gameplayPauseAction.started -= OnPausePressed;
            gameplayPauseAction.started += OnPausePressed;
        }

        if (uiPauseAction != null)
        {
            uiPauseAction.started -= OnPausePressed;
            uiPauseAction.started += OnPausePressed;
        }
    }

    private void UnregisterPauseInput()
    {
        if (gameplayPauseAction != null)
        {
            gameplayPauseAction.started -= OnPausePressed;
        }

        if (uiPauseAction != null)
        {
            uiPauseAction.started -= OnPausePressed;
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (controlsCanvas.activeSelf)
        {
            BackFromControls();
            return;
        }

        if (victoryCanvas.activeSelf)
        {
            return;
        }

        if (defeatCanvas.activeSelf)
        {
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        CloseAllMenus();

        pauseCanvas.SetActive(true);
        pauseMenu.ResetSelection();

        isPaused = true;
        Time.timeScale = 0f;
        SwitchToUIInput();
    }

    public void ResumeGame()
    {
        CloseAllMenus();
        isPaused = false;
        Time.timeScale = 1f;
        SwitchToGameplayInput();
    }

    public void OpenControlsFromPause()
    {
        previousCanvasBeforeControls = pauseCanvas;
        OpenControlsCanvas();
    }

    public void OpenControlsFromVictory()
    {
        previousCanvasBeforeControls = victoryCanvas;
        OpenControlsCanvas();
    }

    public void OpenControlsFromDefeat()
    {
        previousCanvasBeforeControls = defeatCanvas;
        OpenControlsCanvas();
    }

    public void BackFromControls()
    {
        controlsCanvas.SetActive(false);

        if (previousCanvasBeforeControls == pauseCanvas)
        {
            pauseCanvas.SetActive(true);
            pauseMenu.ResetSelection();
            return;
        }

        if (previousCanvasBeforeControls == victoryCanvas)
        {
            victoryCanvas.SetActive(true);

            if (victoryMenu != null)
            {
                victoryMenu.ResetSelection();
            }

            return;
        }

        if (previousCanvasBeforeControls == defeatCanvas)
        {
            defeatCanvas.SetActive(true);
            defeatMenu.ResetSelection();
        }
    }

    public void OpenVictoryMenu()
    {
        CloseAllMenus();

        victoryCanvas.SetActive(true);

        if (victoryMenu is not null)
        {
            victoryMenu.ResetSelection();
            victoryMenu.ResetStars();
            victoryMenu.StarShow(CalculateStars());
        }
        
        GameInstance.instance.LevelComplete(TrainGameMode.instance.GetLevelIndex(),CalculateStars());

        isPaused = false;
        Time.timeScale = 0f;
        SwitchToUIInput();
    }

    public void OpenDefeatMenu()
    {
        CloseAllMenus();

        defeatCanvas.SetActive(true);
        defeatMenu.ResetSelection();

        isPaused = false;
        Time.timeScale = 0f;
        SwitchToUIInput();
    }

    public void RestartLevel()
    {
        sceneLoadManager.ReloadScene();
    }

    public void GoToMainMenu()
    {
        SceneLoadManager.instance.LoadScene("MainMenu",1);
    }

    public void GoToMapMenu()
    {
        SceneLoadManager.instance.LoadScene("MapScene",1);
    }

    private void OpenControlsCanvas()
    {
        pauseCanvas.SetActive(false);
        victoryCanvas.SetActive(false);
        defeatCanvas.SetActive(false);

        controlsCanvas.SetActive(true);
        controlsMenu.ResetSelection();
    }

    private void CloseAllMenus()
    {
        pauseCanvas.SetActive(false);
        victoryCanvas.SetActive(false);
        defeatCanvas.SetActive(false);
        controlsCanvas.SetActive(false);
    }

    private void SwitchToUIInput()
    {
        gameplayPlayerInput.SwitchCurrentActionMap("UI");
    }

    private void SwitchToGameplayInput()
    {
        gameplayPlayerInput.SwitchCurrentActionMap("Gameplay");
    }

    private int CalculateStars()
    {
        float currentLife = TrainGameMode.instance.GetCurrentTrainLife();
        float maxLife = TrainGameMode.instance.GetMaxTrainLife();

        if (maxLife <= 0f)
        {
            return 0;
        }

        if (currentLife > maxLife * threeStarsLifePercent)
        {
            return 3;
        }

        if (currentLife > maxLife * twoStarsLifePercent)
        {
            return 2;
        }

        if (currentLife > maxLife * oneStarLifePercent)
        {
            return 1;
        }

        return 0;
    }
}
