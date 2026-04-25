using System;
using UnityEngine;

[Serializable]
public class TrainLife : GamemodeSubsystem
{
    [field: SerializeField] public int maxTrainLife { get; set; }
    public float currentTrainLife { get; private set; }

    public bool isDead { get; private set; } = false;

    public override void OnStart()
    {
        currentTrainLife = maxTrainLife;
        isDead = false;
        UpdateLifeBar();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentTrainLife -= amount;
        currentTrainLife = Mathf.Clamp(currentTrainLife, 0f, maxTrainLife);

        UpdateLifeBar();

        //Si es menor a 0 muere y se lo dice al gamemode
        if (currentTrainLife <= 0f)
        {
            isDead = true;
            if (TrainGameMode.onGameOver != null) TrainGameMode.onGameOver();
        }
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

    private void UpdateLifeBar()
    {
        TrainGameMode.UpdateLifeBar(currentTrainLife, maxTrainLife);
    }
}
