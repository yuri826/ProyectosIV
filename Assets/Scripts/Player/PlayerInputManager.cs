using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    private void Start()
    {
        playerInput.actions["Move"].performed += playerMovement.UpdateMovementInput;
        playerInput.actions["Move"].canceled += playerMovement.UpdateMovementInput;
        playerInput.actions["Interact"].started += playerMovement.Interact;
        playerInput.actions["Act"].started += playerMovement.Act;
        playerInput.actions["Act"].canceled += playerMovement.UnAct;
        playerInput.actions["Dash"].started += playerMovement.StartDash;
        
        playerInput.actions["Interact"].started += TrainGameMode.instance.LevelIntroManager.OnSelectStarted;
        playerInput.actions["Interact"].canceled += TrainGameMode.instance.LevelIntroManager.OnSelectCanceled;
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= playerMovement.UpdateMovementInput;
        playerInput.actions["Move"].canceled -= playerMovement.UpdateMovementInput;
        playerInput.actions["Interact"].started -= playerMovement.Interact;
        playerInput.actions["Act"].started -= playerMovement.Act;
        playerInput.actions["Act"].canceled -= playerMovement.UnAct;
        playerInput.actions["Dash"].started -= playerMovement.StartDash;
        
        playerInput.actions["Interact"].started -= TrainGameMode.instance.LevelIntroManager.OnSelectStarted;
        playerInput.actions["Interact"].canceled -= TrainGameMode.instance.LevelIntroManager.OnSelectCanceled;
    }
}
