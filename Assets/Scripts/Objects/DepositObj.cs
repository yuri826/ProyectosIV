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

    private void Start()
    {
        repairBar.SetActive(false);
    }

    private void Update()
    {
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

    public virtual void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;
        
        switch (state)
        {
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
            
            case "tool":
            
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
