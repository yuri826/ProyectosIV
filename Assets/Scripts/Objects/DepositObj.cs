using System;
using UnityEngine;
using UnityEngine.UI;

public class DepositObj : MonoBehaviour
{
    [SerializeField] protected string[] objectTypeList;
    protected int objectIndex = 0;
    public string state { get; protected set; }= "objects";
    
    [SerializeField] protected GameObject repairBar;
    [SerializeField] protected Image repairBarImage;
    
    [SerializeField] protected float maxRepairTimer;
    protected float repairTimer = 0f;
    protected bool repairing = false;

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
                state = "completed";
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
        
        switch (state)
        {
            //Al estar en este estado acepta objetos
            case "objects":
                
                if (pickableObj.type == objectTypeList[objectIndex])
                {
                    Debug.Log("Correct object");
            
                    objectIndex++;

                    if (objectIndex == objectTypeList.Length)
                    {
                        state = "tool";
                    }
            
                    isCorrectObject = true;
                }
                else
                {
                    isCorrectObject = false;
                }

                break;
            
            //En este estado se puede ejecutar la reparación
            case "tool":
                
            //Aquí ya se completó
            case "completed": break;
        }
    }

    public void OnTool()
    {
        repairBar.SetActive(true);
        PlayerSystem.instance.SetState("repair");
        repairing = true;
    }

    public void RemoveTool()
    {
        PlayerSystem.instance.SetState("move");
        repairing = false;
    }

    protected virtual void Completed()
    {
        repairBar.SetActive(false);
        Debug.Log("Completed");
    }
}
