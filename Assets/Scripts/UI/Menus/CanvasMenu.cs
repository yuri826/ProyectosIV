using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasMenu : MonoBehaviour
{
    [SerializeField] private UIButton[] buttons;
    [SerializeField] private int defaultButtonIndex = 0;
    [SerializeField] private float inputBlockOnEnableTime = 0.15f;

    private int currentButtonIndex = 0;
    private PlayerInput playerInput;
    private bool canReceiveInput = false;
    private Coroutine enableRoutine;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI");
            playerInput.actions["Up"].started += ButtonUp;
            playerInput.actions["Down"].started += ButtonDown;
            playerInput.actions["Select"].started += OnClick;
        }

        ResetSelection();

        canReceiveInput = false;

        if (enableRoutine != null)
        {
            StopCoroutine(enableRoutine);
        }

        enableRoutine = StartCoroutine(EnableInputAfterDelay());
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Up"].started -= ButtonUp;
            playerInput.actions["Down"].started -= ButtonDown;
            playerInput.actions["Select"].started -= OnClick;
        }

        if (enableRoutine != null)
        {
            StopCoroutine(enableRoutine);
            enableRoutine = null;
        }

        canReceiveInput = false;
    }

    private IEnumerator EnableInputAfterDelay()
    {
        yield return new WaitForSecondsRealtime(inputBlockOnEnableTime);
        canReceiveInput = true;
        enableRoutine = null;
    }

    public void ResetSelection()
    {
        if (buttons == null || buttons.Length == 0)
        {
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                continue;
            }

            buttons[i].OnDeHover();
        }

        currentButtonIndex = Mathf.Clamp(defaultButtonIndex, 0, buttons.Length - 1);

        if (buttons[currentButtonIndex] != null)
        {
            buttons[currentButtonIndex].OnHover();
        }
    }

    private void OnClick(InputAction.CallbackContext obj)
    {
        if (!canReceiveInput)
        {
            return;
        }

        if (buttons == null || buttons.Length == 0)
        {
            return;
        }

        if (buttons[currentButtonIndex] == null)
        {
            return;
        }

        buttons[currentButtonIndex].OnSelect();
    }

    private void ButtonDown(InputAction.CallbackContext obj)
    {
        if (!canReceiveInput)
        {
            return;
        }

        if (buttons == null || buttons.Length == 0)
        {
            return;
        }

        if (buttons[currentButtonIndex] != null)
        {
            buttons[currentButtonIndex].OnDeHover();
        }

        currentButtonIndex++;

        if (currentButtonIndex >= buttons.Length)
        {
            currentButtonIndex = 0;
        }

        if (buttons[currentButtonIndex] != null)
        {
            buttons[currentButtonIndex].OnHover();
        }
    }

    private void ButtonUp(InputAction.CallbackContext obj)
    {
        if (!canReceiveInput)
        {
            return;
        }

        if (buttons == null || buttons.Length == 0)
        {
            return;
        }

        if (buttons[currentButtonIndex] != null)
        {
            buttons[currentButtonIndex].OnDeHover();
        }

        currentButtonIndex--;

        if (currentButtonIndex < 0)
        {
            currentButtonIndex = buttons.Length - 1;
        }

        if (buttons[currentButtonIndex] != null)
        {
            buttons[currentButtonIndex].OnHover();
        }
    }
}
