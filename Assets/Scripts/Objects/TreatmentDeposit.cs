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
                state = "completed";
                RemoveTool();
                Completed();
                return;
            }
        }
    }

    public override void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;
        
        switch (state)
        {
            case "objects":

                for (var i = 0; i < objectTypeList.Length; i++)
                {
                    if (pickableObj.type != objectTypeList[i]) continue;
                    Debug.Log("Correct object");
            
                    objectIndex = i;
                    state = "tool";
            
                    isCorrectObject = true;
                    return;
                }

                break;
            
            case "tool":
            
            case "completed": break;
        }
    }

    protected override void Completed()
    {
        repairBar.SetActive(false);
        
        Debug.Log(objectIndex);
        Debug.Log(treatedObjects.Length);
        
        PickableObj pickableObj = Instantiate(treatedObjects[objectIndex], treatedObjectSpawn).GetComponent<PickableObj>();
        PlayerSystem.instance.ForcePick(pickableObj);
        
        state = "objects";
    }
}
