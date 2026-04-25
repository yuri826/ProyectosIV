using System.Linq;
using UnityEngine;

public class EngineObj : DepositObj
{
    [Tooltip("Poner en orden correspondiente al array de strings de objetos aceptados")]
    [SerializeField] private float speedAdding = 30f;

    protected override void Start()
    {
        //No hacer nada
    }

    public override void OnObject(PickableObj pickableObj, out bool isCorrectObject)
    {
        isCorrectObject = false;

        if (objectTypeList.All(t => pickableObj.type != t)) return;
        
        isCorrectObject = true;
        TrainGameMode.instance.GetSpeedManager().AddSpeed(speedAdding);
    }
}
