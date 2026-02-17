using UnityEngine;

public class DepositObj : MonoBehaviour
{
    [SerializeField] private string objectType;

    public void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        if (pickableObj.type == objectType)
        {
            ExecuteAction();

            isCorrectObject = true;
        }
        else
        {
            isCorrectObject = false;
        }
    }

    protected virtual void ExecuteAction()
    {
        Debug.Log("Cojonudo");
    }
}
