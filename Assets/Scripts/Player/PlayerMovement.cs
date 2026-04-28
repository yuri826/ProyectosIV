using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int playerN;
    
    private PlayerInput playerInput;
    private CharacterController characterController;
    private PlayerAudioManager playerAudioManager;
    private PlayerWeapon playerWeapon;

    private Vector2 movementInputDirection;

    public PlayerState currentState = PlayerState.Locked;

    [Header("Movement")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float pushSpeed;
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
        playerAudioManager = GetComponent<PlayerAudioManager>();
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
        playerInput.actions["Act"].started += Act;
        playerInput.actions["Act"].canceled += UnAct;
        playerInput.actions["Dash"].started += StartDash;
    }


    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= UpdateMovementInput;
        playerInput.actions["Move"].canceled -= UpdateMovementInput;
        playerInput.actions["Interact"].started -= Interact;
        playerInput.actions["Interact"].canceled -= StopInteract;
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

                Movement(isMovingPushable ? pushSpeed : walkSpeed);

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
    private void Movement(float currentSpeed)
    {
        //Movimiento base del input
        movementDirection = new Vector3(movementInputDirection.x, 0f, movementInputDirection.y);

        //Añade displacements varios
        //Walk
        Vector3 playerDisplacement = movementDirection * (currentSpeed * Time.deltaTime);
        //Sandstorm
        Vector3 sandstormDisplacement = GetSandstormDisplacement(currentSpeed);
        //Caída
        Vector3 verticalDisplacement = GetVerticalDisplacement();

        characterController.Move(playerDisplacement + sandstormDisplacement + verticalDisplacement);
    
        if (movementDirection != Vector3.zero)
        {
            lookDir = movementDirection;
            currentPushable?.Move(movementDirection);
        }
    }

    //Dash
    private void Dash()
    {
        playerAudioManager.PlaySfx(PlayerSFX.dash);
        
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
        print("Stopinteraction");

        //If using brake
        if (currentBrakeLever is null) goto MOVING_PUSHABLE;
        
        currentBrakeLever.OnRelease(this);
        currentBrakeLever = null;
        currentState = PlayerState.Move;
        
        //If moving pushable
        MOVING_PUSHABLE:
        
        if (!isMovingPushable) return;
        if (currentPushable is not null) currentPushable.StopMoving(this);

        currentPushable = null;
        isMovingPushable = false;
    }

    private void Interaction()
    {
        print("Interaction init");
        
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
                        playerAudioManager.PlaySfx(PlayerSFX.bulletPick);
                        
                        int ammoBatchAmount = playerWeapon.maxChamberAmmo;
                        int addedAmmo = playerWeapon.AddBeltAmmo(ammoBatchAmount);

                        if (addedAmmo > 0)
                        {
                            Destroy(pickableObj.gameObject);
                            goto EndOfInteraction;
                        }
                    }

                    playerAudioManager.PlaySfx(PlayerSFX.pickUpObj);
                    
                    pickableObj.OnPick(this.transform);
                    currentObj = pickableObj;
                    isPicking = true;

                    goto EndOfInteraction;
                }
                
                if (col.TryGetComponent<ObjectBox>(out var objectBox))
                {
                    //print("object box");
                    playerAudioManager.PlaySfx(PlayerSFX.pickUpObj);
                    
                    PickableObj newPickable = Instantiate(objectBox.objectToSpawn).GetComponent<PickableObj>();
                    newPickable.OnPick(this.transform);
                    currentObj = newPickable;
                    isPicking = true;

                    goto EndOfInteraction;
                }

                if (col.TryGetComponent(out PushableObj pushable))
                {
                    if (!pushable.CanStartMoving()) goto EndOfInteraction;
                    
                    currentPushable = pushable;
                    isMovingPushable = true;
                    currentPushable.StartMoving(this);
                    goto EndOfInteraction;
                }

                if (col.TryGetComponent(out BrakeLever brakeLever))
                {
                    currentState = PlayerState.Locked;
                    currentBrakeLever = brakeLever;
                    currentBrakeLever.OnHold(this);
                }

                //throw new WarningException("Object with IInteractable which doesn't need it");
            }
            else
            {
                if (col.TryGetComponent<DepositObj>(out var deposit))
                {
                    playerAudioManager.PlaySfx(PlayerSFX.putDownObj);
                    
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
            playerAudioManager.PlaySfx(PlayerSFX.putDownObj);
            
            currentObj?.Drop(lookDir);
            currentObj = null;
            isPicking = false;
        }
        
        EndOfInteraction: ; //print("end of interaction");
    }
    
    private void Act(InputAction.CallbackContext obj)
    {
        if ((currentState != PlayerState.Move)
            || (playerWeapon != null && playerWeapon.isReloading)) return;

        if (isPicking)
        {
            playerAudioManager.PlaySfx(PlayerSFX.throwObj);
            
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
                    playerAudioManager.PlaySfx(PlayerSFX.onTool);
                    
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

    
    #region Outside state manipulation

    public void DropPushable()
    {
        currentPushable = null;
        isMovingPushable = false;
    }

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
        currentState = PlayerState.Locked;
    }
    
    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position + ((lookDir + interactionOffset) * interactionDistance), interactionRadius);
    }
}
