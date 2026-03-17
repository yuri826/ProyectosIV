using UnityEngine;
using UnityEngine.UI;

public class TreatmentDeposit : DepositObj
{
    [Tooltip("Poner en orden correspondiente al array de strings de objetos aceptados")]
    [SerializeField] private GameObject[] treatedObjects;
    [SerializeField] protected Transform treatedObjectSpawn;
    
    private void Update()
    {
        if (repairing)
        {
            repairTimer += Time.deltaTime;
            
            repairBarImage.fillAmount = repairTimer / maxRepairTimer;

            if (repairTimer >= maxRepairTimer)
            {
                currentState = DepositState.Completed;
                RemoveTool();
                GiveObj();
                return;
            }
        }
    }

    public override void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;
        
        switch (currentState)
        {
            case DepositState.Objects:

                for (var i = 0; i < objectTypeList.Length; i++)
                {
                    if (pickableObj.type != objectTypeList[i]) continue;
                    Debug.Log("Correct object");
            
                    objectIndex = i;
                    currentState = DepositState.Tool;
            
                    isCorrectObject = true;
                    return;
                }

                break;
            
            case DepositState.Tool:
            
            case DepositState.Completed: break;
        }
    }

    private void GiveObj()
    {
        repairBar.SetActive(false);
        
        GameObject pickableObj = Instantiate(treatedObjects[objectIndex], treatedObjectSpawn);
        pickableObj.transform.parent = null;
        PlayerSystem.instance.ForcePick(pickableObj.GetComponent<PickableObj>(), currentPlayer);
        
        currentState = DepositState.Objects;;
    }
}
