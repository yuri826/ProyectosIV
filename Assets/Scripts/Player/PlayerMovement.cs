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

    public string state { get; set; } = "locked";

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

    private void Start()
    {
        PlayerSystem.instance.AddPlayer(this, playerN);
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
        switch (state)
        {
            case "move":

                Movement();
                
                break;
            
            case "dash":
                
                dashTimer -= Time.deltaTime;

                if (dashTimer <= 0)
                {
                    state = "move";
                }

                Dash();

                break;
            
            case "repair":

                break;
        }
    }

    private void Movement()
    {
        movementDirection = new Vector3(movementInputDirection.x, 0f, movementInputDirection.y);
        characterController.Move(movementDirection * (walkSpeed * Time.deltaTime));
        
        if (movementDirection != Vector3.zero) lookDir = movementDirection;
    }

    private void Dash()
    {
        characterController.Move(lookDir * (dashSpeed * Time.deltaTime));
    }
    
    private void StartDash(InputAction.CallbackContext obj)
    {
        if (state == "move")
        {
            dashTimer = maxDashTimer;
            state = "dash";
        }
    }

    private void UpdateMovementInput(InputAction.CallbackContext obj)
    {
        movementInputDirection = obj.ReadValue<Vector2>();
    }
    
    private void Interact(InputAction.CallbackContext obj)
    {
        StartCoroutine(InteractionWait());
    }

    private IEnumerator InteractionWait()
    {
        yield return new WaitForEndOfFrame();
        Interaction();
    }

    private void Interaction()
    {
        if (state != "move") return;
        
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
                        // print("pickable object");
                        // print($"Type: {pickableObj.type}");
                        
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
                    
                    if (correctObject) Destroy(currentObj.gameObject);
                    else currentObj.Drop(Quaternion.Euler(0,-90,0) * lookDir);
                    
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
            Collider[] cols = Physics.OverlapSphere(this.transform.position + ((lookDir + interactionOffset) * interactionDistance), interactionRadius);

            foreach (Collider col in cols)
            {
                if (col.TryGetComponent<DepositObj>(out var deposit) && deposit.state == "tool")
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
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position + ((lookDir + interactionOffset) * interactionDistance), interactionRadius);
    }
}
