using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class LevelIntroManager : GamemodeSubsystem
{
    [Header("Canvas")]
    [SerializeField] private GameObject introCanvas;

    [Header("Hold To Start")]
    [SerializeField] private Image holdFillImage;
    [SerializeField] private float holdDuration = 1.2f;

    [SerializeField] private PlayerInput playerInput;

    private bool introActive = false;
    private bool introCompleted = false;
    private bool isHolding = false;
    private float currentHoldTime = 0f;

    public override void OnStart()
    {
        //playerInput = TrainGameMode.GetPlayerInput(0);
        
        playerInput.actions["Select"].started += OnSelectStarted;
        playerInput.actions["Select"].canceled += OnSelectCanceled;

        OpenIntro();
    }

    public override void OnUpdate()
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

        Time.timeScale = 1f;
    }

    private void OnSelectStarted(InputAction.CallbackContext context)
    {
        Debug.Log("hold");
        if (!introActive || introCompleted)
        {
            return;
        }

        isHolding = true;
    }

    private void OnSelectCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("release");
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
        playerInput.actions["Select"].started -= OnSelectStarted;
        playerInput.actions["Select"].canceled -= OnSelectCanceled;
        
        introActive = false;
        introCompleted = true;
        isHolding = false;

        holdFillImage.fillAmount = 1f;
        introCanvas.SetActive(false);

        TrainGameMode.StartLevelCountdown();
    }
}
