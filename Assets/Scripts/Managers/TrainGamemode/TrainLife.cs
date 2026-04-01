using System;
using UnityEngine;

[Serializable]
public class TrainLife : GamemodeSubsystem
{
    [field: SerializeField]
    public int maxTrainLife { get; set; }
    
    [field: SerializeField] private float currentTrainLife { get; set; }

    private bool isDead = false;

    public override void OnStart()
    {
        currentTrainLife = maxTrainLife;
        UpdateLifeBar();
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }
        
        currentTrainLife -= amount;
        currentTrainLife = Mathf.Clamp(currentTrainLife, 0f, maxTrainLife);
        
        UpdateLifeBar();
        
        if (currentTrainLife <= 0)
        {
            TrainGameMode.onGameOver();
        }
    }

    private void UpdateLifeBar()
    {
        TrainGameMode.UpdateLifeBar(currentTrainLife, maxTrainLife);
    }

    public void RepairTrain(float amount)
    {
        if (isDead)
        {
            return;
        }
    
        currentTrainLife += amount;
        currentTrainLife = Mathf.Clamp(currentTrainLife, 0f, maxTrainLife);
        
        UpdateLifeBar();
    }
}
