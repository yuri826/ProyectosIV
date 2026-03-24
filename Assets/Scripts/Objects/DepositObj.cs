using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DepositObj : MonoBehaviour
{
    [SerializeField] protected ResourceType[] objectTypeList;
    protected int objectIndex = 0;
    public DepositState currentState = DepositState.Objects;
    
    [SerializeField] protected GameObject repairBar;
    [SerializeField] protected Image repairBarImage;
    
    [SerializeField] protected float maxRepairTimer;
    protected float repairTimer = 0f;
    protected bool repairing = false;

    protected int currentPlayer;

    protected virtual void Start()
    {
        repairBar.SetActive(false);
    }

    private void Update()
    {
        //Aquí se maneja la lógica de mantener pulsado y reparar el objeto
        if (repairing)
        {
            repairTimer += Time.deltaTime;
            
            repairBarImage.fillAmount = repairTimer / maxRepairTimer;

            if (repairTimer >= maxRepairTimer)
            {
                currentState = DepositState.Completed;
                RemoveTool();
                Completed();
                return;
            }
        }
    }

    //Este método se llama cuando el jugador interactua con el depósito con un objeto
    public virtual void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;
        
        switch (currentState)
        {
            //Al estar en este estado acepta objetos
            case DepositState.Objects:
                
                if (pickableObj.type == objectTypeList[objectIndex])
                {
                    Debug.Log("Correct object");
            
                    objectIndex++;

                    if (objectIndex == objectTypeList.Length)
                    {
                        currentState = DepositState.Tool;
                    }
            
                    isCorrectObject = true;
                }
                else
                {
                    isCorrectObject = false;
                }

                break;
            
            //En este estado se puede ejecutar la reparación
            case DepositState.Tool:
                
            //Aquí ya se completó
            case DepositState.Completed: break;
        }
    }

    public void OnTool(int playerN)
    {
        if (!repairing)
        {
            currentPlayer = playerN;
            repairBar.SetActive(true);
            TrainGameMode.instance.SetPlayerState(PlayerState.Repair, playerN);
            repairing = true;
        }
    }

    public void RemoveTool()
    {
        Debug.Log(currentPlayer);
        TrainGameMode.instance.SetPlayerState(PlayerState.Move, currentPlayer);
        repairing = false;
    }

    protected virtual void Completed()
    {
        repairBar.SetActive(false);
        Debug.Log("Completed");
    }
}
