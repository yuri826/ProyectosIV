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
            //Suma al tiempo de reparación
            repairTimer += Time.deltaTime;
            
            //Updatea barra
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

        GameObject treatedPrefab = treatedObjects[objectIndex];

        if (treatedPrefab is null)
        {
            currentState = DepositState.Objects;
            return;
        }

        PickableObj treatedPickable = treatedPrefab.GetComponent<PickableObj>();
        bool isBulletOutput = treatedPickable is not null && treatedPickable.type == ResourceType.Bullets;

        if (isBulletOutput)
        {
            PlayerMovement player = TrainGameMode.instance.GetPlayer(currentPlayer);
            PlayerWeapon weapon = player?.GetComponent<PlayerWeapon>();

            if (weapon is not null)
            {
                int ammoBatchAmount = weapon.maxChamberAmmo;
                int addedAmmo = weapon.AddBeltAmmo(ammoBatchAmount);

                // Si al menos una bala cabe en el cinturón, se añade directamente.
                // Si sobran, se pierden.
                if (addedAmmo > 0)
                {
                    currentState = DepositState.Objects;
                    return;
                }
            }

            // Si no cabía ninguna bala en el cinturón, se crea el pack físico en el mundo.
            Instantiate(treatedPrefab, treatedObjectSpawn.position, treatedObjectSpawn.rotation);

            currentState = DepositState.Objects;
            return;
        }

        GameObject pickableObj = Instantiate(treatedPrefab, treatedObjectSpawn.position, treatedObjectSpawn.rotation);
        TrainGameMode.instance.ForcePick(pickableObj.GetComponent<PickableObj>(), currentPlayer);

        currentState = DepositState.Objects;
    }
}
