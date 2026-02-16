using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;

    private Vector2 movementInputDirection;

    [Header("Movement")] 
    [SerializeField] private float walkSpeed;
    private Vector3 movementDirection;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerInput.actions["Move"].performed += UpdateMovementInput;
        playerInput.actions["Move"].canceled += UpdateMovementInput;
    }
    
    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= UpdateMovementInput;
        playerInput.actions["Move"].canceled -= UpdateMovementInput;
    }

    private void Update()
    {
        Movement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PickableObj>(out var pickableObj))
        {
            pickableObj.parentTransform = this.transform;
            pickableObj.OnPick();
        }
    }

    private void Movement()
    {
        movementDirection = new Vector3(movementInputDirection.x, 0f, movementInputDirection.y);
        characterController.Move(movementDirection * (walkSpeed * Time.deltaTime));
    }

    private void UpdateMovementInput(InputAction.CallbackContext obj)
    {
        movementInputDirection = obj.ReadValue<Vector2>();
    }
}
