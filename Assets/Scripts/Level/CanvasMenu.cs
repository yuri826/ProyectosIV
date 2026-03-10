using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasMenu : MonoBehaviour
{
    [SerializeField] private UIButton[] buttons;
    private int currentButtonIndex = 0;
    private PlayerInput playerInput;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        buttons[currentButtonIndex].OnHover();
    }

    private void OnEnable()
    {
        playerInput.actions["Up"].started += ButtonUp;
        playerInput.actions["Down"].started += ButtonDown;
        playerInput.actions["Select"].started += OnClick;
    }

    private void OnDisable()
    {
        playerInput.actions["Up"].started -= ButtonUp;
        playerInput.actions["Down"].started -= ButtonDown;
        playerInput.actions["Select"].started -= OnClick;
    }
    
    private void OnClick(InputAction.CallbackContext obj)
    {
        buttons[currentButtonIndex].OnSelect();
    }

    private void ButtonDown(InputAction.CallbackContext obj)
    {
        Debug.Log(buttons[currentButtonIndex].name);
        
        buttons[currentButtonIndex].OnDeHover();
        
        currentButtonIndex++;

        if (currentButtonIndex >= buttons.Length)
        {
            currentButtonIndex = 0;
        }
        else if (currentButtonIndex < 0)
        {
            currentButtonIndex = buttons.Length - 1;
        }
        
        buttons[currentButtonIndex].OnHover();
    }

    private void ButtonUp(InputAction.CallbackContext obj)
    {
        Debug.Log(buttons[currentButtonIndex].name);
        
        buttons[currentButtonIndex].OnDeHover();

        currentButtonIndex--;
        
        if (currentButtonIndex >= buttons.Length)
        {
            currentButtonIndex = 0;
        }
        else if (currentButtonIndex < 0)
        {
            currentButtonIndex = buttons.Length - 1;
        }
        
        buttons[currentButtonIndex].OnHover();
    }
}
