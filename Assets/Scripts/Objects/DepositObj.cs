using UnityEngine;

public class DepositObj : MonoBehaviour
{
    [SerializeField] private string[] objectTypeList;
    private int objectIndex = 0;

    public virtual void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        if (pickableObj.type == objectTypeList[objectIndex])
        {
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
        Debug.Log("DONE");    
    }
}
