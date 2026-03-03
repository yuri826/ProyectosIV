using System;
using UnityEngine;

public class DepositObj : MonoBehaviour
{
    [SerializeField] private string[] objectTypeList;
    private int objectIndex = 0;
    public string state { get; private set; }= "objects";
    
    [SerializeField] private float maxRepairTimer;
    private float repairTimer = 0f;
    private bool repairing = false;

    private void Update()
    {
        if (repairing)
        {
            repairTimer += Time.deltaTime;

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
        Debug.Log("Completed");
    }
}
