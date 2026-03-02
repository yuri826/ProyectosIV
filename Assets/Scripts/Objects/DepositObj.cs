using UnityEngine;

public class DepositObj : MonoBehaviour
{
    [SerializeField] private string[] objectTypeList;
    private int objectIndex = 0;
    private bool isCompleted = false;

    public virtual void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        if (isCompleted)
        {
            print("Already completed");
            isCorrectObject = false;
            return;
        }

        if (pickableObj.type == objectTypeList[objectIndex])
        {
            Debug.Log("Correct object");
            
            objectIndex++;

            if (objectIndex == objectTypeList.Length)
            {
                ExecuteAction();
            }
            
            isCorrectObject = true;
        }
        else
        {
            isCorrectObject = false;
        }
    }

    protected virtual void ExecuteAction()
    {
        isCompleted = true;
        Debug.Log("DONE");    
    }
}
