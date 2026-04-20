using UnityEngine;

public class EngineObj : DepositObj
{
    [Tooltip("Poner en orden correspondiente al array de strings de objetos aceptados")]
    [SerializeField] private float speedAdding = 30f;

    protected override void Start()
    {
        //
    }

    public override void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;

        for (var i = 0; i < objectTypeList.Length; i++)
        {
            if (pickableObj.type != objectTypeList[i]) continue;
    
            isCorrectObject = true;
            
            Completed();
            return;
        }
    }

    protected override void Completed()
    {
        TrainGameMode.instance.GetSpeedManager().AddSpeed(speedAdding);
    }
}
