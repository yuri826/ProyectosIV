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
        isDead = false;
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

        if (currentTrainLife <= 0f)
        {
            isDead = true;

            if (TrainGameMode.onGameOver != null)
            {
                TrainGameMode.onGameOver();
            }
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

    public float GetCurrentTrainLife()
    {
        return currentTrainLife;
    }

    public int GetMaxTrainLife()
    {
        return maxTrainLife;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
