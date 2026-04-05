using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelIntroManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject introCanvas;

    [Header("Input")]
    [SerializeField] private PlayerInput gameplayPlayerInput;

    [Header("Hold To Start")]
    [SerializeField] private Image holdFillImage;
    [SerializeField] private float holdDuration = 1.2f;

    private InputAction uiSelectAction;

    private bool introActive = false;
    private bool introCompleted = false;
    private bool isHolding = false;
    private float currentHoldTime = 0f;

    private void Awake()
    {
        InputActionMap uiMap = gameplayPlayerInput.actions.FindActionMap("UI", true);
        uiSelectAction = uiMap.FindAction("Select", true);
    }

    private void OnEnable()
    {
        uiSelectAction.started += OnSelectStarted;
        uiSelectAction.canceled += OnSelectCanceled;
    }

    private void OnDisable()
    {
        uiSelectAction.started -= OnSelectStarted;
        uiSelectAction.canceled -= OnSelectCanceled;
    }

    private void Update()
    {
        if (!introActive || introCompleted || !isHolding)
        {
            return;
        }

        currentHoldTime += Time.unscaledDeltaTime;
        holdFillImage.fillAmount = currentHoldTime / holdDuration;

        if (currentHoldTime >= holdDuration)
        {
            CompleteIntro();
        }
    }

    public void OpenIntro()
    {
        introActive = true;
        introCompleted = false;
        isHolding = false;
        currentHoldTime = 0f;

        holdFillImage.fillAmount = 0f;
        introCanvas.SetActive(true);

        gameplayPlayerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 1f;
    }

    private void OnSelectStarted(InputAction.CallbackContext context)
    {
        if (!introActive || introCompleted)
        {
            return;
        }

        isHolding = true;
    }

    private void OnSelectCanceled(InputAction.CallbackContext context)
    {
        if (!introActive || introCompleted)
        {
            return;
        }

        isHolding = false;
        currentHoldTime = 0f;
        holdFillImage.fillAmount = 0f;
    }

    private void CompleteIntro()
    {
        introActive = false;
        introCompleted = true;
        isHolding = false;

        holdFillImage.fillAmount = 1f;
        introCanvas.SetActive(false);

        TrainGameMode.instance.StartLevelCountdown();
    }
    
    public void SwitchToGameplayInput()
    {
        gameplayPlayerInput.SwitchCurrentActionMap("Gameplay");
    }
}
