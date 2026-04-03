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
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float maxDashTimer;
    private float dashTimer;

    [Header("Pickables")] 
    private PickableObj currentObj;
    private bool isPicking = false;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance;
    [SerializeField] private float interactionRadius;
    [SerializeField] private Vector3 interactionOffset;
    private DepositObj currentRepairDeposit;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void OnEnable()
    {
        playerInput.actions["Move"].performed += UpdateMovementInput;
        playerInput.actions["Move"].canceled += UpdateMovementInput;
        playerInput.actions["Interact"].started += Interact;
        playerInput.actions["Act"].started += Act;
        playerInput.actions["Act"].canceled += UnAct;
        playerInput.actions["Dash"].started += StartDash;
    }


    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= UpdateMovementInput;
        playerInput.actions["Move"].canceled -= UpdateMovementInput;
        playerInput.actions["Interact"].started -= Interact;
        playerInput.actions["Act"].started -= Act;
        playerInput.actions["Act"].canceled -= UnAct;
        playerInput.actions["Dash"].started -= StartDash;
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Move:

                Movement();
                
                break;
            
            case PlayerState.Dash:
                
                dashTimer -= Time.deltaTime;

                if (dashTimer <= 0)
                {
                    currentState = PlayerState.Move;
                }

                Dash();

                break;
            
            case PlayerState.Repair:

                break;
            
            default:
                break;
        }
    }

    private void Movement()
    {
        movementDirection = new Vector3(movementInputDirection.x, 0f, movementInputDirection.y);

        Vector3 playerDisplacement = movementDirection * (walkSpeed * Time.deltaTime);
        Vector3 sandstormDisplacement = GetSandstormDisplacement(walkSpeed);

        characterController.Move(playerDisplacement + sandstormDisplacement);
        
        if (movementDirection != Vector3.zero) lookDir = movementDirection;
    }

    private void Dash()
    {
        Vector3 dashDisplacement = lookDir * (dashSpeed * Time.deltaTime);
        Vector3 sandstormDisplacement = GetSandstormDisplacement(dashSpeed);

        characterController.Move(dashDisplacement + sandstormDisplacement);
    }
    
    private Vector3 GetSandstormDisplacement(float baseSpeed)
    {
        if (SandstormSystem.Instance == null)
        {
            return Vector3.zero;
        }

        if (!SandstormSystem.Instance.IsSandstormActive())
        {
            return Vector3.zero;
        }

        return SandstormSystem.Instance.GetWindDisplacement(baseSpeed) * Time.deltaTime;
    }
    
    private void StartDash(InputAction.CallbackContext obj)
    {
        if (currentState != PlayerState.Move)
        {
            return;
        }

        if (playerWeapon != null && playerWeapon.IsReloading())
        {
            return;
        }

        dashTimer = maxDashTimer;
        currentState = PlayerState.Dash;
    }

    private void UpdateMovementInput(InputAction.CallbackContext obj)
    {
        movementInputDirection = obj.ReadValue<Vector2>();
    }
    
    private void Interact(InputAction.CallbackContext obj)
    {
        if (currentState != PlayerState.Move)
        {
            return;
        }

        if (playerWeapon != null && playerWeapon.IsReloading())
        {
            return;
        }

        StartCoroutine(InteractionWait());
    }

    private IEnumerator InteractionWait()
    {
        yield return new WaitForEndOfFrame();
        Interaction();
    }

    private void Interaction()
    {
        if (currentState != PlayerState.Move) return;
        
        Collider[] cols = Physics.OverlapSphere(this.transform.position + ((lookDir + interactionOffset) * interactionDistance), interactionRadius);

        // print("interaction");
        // print($"{cols.Length} colliders found");
        // print($"isPicking: {isPicking}");

        bool canDrop = false;
        
        foreach (Collider col in cols)
        {
            //Not picking
            //Can pick objects from ground and box
            if (!isPicking)
            {
                if (col.TryGetComponent<IInteractable>(out var interactable))
                {
                    //print ("interactable collider");
                    
                    if (col.TryGetComponent<PickableObj>(out var pickableObj))
                    {
                        // Caso especial: si son balas y tengo hueco en el cinturón, se equipan directamente.
                        if (pickableObj.type == ResourceType.Bullets)
                        {
                            int ammoBatchAmount = playerWeapon.GetMaxChamberAmmo();
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

                    throw new WarningException("Object with IInteractable which doesn't need it");
                }

                //print("non interactable collider");
            }
            else
            {
                if (col.TryGetComponent<DepositObj>(out var deposit))
                {
                    deposit.OnObject(currentObj, out var correctObject);

                    if (correctObject)
                    {
                        Destroy(currentObj.gameObject);
                        currentObj = null;
                    }
                    else
                    {
                        currentObj.Drop(Quaternion.Euler(0, -90, 0) * lookDir);
                        currentObj = null;
                    }

                    isPicking = false;
                    canDrop = false;

                    goto EndOfInteraction;
                }

                canDrop = true;
            }
        }

        if (canDrop)
        {
            currentObj.Drop(lookDir);
            currentObj = null;
            isPicking = false;
        }
        
        EndOfInteraction: ; //print("end of interaction");
    }

    public void ForcePick(PickableObj p)
    {
        if (isPicking)
        {
            throw new ArgumentException("Forcing pick when picking");
            return;
        }
            
        p.OnPick(this.transform);
        currentObj = p;
        isPicking = true;
    }
    
    private void Act(InputAction.CallbackContext obj)
    {
        if (currentState != PlayerState.Move)
        {
            return;
        }

        if (playerWeapon != null && playerWeapon.IsReloading())
        {
            return;
        }

        if (isPicking)
        {
            currentObj.Throw(lookDir, out var dropObj);

            if (dropObj)
            {
                currentObj = null;
                isPicking = false;
            }
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

    private void UnAct(InputAction.CallbackContext obj)
    {
        currentRepairDeposit?.RemoveTool();
        currentRepairDeposit = null;
    }

    public void EnablePlayer()
    {
        currentState = PlayerState.Move;
    }
    
    public void DisablePlayer()
    {
        currentState = PlayerState.Locked;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position + ((lookDir + interactionOffset) * interactionDistance), interactionRadius);
    }
}
