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
    private Vector3 lookDir = Vector3.right;

    [Header("Pickables")] 
    private PickableObj currentObj = null;
    private DepositObj depositObj = null;
    private bool isPicking = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerInput.actions["Move"].performed += UpdateMovementInput;
        playerInput.actions["Move"].canceled += UpdateMovementInput;
        playerInput.actions["Interact"].started += Interact;
    }
    
    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= UpdateMovementInput;
        playerInput.actions["Move"].canceled -= UpdateMovementInput;
        playerInput.actions["Interact"].started -= Interact;
        
    }

    private void Update()
    {
        Movement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PickableObj>(out var pickableObj) && !isPicking)
        {
            currentObj = pickableObj;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<DepositObj>(out var depositObj))
        {
            this.depositObj = depositObj;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PickableObj>(out var pickableObj))
        {
            currentObj = null;
        }
        
        if (other.TryGetComponent<DepositObj>(out var depositObj))
        {
            this.depositObj = null;
        }
    }

    private void Movement()
    {
        movementDirection = new Vector3(movementInputDirection.x, 0f, movementInputDirection.y);
        characterController.Move(movementDirection * (walkSpeed * Time.deltaTime));
        
        if (movementDirection != Vector3.zero) lookDir = movementDirection;
    }

    private void UpdateMovementInput(InputAction.CallbackContext obj)
    {
        movementInputDirection = obj.ReadValue<Vector2>();
    }
    
    private void Interact(InputAction.CallbackContext obj)
    {
        Debug.Log("Interact");
        switch (isPicking)
        {
            case false:
                
                if (currentObj != null)
                {
                    currentObj.OnPick(this.transform);
                    isPicking = true;
                }

                break;

            case true:

                if (depositObj != null)
                {
                    depositObj.OnObject(currentObj, out var correctObject);
                    
                    if (correctObject) Destroy(currentObj.gameObject);
                    else currentObj.Drop(Quaternion.Euler(0,-90,0) * lookDir);
                    
                    isPicking = false;
                }
                else
                {
                    currentObj.Drop(lookDir);
                    isPicking = false;
                }

                break;
        }
    }
}
