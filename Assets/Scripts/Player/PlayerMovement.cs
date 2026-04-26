using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int playerN;
    public int PlayerN => playerN;
    
    private PlayerInput playerInput;
    private CharacterController characterController;
    private PlayerWeapon playerWeapon;

    private Vector2 movementInputDirection;

    public PlayerState currentState = PlayerState.Locked;

    [Header("Movement")] 
    [SerializeField] private float walkSpeed;
    private Vector3 movementDirection;
    private Vector3 lookDir = Vector3.right;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;
    
    [Header("Grounding")]
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float groundedVerticalSpeed = -2f;
    private float verticalVelocity;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float maxDashTimer;
    private float dashTimer;

    [Header("Pickables")] 
    private PickableObj currentObj;
    private bool isPicking = false;

    [Header("Pushables")]
    private PushableObj currentPushable;
    private bool isMovingPushable = false;
    
    [Header("Interaction")]
    [SerializeField] private float interactionDistance;
    [SerializeField] private float interactionRadius;
    [SerializeField] private Vector3 interactionOffset;
    private DepositObj currentRepairDeposit;
    private BrakeLever currentBrakeLever;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    #region Input bind
    
    private void OnEnable()
    {
        playerInput.actions["Move"].performed += UpdateMovementInput;
        playerInput.actions["Move"].canceled += UpdateMovementInput;
        playerInput.actions["Interact"].started += Interact;
        playerInput.actions["Interact"].canceled += StopInteract;
        playerInput.actions["MoveObject"].started += StartMoveObject;
        playerInput.actions["MoveObject"].canceled += StopMoveObject;
        playerInput.actions["Act"].started += Act;
        playerInput.actions["Act"].canceled += UnAct;
        playerInput.actions["Dash"].started += StartDash;
    }


    private void OnDisable()
    {
        StopMovingPushable();

        playerInput.actions["Move"].performed -= UpdateMovementInput;
        playerInput.actions["Move"].canceled -= UpdateMovementInput;
        playerInput.actions["Interact"].started -= Interact;
        playerInput.actions["Interact"].canceled -= StopInteract;
        playerInput.actions["MoveObject"].started -= StartMoveObject;
        playerInput.actions["MoveObject"].canceled -= StopMoveObject;
        playerInput.actions["Act"].started -= Act;
        playerInput.actions["Act"].canceled -= UnAct;
        playerInput.actions["Dash"].started -= StartDash;
    }
    
    #endregion

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Move:

                if (isMovingPushable)
                    MovePushable();
                else
                    Movement();
                
                break;
            
            case PlayerState.Dash:
                
                if (dashTimer <= 0) currentState = PlayerState.Move;
                else dashTimer -= Time.deltaTime;

                Dash();

                break;
        }
        
        UpdateRotation();
    }
    
    #region Movement stuff
    
    //Trinca el input del jugador
    private void UpdateMovementInput(InputAction.CallbackContext obj)
    {
        movementInputDirection = obj.ReadValue<Vector2>();
    }
    
    //Para las caídas
    private Vector3 GetVerticalDisplacement()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundedVerticalSpeed;
        else
            verticalVelocity += gravity * Time.deltaTime;

        return Vector3.up * (verticalVelocity * Time.deltaTime);
    }
    
    //Displacement del sandstorm
    private Vector3 GetSandstormDisplacement(float baseSpeed)
    {
        Vector3 returnVector = Vector3.zero;

        if (SandstormSystem.Instance.isSandstormActive)
        {
            returnVector = SandstormSystem.Instance.GetWindDisplacement(baseSpeed) * Time.deltaTime;
        }

        return returnVector;
    }
    
    //Para la rotacion
    private void UpdateRotation()
    {
        //Mira la rotación a la que va, si es muy pequeña sale del método
        Vector3 flatLookDir = new Vector3(lookDir.x, 0f, lookDir.z);
        if (flatLookDir.sqrMagnitude <= 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(flatLookDir, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    
    //Movimiento base
    private void Movement()
    {
        //Movimiento base del input
        movementDirection = new Vector3(movementInputDirection.x, 0f, movementInputDirection.y);

        //Añade displacements varios
        //Walk
        Vector3 playerDisplacement = movementDirection * (walkSpeed * Time.deltaTime);
        //Sandstorm
        Vector3 sandstormDisplacement = GetSandstormDisplacement(walkSpeed);
        //Caída
        Vector3 verticalDisplacement = GetVerticalDisplacement();

        characterController.Move(playerDisplacement + sandstormDisplacement + verticalDisplacement);
    
        if (movementDirection != Vector3.zero)
        {
            lookDir = movementDirection;
        }
    }

    //Movimiento mientras empuja
    private void MovePushable()
    {
        if (currentPushable is null)
        {
            ClearPushableMovement();
            return;
        }

        movementDirection = new Vector3(movementInputDirection.x, 0f, movementInputDirection.y);

        if (movementDirection != Vector3.zero)
        {
            lookDir = movementDirection;
        }

        Vector3 pushableDisplacement = currentPushable.Move(movementDirection);
        Vector3 sandstormDisplacement = GetSandstormDisplacement(walkSpeed);
        Vector3 verticalDisplacement = GetVerticalDisplacement();

        characterController.Move(pushableDisplacement + sandstormDisplacement + verticalDisplacement);
    }

    //Dash
    private void Dash()
    {
        Vector3 dashDisplacement = lookDir * (dashSpeed * Time.deltaTime);
        Vector3 sandstormDisplacement = GetSandstormDisplacement(dashSpeed);
        Vector3 verticalDisplacement = GetVerticalDisplacement();

        characterController.Move(dashDisplacement + sandstormDisplacement + verticalDisplacement);
    }
    
    private void StartDash(InputAction.CallbackContext obj)
    {
        if ((currentState != PlayerState.Move) || 
            (playerWeapon != null && playerWeapon.isReloading)) return;

        dashTimer = maxDashTimer;
        currentState = PlayerState.Dash;
    }
    
    #endregion
    
    #region Interaction / Action
    
    private void Interact(InputAction.CallbackContext obj)
    {
        if ((currentState != PlayerState.Move) || 
            (playerWeapon != null && playerWeapon.isReloading)) return;

        Interaction();
    }
    
    private void StopInteract(InputAction.CallbackContext obj)
    {
        if (currentBrakeLever is not null)
        {
            currentBrakeLever?.OnRelease();
            currentState = PlayerState.Move;
        }
    }

    private void Interaction()
    {
        if (currentState != PlayerState.Move) return;
        
        Collider[] cols = Physics.OverlapSphere(this.transform.position + ((lookDir + interactionOffset) * interactionDistance), interactionRadius);
        bool canDrop = false;
        
        foreach (Collider col in cols)
        {
            //Not picking
            //Can pick objects from ground and box
            if (!isPicking)
            {
                // if (col.TryGetComponent<IInteractable>(out var interactable))
                // {
                    //print ("interactable collider");
                    
                if (col.TryGetComponent<PickableObj>(out var pickableObj))
                {
                    // Caso especial: si son balas y tengo hueco en el cinturón, se equipan directamente.
                    if (pickableObj.type == ResourceType.Bullets)
                    {
                        int ammoBatchAmount = playerWeapon.maxChamberAmmo;
                        int addedAmmo = playerWeapon.AddBeltAmmo(ammoBatchAmount);

                        if (addedAmmo > 0)
                        {
                            Destroy(pickableObj.gameObject);
                            goto EndOfInteraction;
                        }
                    }

                    pickableObj.OnPick(this.transform);
                    currentObj = pickableObj;
                    isPicking = true;

                    goto EndOfInteraction;
                }
                
                if (col.TryGetComponent<ObjectBox>(out var objectBox))
                {
                    //print("object box");
                    
                    PickableObj newPickable = Instantiate(objectBox.objectToSpawn).GetComponent<PickableObj>();
                    newPickable.OnPick(this.transform);
                    currentObj = newPickable;
                    isPicking = true;

                    goto EndOfInteraction;
                }

                if (col.TryGetComponent(out BrakeLever brakeLever))
                {
                    currentState = PlayerState.Locked;
                    currentBrakeLever = brakeLever;
                    currentBrakeLever.OnHold();
                }

                //throw new WarningException("Object with IInteractable which doesn't need it");
            }
            else
            {
                if (col.TryGetComponent<DepositObj>(out var deposit))
                {
                    deposit.OnObject(currentObj, out var correctObject);

                    if (correctObject)
                        Destroy(currentObj.gameObject);
                    else
                        currentObj.Drop(Quaternion.Euler(0, -90, 0) * lookDir);
                    
                    currentObj = null;

                    isPicking = false;

                    goto EndOfInteraction;
                }

                canDrop = true;
            }
        }

        if (canDrop)
        {
            currentObj?.Drop(lookDir);
            currentObj = null;
            isPicking = false;
        }
        
        EndOfInteraction: ; //print("end of interaction");
    }
    
    // private IEnumerator InteractionWait()
    // {
    //     yield return new WaitForEndOfFrame();
    //     Interaction();
    // }
    
    private void Act(InputAction.CallbackContext obj)
    {
        if ((currentState != PlayerState.Move)
            || (playerWeapon != null && playerWeapon.isReloading)) return;

        if (isPicking)
        {
            currentObj.Throw(lookDir, out var dropObj);

            if (!dropObj) return;
            
            currentObj = null;
            isPicking = false;
        }
        else
        {
            Collider[] cols = Physics.OverlapSphere(
                this.transform.position + ((lookDir + interactionOffset) * interactionDistance),
                interactionRadius
            );

            foreach (Collider col in cols)
            {
                if (col.TryGetComponent<DepositObj>(out var deposit) && deposit.currentState == DepositState.Tool)
                {
                    Debug.Log("Omg deposit to repair omg");
                    currentRepairDeposit = deposit;
                    deposit.OnTool(playerN);
                    goto EndOfAct;
                }
            }

            playerWeapon.Shoot(lookDir);

            EndOfAct:;
        }
    }
    
    #endregion

    #region PushableObjs

    private void StartMoveObject(InputAction.CallbackContext obj)
    {
        if ((currentState != PlayerState.Move) 
            || (playerWeapon != null && playerWeapon.isReloading)
            || (isPicking)) return;

        Collider[] cols = Physics.OverlapSphere(
            transform.position + ((lookDir + interactionOffset) * interactionDistance),
            interactionRadius
        );

        foreach (var t in cols)
        {
            if ((!t.TryGetComponent(out PushableObj pushable)) || 
                (!pushable.CanStartMoving(this))) continue;

            currentPushable = pushable;
            isMovingPushable = true;
            currentPushable.StartMoving(this);
            return;
        }
    }

    private void StopMoveObject(InputAction.CallbackContext obj)
    {
        StopMovingPushable();
    }
    
    private void StopMovingPushable()
    {
        if (!isMovingPushable) return;
        
        if (currentPushable is not null) currentPushable.StopMoving(this);

        ClearPushableMovement();
    }
    
    private void ClearPushableMovement()
    {
        currentPushable = null;
        isMovingPushable = false;
    }

    #endregion
    
    #region Outside state manipulation

    private void UnAct(InputAction.CallbackContext obj)
    {
        currentRepairDeposit?.RemoveTool();
        currentRepairDeposit = null;
    }
    
    public void ForcePick(PickableObj p)
    {
        if (isPicking) throw new ArgumentException("Forcing pick when picking");
            
        p.OnPick(this.transform);
        currentObj = p;
        isPicking = true;
    }

    public void ForceDropObj()
    {
        StopMovingPushable();

        if (!isPicking) return;

        currentObj.Drop(lookDir);
        currentObj = null;
        isPicking = false;
    }

    public void EnablePlayer()
    {
        currentState = PlayerState.Move;
    }
    
    public void DisablePlayer()
    {
        StopMovingPushable();
        currentState = PlayerState.Locked;
    }
    
    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position + ((lookDir + interactionOffset) * interactionDistance), interactionRadius);
    }
}
